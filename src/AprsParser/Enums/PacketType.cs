namespace AprsSharp.Parsers.Aprs
{
    /// <summary>
    /// The APRS packet type.
    /// </summary>
    public enum PacketType
    {
        /// <summary>
        /// Current Mic-E Data (Rev 0 beta)
        /// </summary>
        CurrentMicEData,

        /// <summary>
        /// Old Mic-E Data (Rev 0 beta)
        /// </summary>
        OldMicEData,

        /// <summary>
        /// Position without timestamp (no APRS messaging), or Ultimeter 2000 WX Station
        /// </summary>
        PositionWithoutTimestampNoMessaging,

        /// <summary>
        /// Peet Bros U-II Weather Station
        /// </summary>
        PeetBrosUIIWeatherStation,

        /// <summary>
        /// Raw GPS data or Ultimeter 2000
        /// </summary>
        RawGPSData,

        /// <summary>
        /// Agrelo DFJr / MicroFinder
        /// </summary>
        AgreloDFJrMicroFinder,

        /// <summary>
        /// [Reserved - Map Feature]
        /// </summary>
        MapFeature,

        /// <summary>
        /// Old Mic-E Data (but Current data for TM-D700)
        /// </summary>
        OldMicEDataCurrentTMD700,

        /// <summary>
        /// Item
        /// </summary>
        Item,

        /// <summary>
        /// [Reserved - shelter data with time]
        /// </summary>
        ShelterDataWithTime,

        /// <summary>
        /// Invalid data or test data
        /// </summary>
        InvalidOrTestData,

        /// <summary>
        /// [Reserved - Space Weather]
        /// </summary>
        SpaceWeather,

        /// <summary>
        /// Unused
        /// </summary>
        Unused,

        /// <summary>
        /// Position with timestamp (no APRS messaging)
        /// </summary>
        PositionWithTimestampNoMessaging,

        /// <summary>
        /// Message
        /// </summary>
        Message,

        /// <summary>
        /// Object
        /// </summary>
        [field:System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720", Justification = "APRS spec uses the word 'object', so it is appropriate here")]
        Object,

        /// <summary>
        /// Station Capabilities
        /// </summary>
        StationCapabilities,

        /// <summary>
        /// Position without timestamp (with APRS messaging)
        /// </summary>
        PositionWithoutTimestampWithMessaging,

        /// <summary>
        /// Status
        /// </summary>
        Status,

        /// <summary>
        /// Query
        /// </summary>
        Query,

        /// <summary>
        /// [Do not use]
        /// </summary>
        DoNotUse,

        /// <summary>
        /// Positionwith timestamp (with APRS messaging)
        /// </summary>
        PositionWithTimestampWithMessaging,

        /// <summary>
        /// Telemetry data
        /// </summary>
        TelemetryData,

        /// <summary>
        /// Maidenhead grid locator beacon (obsolete)
        /// </summary>
        MaidenheadGridLocatorBeacon,

        /// <summary>
        /// Weather Report (without position)
        /// </summary>
        WeatherReport,

        /// <summary>
        /// Current Mic-E Data (not used in TM-D700)
        /// </summary>
        CurrentMicEDataNotTMD700,

        /// <summary>
        /// User-Defined APRS packet format
        /// </summary>
        UserDefinedAPRSPacketFormat,

        /// <summary>
        /// [Do not use - TNC stream switch character]
        /// </summary>
        DoNotUseTNSStreamSwitchCharacter,

        /// <summary>
        /// Third-party traffic
        /// </summary>
        ThirdPartyTraffic,

        /// <summary>
        /// Not a recognized symbol
        /// </summary>
        Unknown,
    }
}
