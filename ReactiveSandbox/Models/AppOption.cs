using System;

namespace ReactiveSandbox.Models;

public class AppOption
{
    public TimeSpan SendingInterval { get; set; } = TimeSpan.FromMilliseconds(100);
    public TimeSpan FutureToleranceTime { get; set; } = TimeSpan.FromMilliseconds(200);
    public TimeSpan InactiveToleranceTime { get; set; } = TimeSpan.FromMilliseconds(200);
    public TimeSpan ExpiredToleranceTime { get; set; } = TimeSpan.FromMilliseconds(1000);
}
