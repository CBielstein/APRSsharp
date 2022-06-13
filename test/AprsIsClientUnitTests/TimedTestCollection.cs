namespace AprsSharpUnitTests
{
    using Xunit;

    /// <summary>
    /// Test collection to disable test parallelism, which
    /// is required for set timeouts on tests.
    /// </summary>
    [CollectionDefinition(nameof(TimedTestCollection), DisableParallelization = true)]
    public class TimedTestCollection
    {
    }
}
