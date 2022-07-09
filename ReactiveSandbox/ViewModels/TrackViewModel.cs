using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReactiveSandbox.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ReactiveSandbox.ViewModels;

public partial class TrackViewModel : ReactiveObject, IEquatable<TrackViewModel>, IDisposable
{
    private readonly IOptions<AppOption> _option;
    private readonly ILogger<TrackViewModel> _logger;
    private readonly CompositeDisposable _cleanup = new();

    private bool _disposedValue;
    private IDisposable _stateManagerCleanup = Disposable.Empty;
    private DateTime _time;
    private readonly ObservableAsPropertyHelper<string> _text;

    public int Id { get; }

    public DateTime Time
    {
        get => _time;
        private set => this.RaiseAndSetIfChanged(ref _time, value);
    }

    [Reactive]
    public State State { get; private set; }

    [Reactive]
    public int Updates { get; private set; }

    public string Text => _text.Value;

    public TrackViewModel(in TrackDto trackDto, IOptions<AppOption> option, ILogger<TrackViewModel> logger)
    {
        _option = option ?? throw new ArgumentNullException(nameof(option));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _ = this.WhenAnyValue(track => track.State)
            .Subscribe(_ => UpdateStateTimer())
            .DisposeWith(_cleanup);

        _text = this.WhenAnyValue(track => track.State, track => track.Updates)
            .Throttle(TimeSpan.FromMilliseconds(100))
            .Select(_ => ToString())
            .ToProperty(this, nameof(Text))
            .DisposeWith(_cleanup);

        Id = trackDto.Id;
        Updates = -1;

        Update(trackDto);
    }

    #region Logging source generation
    [LoggerMessage(0, LogLevel.Information, "Track {Id} has been disposed.")]
    partial void LogDisposed(int id);
    #endregion

    public void Update(in TrackDto trackDto)
    {
        Time = trackDto.Time;
        Updates++;

        // Maybe need some refactoring
        UpdateStateTimer();
    }

    private void UpdateStateTimer()
    {
        _stateManagerCleanup.Dispose();

        RefreshState();

        switch (State)
        {
            case State.Active:
                _stateManagerCleanup = Observable.Timer(_option.Value.InactiveToleranceTime).Subscribe(_ => State = State.Inactive);
                return;
            case State.Inactive:
                _stateManagerCleanup = Observable.Timer(_option.Value.ExpiredToleranceTime - _option.Value.InactiveToleranceTime).Subscribe(_ => State = State.Expired);
                return;
            case State.Expired:
            default:
                return;
        }
    }

    private void RefreshState()
    {
        if (Time > DateTime.Now - _option.Value.InactiveToleranceTime)
        {
            State = State.Active;
            return;
        }

        State = Time <= DateTime.Now - _option.Value.ExpiredToleranceTime ? State.Expired : State.Inactive;
    }

    public override string ToString() => $"Id: {Id} - Time: {Time} - Updates: {Updates} - State: {State}";

    #region IEquatable
    public override bool Equals(object? obj) => Equals(obj as TrackViewModel);

    public bool Equals(TrackViewModel? other)
    {
        if (other is null)
        {
            return false;
        }

        // Optimization for a common success case.
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        // If run-time types are not exactly the same, return false.
        if (GetType() != other.GetType())
        {
            return false;
        }

        // Return true if the fields match.
        return Id == other.Id
            && Time == other.Time;
    }

    public override int GetHashCode() => HashCode.Combine(Id, Time);

    public static bool operator ==(TrackViewModel lhs, TrackViewModel rhs) => lhs is null ? rhs is null : lhs.Equals(rhs);

    public static bool operator !=(TrackViewModel lhs, TrackViewModel rhs) => !(lhs == rhs);
    #endregion

    #region IDisposable
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _stateManagerCleanup.Dispose();
                _cleanup.Dispose();

                LogDisposed(Id);
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
