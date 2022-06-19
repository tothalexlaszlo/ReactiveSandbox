using System.ComponentModel;
using System.Threading;
using System;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ReactiveSandbox.ViewModels;

internal class TrackViewModel : ReactiveObject, IDisposable
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

