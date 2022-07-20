namespace ReactiveSandbox.Shared.Models;

public class AppOption
{
    public TimeSpan SendingInterval { get; set; } = TimeSpan.FromSeconds(0.2);
    public TimeSpan FutureToleranceTime { get; set; } = TimeSpan.FromSeconds(0.4);
    public TimeSpan InactiveToleranceTime { get; set; } = TimeSpan.FromSeconds(0.4);
    public TimeSpan ExpiredToleranceTime { get; set; } = TimeSpan.FromSeconds(2);
}
