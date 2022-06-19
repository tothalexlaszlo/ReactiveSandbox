using System;

namespace ReactiveSandbox.Models;
internal readonly record struct TrackDto
{
    public int Id { get; init; }
    public DateTime Time { get; init; }

    public TrackDto(int id, DateTime time)
    {
        Id = id;
        Time = time;
    }
}
