namespace AprsSharp.AprsParser;

/// <summary>
/// Defines the standard Digipeater paths.
/// </summary>
public enum DigipeaterPaths
{
    /// <summary>
    /// -0.
    /// </summary>
    UseViaPath = 0,

    /// <summary>
    /// WIDE1-1
    /// -1.
    /// </summary>
    WIDE11 = 1,

    /// <summary>
    /// WIDE2-2
    /// -2.
    /// </summary>
    WIDE22 = 2,

    /// <summary>
    /// WIDE3-3
    /// -3.
    /// </summary>
    WIDE33 = 3,

    /// <summary>
    /// WIDE4-4
    /// -4.
    /// </summary>
    WIDE44 = 4,

    /// <summary>
    /// WIDE5-5
    /// -5.
    /// </summary>
    WIDE55 = 5,

    /// <summary>
    /// WIDE6-6
    /// -6.
    /// </summary>
    WIDE66 = 6,

    /// <summary>
    /// WIDE7-7
    /// -7.
    /// </summary>
    WIDE77 = 7,

    /// <summary>
    /// -8.
    /// </summary>
    NorthPath = 8,

    /// <summary>
    /// -9.
    /// </summary>
    SouthPath = 9,

    /// <summary>
    /// -10.
    /// </summary>
    EastPath = 10,

    /// <summary>
    /// -11.
    /// </summary>
    WestPath = 11,

    /// <summary>
    /// North + WIDE
    /// -12.
    /// </summary>
    NorthWIDE = 12,

    /// <summary>
    /// South + WIDE
    /// -13.
    /// </summary>
    SouthWIDE = 13,

    /// <summary>
    /// East + WIDE
    /// -14.
    /// </summary>
    EastWIDE = 14,

    /// <summary>
    /// West + WIDE
    /// -15.
    /// </summary>
    WestWIDE = 15,
}
