using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using ReactiveSandbox.Services;

namespace ReactiveSandbox.ViewModels;

internal class MainWindowViewModel : ReactiveObject, IDisposable
{
    private readonly SourceCache<TrackViewModel, int> _tracksCache = new(track => track.Id);
    private readonly IDisposable _cleanup;
    private readonly ReadOnlyObservableCollection<TrackViewModel> _tracks;
    private bool _disposedValue;

    public ReadOnlyObservableCollection<TrackViewModel> InboundTracks => _tracks;

    public MainWindowViewModel()
    {
        var tracksSourceListCleanup = _tracksCache
            .Connect()
            .ForEachChange(changedTrack =>
            {
                if (changedTrack.Current.State is Models.State.Expired)
                {
                    _tracksCache.Remove(changedTrack.Current);
                }
            })
            .AutoRefresh(track => track.Updates)
            .AutoRefresh(track => track.State)
            .Sort(SortExpressionComparer<TrackViewModel>.Ascending(track => track.Id))
            .ObserveOn(new DispatcherScheduler(Application.Current.Dispatcher))
            .Bind(out _tracks)
            .DisposeMany()
            .Subscribe((_) => Console.WriteLine(_tracksCache.Count));

        var generator = new GeneratorService();
        var generatorCleanup = generator.Tracks
            .Subscribe(trackDtos => _tracksCache.Edit(innerTracks =>
            {
                foreach (var trackDto in trackDtos)
{
                    if (trackDto.Time >= GetFutureToleranceTime() || trackDto.Time <= GetExpiredToleranceTime())
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
                        innerTracks.AddOrUpdate(new TrackViewModel(trackDto));
                    }
                }
            }));

        _cleanup = new CompositeDisposable(tracksSourceListCleanup, generatorCleanup);
    }

    private static DateTime GetFutureToleranceTime() => DateTime.Now + TimeSpan.FromSeconds(2);
    private static DateTime GetExpiredToleranceTime() => DateTime.Now - TimeSpan.FromSeconds(10);
    private static TimeSpan GetExpiredTimeInterval(TrackViewModel track) => track.Time - (DateTime.Now - TimeSpan.FromSeconds(10));

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
