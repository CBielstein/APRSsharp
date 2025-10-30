namespace AprsSharp.AprsParser;

/// <summary>
/// Defines various messages encoded in a Mic-E packet.
/// </summary>
public enum MicEMessageType
{
    /// <summary>
    /// Off Duty.
    /// </summary>
    OffDuty,

    /// <summary>
    /// En Route.
    /// </summary>
    EnRoute,

    /// <summary>
    /// In Service.
    /// </summary>
    InService,

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

    /// <summary>
    /// Not one of the known encodings.
    /// Used for a mix of standard and custom indicators.
    /// </summary>
    Unknown,
}
