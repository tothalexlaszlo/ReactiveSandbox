using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using ReactiveSandbox.Models;

namespace ReactiveSandbox.Services;

internal class GeneratorService
{
    private readonly Random _random = new();

    public IObservable<IList<TrackDto>> Tracks { get; }

    public GeneratorService() => Tracks = Observable.Interval(TimeSpan.FromSeconds(1)).Select(_ => GenerateTracks());

    private IList<TrackDto> GenerateTracks()
    {
        return new List<TrackDto>()
        {
            new TrackDto(id: 0, time: DateTime.Now - TimeSpan.FromSeconds(20)), // Always filtered
            new TrackDto(id: 1, time: DateTime.Now + TimeSpan.FromSeconds(20)), // Always filtered
            new TrackDto(id: 2, time: DateTime.Now),                            // Always present
            new TrackDto(id: _random.Next(3, 23), time: DateTime.Now),          // Randomly expired
            new TrackDto(id: _random.Next(3, 23), time: DateTime.Now),          // Randomly expired
            new TrackDto(id: _random.Next(3, 23), time: DateTime.Now),          // Randomly expired
            new TrackDto(id: _random.Next(3, 23), time: DateTime.Now),          // Randomly expired
            new TrackDto(id: _random.Next(3, 23), time: DateTime.Now)           // Randomly expired
        };
    }
}
