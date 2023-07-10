namespace AprsSharp.Parsers.Aprs;

/// <summary>
/// Represents an info field that is not supported by APRS#.
/// </summary>
public class UnsupportedInfo : InfoField
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnsupportedInfo"/> class.
    /// </summary>
    /// <param name="encodedInfoField">A string encoding of an <see cref="InfoField"/>.</param>
    public UnsupportedInfo(string encodedInfoField)
        : base(encodedInfoField)
    {
        Content = encodedInfoField;
    }

    /// <summary>
    /// Gets the <see cref="InfoField"/> content.
    /// </summary>
    public string Content { get; }

    /// <inheritdoc/>
    public override string Encode() => Content;
}
