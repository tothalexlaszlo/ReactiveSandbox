using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using ReactiveSandbox.Services;
using System.Reactive;
using System.Reactive.Disposables;

namespace ReactiveSandbox.ViewModels;

public class MainWindowViewModel : ReactiveObject, IDisposable
{
    private readonly CompositeDisposable _cleanup = new();
    private readonly ReadOnlyObservableCollection<TrackViewModel> _tracks;
    private bool _disposedValue;

    public ReadOnlyObservableCollection<TrackViewModel> InboundTracks => _tracks;

    public ReactiveCommand<Unit, int> CleanCommand { get; }

    public MainWindowViewModel(TrackService trackService)
    {
        _ = trackService.Connect()
            .AutoRefresh(track => track.Text)
            .Sort(SortExpressionComparer<TrackViewModel>.Ascending(track => track.Id))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _tracks)
            .DisposeMany()
            .Subscribe()
            .DisposeWith(_cleanup);

        CleanCommand = ReactiveCommand.Create(() => trackService.ClearTracks());
        _ = CleanCommand.Subscribe(observer => Console.WriteLine($"Clean command: {observer} item has been removed."))
            .DisposeWith(_cleanup);
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