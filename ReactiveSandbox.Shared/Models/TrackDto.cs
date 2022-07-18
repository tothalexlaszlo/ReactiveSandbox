using System;

namespace ReactiveSandbox.Shared.Models;
public readonly record struct TrackDto
{
    public int Id { get; }
    public DateTime Time { get; }

    public TrackDto(int id, in DateTime time)
    {
        Id = id;
        Time = time;
    }
}
