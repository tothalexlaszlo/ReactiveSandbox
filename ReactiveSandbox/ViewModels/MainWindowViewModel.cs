using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using ReactiveSandbox.Services;

namespace ReactiveSandbox.ViewModels;

public class MainWindowViewModel : ReactiveObject, IDisposable
{
    private readonly IDisposable _cleanup;
    private readonly ReadOnlyObservableCollection<TrackViewModel> _tracks;
    private bool _disposedValue;

    public ReadOnlyObservableCollection<TrackViewModel> InboundTracks => _tracks;

    public MainWindowViewModel(TrackService trackService)
    {
        _cleanup = trackService
            .Connect()
            .AutoRefresh(track => track.Text)
            .Sort(SortExpressionComparer<TrackViewModel>.Ascending(track => track.Id))
            .DisposeMany()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _tracks)
            .Subscribe();
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
