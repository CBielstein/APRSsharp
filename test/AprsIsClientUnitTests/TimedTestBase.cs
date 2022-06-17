namespace AprsSharpUnitTests.Connections.AprsIs
{
    using System;
    using System.Threading.Tasks;
    using AprsSharp.Connections.AprsIs;
    using Xunit;

    /// <summary>
    /// A base class for common functionality across tests
    /// of <see cref="AprsIsClient"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP007", Justification = "Test code with dispose for convenience.")]
    [Collection(nameof(TimedTestCollection))]
    public class TimedTestBase : IDisposable
    {
        /// <summary>
        /// The <see cref="AprsIsClient"/> for testing.
        /// Reference via <see cref="AprsIs"/> to get the
        /// free dispose on assignment.
        /// </summary>
        private AprsIsClient? aprsIsClient;

        /// <summary>
        /// Gets or sets <see cref="AprsIsClient"/> for testing.
        /// </summary>
        protected AprsIsClient? Client
        {
            get => aprsIsClient;
            set
            {
                aprsIsClient?.Dispose();
                aprsIsClient = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="TaskCompletionSource"/> to signal and
        /// wait on a specific condition to be met in a test.
        /// </summary>
        protected TaskCompletionSource ConditionMet { get; set; } = new TaskCompletionSource();

        /// <inheritdoc/>
        public virtual void Dispose()
        {
            aprsIsClient?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}