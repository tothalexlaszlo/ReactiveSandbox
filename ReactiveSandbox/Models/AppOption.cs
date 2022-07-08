using System;

namespace ReactiveSandbox.Models;

public class AppOption
{
    public TimeSpan SendingInterval { get; set; } = TimeSpan.FromSeconds(0.5);
    public TimeSpan FutureToleranceTime { get; set; } = TimeSpan.FromSeconds(1);
    public TimeSpan InactiveToleranceTime { get; set; } = TimeSpan.FromSeconds(1);
    public TimeSpan ExpiredToleranceTime { get; set; } = TimeSpan.FromSeconds(5);
}
