using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveSandbox.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ReactiveSandbox.ViewModels;

internal class TrackViewModel : ReactiveObject, IEquatable<TrackViewModel>, IDisposable
{
    private bool _disposedValue;
    private IDisposable _stateManagerCleanup = Disposable.Empty;

    public int Id { get; }

    [Reactive]
    public DateTime Time { get; private set; }

    [Reactive]
    public State State { get; private set; }

    [Reactive]
    public int Updates { get; private set; }

    public TrackViewModel(in TrackDto trackDto)
    {
        Id = trackDto.Id;
        Updates = -1;

        Update(trackDto);
    }

    public void Update(in TrackDto trackDto)
    {
        _stateManagerCleanup.Dispose();
        Time = trackDto.Time;
        Updates++;

        RefreshState();

        if (State is State.Active)
        {
            _stateManagerCleanup = Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ => State = State.Inactive);
        }
        else if (State is State.Inactive)
        {
            _stateManagerCleanup = Observable.Timer(TimeSpan.FromSeconds(8)).Subscribe(_ => State = State.Expired);
        }
    }

    private void RefreshState()
    {
        if (Time > DateTime.Now - TimeSpan.FromSeconds(2))
        {
            State = State.Active;
            return;
        }

        State = Time <= DateTime.Now - TimeSpan.FromSeconds(10) ? State.Expired : State.Inactive;
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

    public override int GetHashCode() => Id.GetHashCode();

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
                Console.WriteLine($"Track {Id} is disposed.");
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

