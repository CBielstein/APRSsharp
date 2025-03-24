namespace AprsSharpUnitTests.AprsIsClient
{
    using Xunit;

    /// <summary>
    /// Test collection to disable test parallelism, which
    /// is required for set timeouts on tests.
    /// </summary>
    [CollectionDefinition(nameof(TimedTests), DisableParallelization = true)]
    public class TimedTests
    {
    }
}
