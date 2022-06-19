using System;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ReactiveSandbox.ViewModels;

internal class TrackViewModel : ReactiveObject, IEquatable<TrackViewModel>, IDisposable
{
    private bool _disposedValue;

    public int Id { get; init; }

    [Reactive]
    public DateTime Time { get; set; }

    [Reactive]
    public int Updates { get; set; }

    public TrackViewModel(int id)
    {
        Id = id;
    }

    public override string ToString() => $"Id: {Id} - Time: {Time} - Updates: {Updates}";

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

