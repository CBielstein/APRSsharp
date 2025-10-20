namespace AprsSharp.AprsParser;

/// <summary>
/// Defines which geographical quadrant a Mic-E message is referencing.
/// </summary>
public enum GeoQuadrant
{
    /// <summary>
    /// Northwest.
    /// </summary>
    Northwest,

    /// <summary>
    /// Southwest.
    /// </summary>
    Southwest,

    /// <summary>
    /// Northeast.
    /// </summary>
    Northeast,

    /// <summary>
    /// Returning.
    /// </summary>
    Returning,

    /// <summary>
    /// Committed.
    /// </summary>
    Committed,

    /// <summary>
    /// Special.
    /// </summary>
    Special,

    /// <summary>
    /// Priority.
    /// </summary>
    Priority,

    /// <summary>
    /// Emergency.
    /// </summary>
    Emergency,

    /// <summary>
    /// Custom0
    /// </summary>
    Custom0,

    /// <summary>
    /// Custom1
    /// </summary>
    Custom1,

    /// <summary>
    /// Custom2
    /// </summary>
    Custom2,

    /// <summary>
    /// Custom3
    /// </summary>
    Custom3,

    /// <summary>
    /// Custom4
    /// </summary>
    Custom4,

    /// <summary>
    /// Custom5
    /// </summary>
    Custom5,

    /// <summary>
    /// Custom6
    /// </summary>
    Custom6,
}
