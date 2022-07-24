using Splat;

namespace ReactiveSandbox.Wpf;

public sealed class Mode : IModeDetector
{
    private readonly bool _inUnitTestRunner;

    public static readonly Mode Run = new(false);
    public static readonly Mode Test = new(true);

    private Mode(bool inUnitTestRunner)
    {
        _inUnitTestRunner = inUnitTestRunner;
    }

    public bool? InUnitTestRunner() => _inUnitTestRunner;
}
