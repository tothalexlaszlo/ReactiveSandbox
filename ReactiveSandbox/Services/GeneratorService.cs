using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.Extensions.Options;
using ReactiveSandbox.Models;
using ReactiveUI;

namespace ReactiveSandbox.Services;

public class GeneratorService
{
    private int _counter;
    private readonly Random _random = new();
    private readonly int _alwaysExpireCounterThreshold;

    public IObservable<IList<TrackDto>> Tracks { get; }

    public GeneratorService(IOptions<AppOption> option)
    {
        _alwaysExpireCounterThreshold = (int)(option.Value.ExpiredToleranceTime / option.Value.SendingInterval) + 1;
        Tracks = Observable.Interval(option.Value.SendingInterval, RxApp.TaskpoolScheduler).Select(_ => GenerateTracks());
    }

    private IList<TrackDto> GenerateTracks()
    {        
        var list =  new List<TrackDto>()
        {
            new TrackDto(id: 0, time: DateTime.Now - TimeSpan.FromSeconds(20)), // Always filtered
            new TrackDto(id: 1, time: DateTime.Now + TimeSpan.FromSeconds(20)), // Always filtered
            new TrackDto(id: 2, time: DateTime.Now),                            // Always present
            new TrackDto(id: _random.Next(3, 23), time: DateTime.Now),          // Randomly expired
            new TrackDto(id: _random.Next(3, 23), time: DateTime.Now),          // Randomly expired
            new TrackDto(id: _random.Next(3, 23), time: DateTime.Now),          // Randomly expired
            new TrackDto(id: _random.Next(3, 23), time: DateTime.Now),          // Randomly expired
            new TrackDto(id: _random.Next(3, 23), time: DateTime.Now),           // Randomly expired
        };

        if (_counter == _alwaysExpireCounterThreshold)
        {
            list.Add(new TrackDto(id: 30, time: DateTime.Now));           // Always expire
            _counter = 0;
        }

        _counter++;
        return list;
    }
}
