using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveSandbox.Services;
using ReactiveSandbox.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace ReactiveSandbox.ViewModels;

public class MainWindowViewModel : ReactiveObject, IDisposable
{
    private readonly SourceCache<TrackViewModel, int> _tracksCache = new(track => track.Id);
    private readonly IDisposable _cleanup;
    private readonly ReadOnlyObservableCollection<TrackViewModel> _tracks;
    private bool _disposedValue;

    public ReadOnlyObservableCollection<TrackViewModel> InboundTracks => _tracks;

    public MainWindowViewModel(GeneratorService generatorService, IOptions<AppOption> options, ILoggerFactory loggerFactory)
    {
        var tracksSourceListCleanup = _tracksCache
            .Connect()
            .AutoRefresh(track => track.Text)
            .Sort(SortExpressionComparer<TrackViewModel>.Ascending(track => track.Id))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _tracks)
            .DisposeMany()
            .Subscribe();

        var expiredManagerCleanup = _tracksCache
            .Connect()
            .WhenPropertyChanged(track => track.State)
            .Subscribe(x =>
            {
                if (x.Value == State.Expired)
                {
                    _tracksCache.RemoveKey(x.Sender.Id);
                }
            });

        var generatorCleanup = generatorService.Tracks
            .Subscribe(trackDtos => _tracksCache.Edit(innerTracks =>
            {
                var futureTimeTolerance = DateTime.Now + options.Value.FutureToleranceTime;
                var expiredToleranceTime = DateTime.Now - options.Value.ExpiredToleranceTime;

                foreach (var trackDto in trackDtos)
{
                    if (trackDto.Time >= futureTimeTolerance || trackDto.Time <= expiredToleranceTime)
                    {
                        continue;
                    }

                    var track = innerTracks.Lookup(trackDto.Id);
                    if (track.HasValue)
                    {
                        track.Value.Update(trackDto);

                        //You may also need to do the following [if properties are used for filtering, sorts, grouping etc]
                        innerTracks.Refresh(track.Value.Id);
                    }
                    else
                    {
                        innerTracks.AddOrUpdate(new TrackViewModel(trackDto, options, loggerFactory.CreateLogger<TrackViewModel>()));
                    }
                }
            }));

        _cleanup = new CompositeDisposable(tracksSourceListCleanup, expiredManagerCleanup, generatorCleanup);
    }

    #region IDispoable
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _cleanup.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
