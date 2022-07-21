namespace ReactiveSandbox.Shared.Models;

public class AppOption
{
    public TimeSpan SendingInterval { get; set; } = TimeSpan.FromSeconds(1);
    public TimeSpan FutureToleranceTime { get; set; } = TimeSpan.FromSeconds(2);
    public TimeSpan InactiveToleranceTime { get; set; } = TimeSpan.FromSeconds(2);
    public TimeSpan ExpiredToleranceTime { get; set; } = TimeSpan.FromSeconds(10);
}
