using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using ReactiveUI.Validation.Helpers;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using System.Text.RegularExpressions;
using ReactiveSandbox.Shared.Services;

namespace ReactiveSandbox.Shared.ViewModels;

public class MainWindowViewModel : ReactiveValidationObject, IDisposable
{
    private readonly CompositeDisposable _cleanup = new();
    private readonly ReadOnlyObservableCollection<TrackViewModel> _tracks;
    private readonly Regex _emailRegex = new("^((\"[\\w-\\s]+\")|([\\w-]+(?:\\.[\\w-]+)*)|(\"[\\w-\\s]+\")([\\w-]+(?:\\.[\\w-]+)*))(@((?:[\\w-]+\\.)*\\w[\\w-]{0,66})\\.([a-z]{2,6}(?:\\.[a-z]{2})?)$)|(@\\[?((25[0-5]\\.|2[0-4][0-9]\\.|1[0-9]{2}\\.|[0-9]{1,2}\\.))((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\\]?$)", RegexOptions.Compiled);
    private readonly ObservableAsPropertyHelper<bool> _canSubmit;

    private bool _disposedValue;

    [Reactive]
    public string Email { get; set; } = string.Empty;

    public bool CanSubmit => _canSubmit.Value;
    public ReadOnlyObservableCollection<TrackViewModel> InboundTracks => _tracks;
    public ValidationHelper EmailValidation { get; }
    public ReactiveCommand<Unit, int> CleanCommand { get; }
    public ReactiveCommand<Unit, Unit> BuggyCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelBuggyExecutionCommand { get; }
    public ReactiveCommand<Unit, Unit> SubmitCommand { get; }

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

        EmailValidation = this.ValidationRule(
            viewModel => viewModel.Email,
            email => _emailRegex.IsMatch(email),
            "Email is not in valid.");

        CleanCommand = ReactiveCommand.Create(() => trackService.ClearTracks());
        _ = CleanCommand.Subscribe(count => Console.WriteLine($"Clean command: {count} items has been removed.")).DisposeWith(_cleanup);

        BuggyCommand = ReactiveCommand.CreateFromObservable(() => Observable.StartAsync(cancellationToken => WaitAndThrowException(cancellationToken))
            .TakeUntil(CancelBuggyExecutionCommand));
        _ = BuggyCommand.ThrownExceptions.Subscribe(exception => Console.WriteLine(exception)).DisposeWith(_cleanup);

        CancelBuggyExecutionCommand = ReactiveCommand.Create(() => { }, BuggyCommand.IsExecuting);

        SubmitCommand = ReactiveCommand.Create(() => { Console.WriteLine($"{Email} was submitted."); }, this.IsValid());

        _canSubmit = SubmitCommand.CanExecute
            .ToProperty(this, x => x.CanSubmit); 
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