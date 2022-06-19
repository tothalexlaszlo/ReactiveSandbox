using System;

namespace ReactiveSandbox.Models;
internal readonly record struct TrackDto
{
    public int Id { get; }
    public DateTime Time { get; }

    public TrackDto(int id, DateTime time)
    {
        Id = id;
        Time = time;
    }
}
