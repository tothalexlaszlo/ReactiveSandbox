using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using ReactiveSandbox.Services;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Threading;

namespace ReactiveSandbox.ViewModels;

public class MainWindowViewModel : ReactiveObject, IDisposable
{
    private readonly CompositeDisposable _cleanup = new();
    private bool _disposedValue;

    private readonly ReadOnlyObservableCollection<TrackViewModel> _tracks;
    public ReadOnlyObservableCollection<TrackViewModel> InboundTracks => _tracks;

    public ReactiveCommand<Unit, int> CleanCommand { get; }
    public ReactiveCommand<Unit, Unit> BuggyCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelBuggyExecutionCommand { get; }

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
        _ = CleanCommand.Subscribe(count => Console.WriteLine($"Clean command: {count} items has been removed.")).DisposeWith(_cleanup);

        BuggyCommand = ReactiveCommand.CreateFromObservable(() => Observable.StartAsync(cancellationToken => WaitAndThrowException(cancellationToken))
            .TakeUntil(CancelBuggyExecutionCommand));
        _ = BuggyCommand.ThrownExceptions.Subscribe(exception => Console.WriteLine(exception)).DisposeWith(_cleanup);

        CancelBuggyExecutionCommand = ReactiveCommand.Create(() => { }, BuggyCommand.IsExecuting);
    }

    private static async Task WaitAndThrowException(CancellationToken cancellationToken)
{
        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        throw new Exception();
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