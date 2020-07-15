
using System;
using Xunit;
using APaRSer;
using GeoCoordinatePortable;

namespace APaRSerUnitTests
{
    public class PacketUnitTest
    {
        //
        // NOTE: Many of these are testing incomplete functionality.
        // Any catch of System.NotImplementedException should be considered for removal in the future.
        //

        /// <summary>
        /// Dcodes a positionless weather report based on the example given in the APRS spec
        /// </summary>
        [Fact]
        public void DecodeInformationFieldFromSpecExample_PositionlessWeatherReportFormat()
        {
            Packet p = new Packet();
      
            try
            {
                p.DecodeInformationField ("_10090556c220s004g005t077r000p000P000h50b09900wRSW");

                throw new System.Exception("Update test. It's now outdated as more functionality has been added.");
            }
            catch (System.NotImplementedException)
            {

                Assert.Equal(Packet.Type.WeatherReport, p.DecodedType);
                Assert.Equal(false, p.HasMessaging);

                Timestamp ts = (Timestamp)p.timestamp;
                Assert.Equal(Timestamp.Type.MDHM, ts.DecodedType);
                Assert.Equal(10, ts.dateTime.Month);
                Assert.Equal(9, ts.dateTime.Day);
                Assert.Equal(05, ts.dateTime.Hour);
                Assert.Equal(56, ts.dateTime.Minute);
            }
        }

        /// <summary>
        /// Decodes a lat/long position report format with timestamp, no APRS messaging, zulu time, with comment
        /// based on the example given in the APRS spec
        /// </summary>
        [Fact]
        public void DecodeInformationFieldFromSpecExample_LatLongPositionReportFormatWithTimestamp_1()
        {
            Packet p = new Packet();
           
            p.DecodeInformationField("/092345z4903.50N/07201.75W>Test1234");
          
            Assert.Equal(Packet.Type.PositionWithTimestampNoMessaging, p.DecodedType);
            Assert.Equal(false, p.HasMessaging);

            Timestamp ts = (Timestamp)p.timestamp;
            Assert.Equal(Timestamp.Type.DHMz, ts.DecodedType);
            Assert.Equal(9, ts.dateTime.Day);
            Assert.Equal(23, ts.dateTime.Hour);
            Assert.Equal(45, ts.dateTime.Minute);

            Position pos = (Position)p.position;
            Assert.Equal(new GeoCoordinate(49.0583, -72.0292), pos.Coordinates);
            Assert.Equal('/', pos.SymbolTableIdentifier);
            Assert.Equal('>', pos.SymbolCode);

            Assert.Equal("Test1234", p.comment);
        }

        /// <summary>
        /// Decodes a lat/long position report format with timestamp, no APRS messaging, zulu time, with comment
        /// based on the example given in the APRS spec
        /// </summary>
        [Fact]
        public void EncodeInformationFieldFromSpecExample_LatLongPositionReportFormatWithTimestamp_1()
        {
            Packet p = new Packet();

            DateTime dt = new DateTime(2016, 12, 9, 23, 45, 0, 0, DateTimeKind.Utc);
            p.timestamp = new Timestamp(dt);

            p.HasMessaging = false;

            GeoCoordinate gc = new GeoCoordinate(49.0583, -72.0292);
            p.position = new Position(gc, '/', '>', 0);

            p.comment = "Test1234";
          
            string encoded = p.EncodeInformationField(Packet.Type.PositionWithTimestampNoMessaging);

            Assert.Equal("/092345z4903.50N/07201.75W>Test1234", encoded);
        }

        /// <summary>
        /// Dcodes a lat/long position report format with timestamp, with APRS messaging, local time, with comment
        /// based on the example given in the APRS spec
        /// </summary>
        [Fact]
        public void DecodeInformationFieldFromSpecExample_LatLongPositionReportFormatWithTimestamp_2()
        {
            Packet p = new Packet();
     
            p.DecodeInformationField ("@092345/4903.50N/07201.75W>Test1234");

            Assert.Equal(Packet.Type.PositionWithTimestampWithMessaging, p.DecodedType);
            Assert.Equal(true, p.HasMessaging);

            Timestamp ts = (Timestamp)p.timestamp;
            Assert.Equal(Timestamp.Type.DHMl, ts.DecodedType);
            Assert.Equal(9, ts.dateTime.Day);
            Assert.Equal(23, ts.dateTime.Hour);
            Assert.Equal(45, ts.dateTime.Minute);

            Position pos = (Position)p.position;
            Assert.Equal(new GeoCoordinate(49.0583, -72.0292), pos.Coordinates);
            Assert.Equal('/', pos.SymbolTableIdentifier);
            Assert.Equal('>', pos.SymbolCode);

            Assert.Equal("Test1234", p.comment);
        }

        /// <summary>
        /// Dcodes a lat/long position report format with timestamp, with APRS messaging, local time, with comment
        /// based on the example given in the APRS spec
        /// </summary>
        [Fact]
        public void EncodeInformationFieldFromSpecExample_LatLongPositionReportFormatWithTimestamp_2()
        {
            Packet p = new Packet();

            DateTime dt = new DateTime(2016, 12, 9, 23, 45, 00, DateTimeKind.Local);
            p.timestamp = new Timestamp(dt);

            p.HasMessaging = true;

            p.position = new Position(new GeoCoordinate(49.0583, -72.0292), '/', '>', 0);

            p.comment = "Test1234";

            string encoded = p.EncodeInformationField(Packet.Type.PositionWithTimestampWithMessaging, Timestamp.Type.DHMl);
                      
            Assert.Equal("@092345/4903.50N/07201.75W>Test1234", encoded);
        }

        /// <summary>
        /// Lat/Long Position Report Format — with Data Extension and Timestamp 
        /// with timestamp, with APRS messaging, local time, course/speed.
        /// </summary>
        [Fact (Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeInformationFieldFromSpecExample_LatLongPositionReportFormatWithDataExtensionAndTimestamp_1()
        {
            Packet p = new Packet();
    
            p.DecodeInformationField("@092345/4903.50N/07201.75W>088/036");

            Assert.Equal(Packet.Type.PositionWithTimestampWithMessaging, p.DecodedType);
            Assert.Equal(true, p.HasMessaging);

            Timestamp ts = (Timestamp)p.timestamp;
            Assert.Equal(Timestamp.Type.DHMl, ts.DecodedType);
            Assert.Equal(9, ts.dateTime.Day);
            Assert.Equal(23, ts.dateTime.Hour);
            Assert.Equal(45, ts.dateTime.Minute);

            Assert.True(false, "Not yet handling data extension.");
        }

        /// <summary>
        /// Lat/Long Position Report Format — with Data Extension and Timestamp 
        /// with timestamp, APRS messaging, hours/mins/secs time, PHG
        /// </summary>
        [Fact (Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeInformationFieldFromSpecExample_LatLongPositionReportFormatWithDataExtensionAndTimestamp_2()
        {
            Packet p = new Packet();
      
            p.DecodeInformationField("@234517h4903.50N/07201.75W>PHG5132");

            Assert.Equal(Packet.Type.PositionWithTimestampWithMessaging, p.DecodedType);
            Assert.Equal(true, p.HasMessaging);

            Timestamp ts = (Timestamp)p.timestamp;
            Assert.Equal(Timestamp.Type.HMS, ts.DecodedType);
            Assert.Equal(23, ts.dateTime.Hour);
            Assert.Equal(45, ts.dateTime.Minute);
            Assert.Equal(17, ts.dateTime.Second);

            Assert.True(false, "Not yet handling data extension.");
        }

        /// <summary>
        /// Lat/Long Position Report Format — with Data Extension and Timestamp 
        /// with timestamp, APRS messaging, zulu time, radio range.
        /// </summary>
        [Fact (Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeInformationFieldFromSpecExample_LatLongPositionReportFormatWithDataExtensionAndTimestamp_3()
        {
            Packet p = new Packet();
          
            p.DecodeInformationField("@092345z4903.50N/07201.75W>RNG0050");

            Assert.Equal(Packet.Type.PositionWithTimestampWithMessaging, p.DecodedType);
            Assert.Equal(true, p.HasMessaging);

            Timestamp ts = (Timestamp)p.timestamp;
            Assert.Equal(Timestamp.Type.DHMz, ts.DecodedType);
            Assert.Equal(09, ts.dateTime.Day);
            Assert.Equal(23, ts.dateTime.Hour);
            Assert.Equal(45, ts.dateTime.Minute);

            Assert.True(false, "Not yet handling data extensions.");
       
           
        }

              
        /// <summary>
        /// Lat/Long Position Report Format — with Data Extension and Timestamp 
        /// with timestamp, hours/mins/secs time, DF, no APRS messaging
        /// </summary>
        [Fact (Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeInformationFieldFromSpecExample_LatLongPositionReportFormatWithDataExtensionAndTimestamp_4()
        {
            Packet p = new Packet();
       
            p.DecodeInformationField("/234517h4903.50N/07201.75W>DFS2360");

            Assert.Equal(Packet.Type.PositionWithTimestampNoMessaging, p.DecodedType);
            Assert.Equal(false, p.HasMessaging);

            Timestamp ts = (Timestamp)p.timestamp;
            Assert.Equal(Timestamp.Type.HMS, ts.DecodedType);
            Assert.Equal(23, ts.dateTime.Hour);
            Assert.Equal(45, ts.dateTime.Minute);
            Assert.Equal(17, ts.dateTime.Second);

            Position pos = (Position)p.position;
            Assert.Equal(new GeoCoordinate(49.0583, -72.0292), pos.Coordinates);
            Assert.Equal('/', pos.SymbolTableIdentifier);
            Assert.Equal('>', pos.SymbolCode);

            Assert.True(false, "Not yet handling DF data.");
        }

        /// <summary>
        /// Lat/Long Position Report Format — with Data Extension and Timestamp 
        /// weather report
        /// </summary>
        [Fact (Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeInformationFieldFromSpecExample_LatLongPositionReportFormatWithDataExtensionAndTimestamp_5()
        {
            Packet p = new Packet();
   
            p.DecodeInformationField("@092345z4903.50N/07201.75W_090/000g000t066r000p000…dUII");

            Assert.Equal(Packet.Type.WeatherReport, p.DecodedType);
            Assert.Equal(false, p.HasMessaging);

            Timestamp ts = (Timestamp)p.timestamp;
            Assert.Equal(Timestamp.Type.DHMz, ts.DecodedType);
            Assert.Equal(09, ts.dateTime.Day);
            Assert.Equal(23, ts.dateTime.Hour);
            Assert.Equal(45, ts.dateTime.Minute);

            Assert.True(false, "Not yet handling weather reports");
        }

        /// <summary>
        ///  DF Report Format — with Timestamp
        ///  with timestamp, course/speed/bearing/NRQ, with APRS messaging. 
        /// </summary>
        [Fact (Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeInformationFieldFromSpecExample_DFReportFormat_1()
        {
            Packet p = new Packet();
   
            p.DecodeInformationField("@092345z4903.50N/07201.75W\088/036/270/729");

            Assert.Equal(Packet.Type.PositionWithTimestampWithMessaging, p.DecodedType);
            Assert.Equal(true, p.HasMessaging);

            Timestamp ts = (Timestamp)p.timestamp;
            Assert.Equal(Timestamp.Type.DHMz, ts.DecodedType);
            Assert.Equal(09, ts.dateTime.Day);
            Assert.Equal(23, ts.dateTime.Hour);
            Assert.Equal(45, ts.dateTime.Minute);

            Assert.True(false, "Not yet handling DF Report.");
        }

        /// <summary>
        ///  DF Report Format — with Timestamp
        ///   with timestamp, bearing/NRQ, no course/speed, no APRS messaging.
        /// </summary>
        [Fact (Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeInformationFieldFromSpecExample_DFReportFormat_2()
        {
            Packet p = new Packet();
      
            p.DecodeInformationField(@"/092345z4903.50N/07201.75W\000/000/270/729");

            Assert.Equal(Packet.Type.PositionWithTimestampNoMessaging, p.DecodedType);
            Assert.Equal(false, p.HasMessaging);

            Timestamp ts = (Timestamp)p.timestamp;
            Assert.Equal(Timestamp.Type.DHMz, ts.DecodedType);
            Assert.Equal(09, ts.dateTime.Day);
            Assert.Equal(23, ts.dateTime.Hour);
            Assert.Equal(45, ts.dateTime.Minute);

            Position pos = (Position)p.position;
            Assert.Equal(new GeoCoordinate(49.0583, -72.0292), pos.Coordinates);
            Assert.Equal('/', pos.SymbolTableIdentifier);
            Assert.Equal('\\', pos.SymbolCode);

            Assert.True(false, "Not yet handling bearing, course/speed.");
        }

        /// <summary>
        ///  Compressed Lat/Long Position Report Format — with Timestamp
        ///  with APRS messaging, timestamp, radio range
        /// </summary>
        [Fact (Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeInformationFieldFromSpecExample_CompressedLatLongPositionReportFormat()
        {
            Packet p = new Packet();
    
            p.DecodeInformationField("@092345z/5L!!<*e7>{?!");

            Assert.Equal(Packet.Type.PositionWithTimestampWithMessaging, p.DecodedType);
            Assert.Equal(true, p.HasMessaging);

            Timestamp ts = (Timestamp)p.timestamp;
            Assert.Equal(Timestamp.Type.DHMz, ts.DecodedType);
            Assert.Equal(09, ts.dateTime.Day);
            Assert.Equal(23, ts.dateTime.Hour);
            Assert.Equal(45, ts.dateTime.Minute);

            Assert.True(false, "Not yet handling compressed latlong position report format.");
        }

        /// <summary>
        ///  Complete Weather Report Format — with Lat/Long position and Timestamp
        /// </summary>
        [Fact (Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeInformationFieldFromSpecExample_CompleteWeatherReportFormatwithLatLongPositionAndTimestamp()
        {
            Packet p = new Packet();
  
            p.DecodeInformationField("@092345z4903.50N/07201.75W_220/004g005t-07r000p000P000h50b09900wRSW");

            Assert.Equal(Packet.Type.PositionWithTimestampWithMessaging, p.DecodedType);
            Assert.Equal(true, p.HasMessaging);

            Timestamp ts = (Timestamp)p.timestamp;
            Assert.Equal(Timestamp.Type.DHMz, ts.DecodedType);
            Assert.Equal(09, ts.dateTime.Day);
            Assert.Equal(23, ts.dateTime.Hour);
            Assert.Equal(45, ts.dateTime.Minute);

            Assert.True(false, "Not yet handling weather information.");
        }

        /// <summary>
        ///  Complete Weather Report Format — with Compressed Lat/Long position, with Timestamp
        /// </summary>
        [Fact (Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeInformationFieldFromSpecExample_CompleteWeatherReportFormatWithCompressedLatLongPositionWithTimestamp()
        {
            Packet p = new Packet();
 
            p.DecodeInformationField("@092345z/5L!!<*e7 _7P[g005t077r000p000P000h50b09900wRSW");

            Assert.Equal(Packet.Type.PositionWithTimestampWithMessaging, p.DecodedType);
            Assert.Equal(false, p.HasMessaging);

            Timestamp ts = (Timestamp)p.timestamp;
            Assert.Equal(Timestamp.Type.DHMz, ts.DecodedType);
            Assert.Equal(09, ts.dateTime.Day);
            Assert.Equal(23, ts.dateTime.Hour);
            Assert.Equal(45, ts.dateTime.Minute);

            Assert.True(false, "Not yet handling weather or compressed lat long position.");
        }

        /// <summary>
        ///  Complete Lat/Long Position Report Format — without Timestamp
        /// no timestamp, no APRS messaging, with comment
        /// </summary>
        [Fact]
        public void DecodeInformationFieldFromSpecExample_CompleteLatLongPositionReportFormatWithoutTimestamp_1()
        {
            Packet p = new Packet();
            p.DecodeInformationField("!4903.50N/07201.75W-Test 001234");

            Assert.Equal(Packet.Type.PositionWithoutTimestampNoMessaging, p.DecodedType);
            Assert.Equal(false, p.HasMessaging);
            Assert.Equal("Test 001234", p.comment);

            Position pos = (Position)p.position;
            Assert.Equal(new GeoCoordinate(49.0583, -72.0292), pos.Coordinates);
            Assert.Equal('/', pos.SymbolTableIdentifier);
            Assert.Equal('-', pos.SymbolCode);
        }

        /// <summary>
        ///  Complete Lat/Long Position Report Format — without Timestamp
        /// no timestamp, no APRS messaging, with comment
        /// </summary>
        [Fact]
        public void EncodeInformationFieldFromSpecExample_CompleteLatLongPositionReportFormatWithoutTimestamp_1()
        {
            Packet p = new Packet();

            p.HasMessaging = false;

            p.comment = "Test 001234";
            p.position = new Position(new GeoCoordinate(49.0583, -72.0292), '/', '-', 0);

        
            string encoded = p.EncodeInformationField(Packet.Type.PositionWithoutTimestampNoMessaging);
            Assert.Equal("!4903.50N/07201.75W-Test 001234", encoded);
        }

        /// <summary>
        ///  Complete Lat/Long Position Report Format — without Timestamp
        /// no timestamp, no APRS messaging, altitude = 1234 ft. 
        /// </summary>
        [Fact (Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeInformationFieldFromSpecExample_CompleteLatLongPositionReportFormatWithoutTimestamp_2()
        {
            Packet p = new Packet();
            p.DecodeInformationField("!4903.50N/07201.75W-Test /A=001234");

            Assert.Equal(Packet.Type.PositionWithoutTimestampNoMessaging, p.DecodedType);
            Assert.Equal(false, p.HasMessaging);
            Assert.Equal("Test /A=001234", p.comment);

            Position pos = (Position)p.position;
            Assert.Equal(new GeoCoordinate(49.0583, -72.0292), pos.Coordinates);
            Assert.Equal('/', pos.SymbolTableIdentifier);
            Assert.Equal('-', pos.SymbolCode);

            Assert.True(false, "Unhandled altitude.");
        }

        /// <summary>
        ///  Complete Lat/Long Position Report Format — without Timestamp
        /// no timestamp, no APRS messaging, location to nearest degree.
        /// </summary>
        [Fact]
        public void DecodeInformationFieldFromSpecExample_CompleteLatLongPositionReportFormatWithoutTimestamp_3()
        {
            Packet p = new Packet();
            p.DecodeInformationField("!49  .  N/072  .  W-");

            Assert.Equal(Packet.Type.PositionWithoutTimestampNoMessaging, p.DecodedType);
            Assert.Equal(false, p.HasMessaging);
            Assert.Equal(null, p.comment);

            Position pos = (Position)p.position;
            Assert.Equal(new GeoCoordinate(49, -72), pos.Coordinates);
            Assert.Equal('/', pos.SymbolTableIdentifier);
            Assert.Equal('-', pos.SymbolCode);
            Assert.Equal(4, pos.Ambiguity);
        }

        /// <summary>
        ///  Complete Lat/Long Position Report Format — without Timestamp
        /// no timestamp, no APRS messaging, location to nearest degree.
        /// </summary>
        [Fact]
        public void EncodeInformationFieldFromSpecExample_CompleteLatLongPositionReportFormatWithoutTimestamp_3()
        {
            Packet p = new Packet();

            p.HasMessaging = false;
            p.position = new Position(new GeoCoordinate(49, -72), '/', '-', 4);

            string encoded = p.EncodeInformationField(Packet.Type.PositionWithoutTimestampNoMessaging);
            Assert.Equal("!49  .  N/072  .  W-", encoded);
        }

        [Fact]
        public void GetTypeChar_DoNotUse()
        {
            Packet p = new Packet();
 
            try
            {
                p.GetTypeChar(Packet.Type.DoNotUse);

                Assert.True(false, "Should have thrown exception.");
            }
            catch (ArgumentException)
            {
                return;
            }
        }

        [Fact]
        public void GetTypeChar_1()
        {
            Packet p = new Packet();
      
            char value = (char)p.GetTypeChar(Packet.Type.PositionWithoutTimestampWithMessaging);

            Assert.Equal('=', value);
        }

        [Fact]
        public void GetDataType_1()
        {
            Packet p = new Packet();
   
            Packet.Type value = (Packet.Type)p.GetDataType("/092345z4903.50N/07201.75W>Test1234");

            Assert.Equal(Packet.Type.PositionWithTimestampNoMessaging, value);
        }

        [Fact]
        public void DecodeInformationFieldFromSpecExample_MaidenheadLocatorBeacon_1()
        {
            Packet p = new Packet();
  
            p.DecodeInformationField("[IO91SX] 35 miles NNW of London");

            Position pos = (Position)p.position;

            Assert.Equal(51.98, Math.Round(pos.Coordinates.Latitude, 2));
            Assert.Equal(-0.46, Math.Round(pos.Coordinates.Longitude, 2));
            Assert.Equal(" 35 miles NNW of London", p.comment);
        }

        [Fact]
        public void DecodeInformationFieldFromSpecExample_MaidenheadLocatorBeacon_2()
        {
            Packet p = new Packet();
            p.DecodeInformationField("[IO91SX]");

            Position pos = (Position)p.position;

            Assert.Equal(51.98, Math.Round(pos.Coordinates.Latitude, 2));
            Assert.Equal(-0.46, Math.Round(pos.Coordinates.Longitude, 2));
            Assert.Equal(null, p.comment);
        }

        [Fact]
        public void EncodeInformationFieldFromSpecExample_MaidenheadLocatorBeacon_1()
        {
            Packet p = new Packet();
            p.comment = "35 miles NNW of London";
            p.position = new Position(new GeoCoordinate(51.98, -0.46));

            string encoded = p.EncodeInformationField(Packet.Type.MaidenheadGridLocatorBeacon);

            Assert.Equal("[IO91SX] 35 miles NNW of London", encoded);
        }

        [Fact]
        public void EncodeInformationFieldFromSpecExample_MaidenheadLocatorBeacon_2()
        {
            Packet p = new Packet();
            p.position = new Position(new GeoCoordinate(51.98, -0.46));

            string encoded = p.EncodeInformationField(Packet.Type.MaidenheadGridLocatorBeacon);

            Assert.Equal("[IO91SX]", encoded);
        }

        [Fact]
        public void EncodeInformationFieldFromSpecExample_MaidenheadLocatorBeacon_3()
        {
            Packet p = new Packet();
            p.comment = "35 miles NNW of London";
            p.position = new Position();
            p.position.DecodeMaidenhead("IO91SX");

            string encoded = p.EncodeInformationField(Packet.Type.MaidenheadGridLocatorBeacon);

            Assert.Equal("[IO91SX] 35 miles NNW of London", encoded);
        }

        [Fact]
        public void EncodeInformationFieldFromSpecExample_MaidenheadLocatorBeacon_4()
        {
            Packet p = new Packet();
            p.position = new Position();
            p.position.DecodeMaidenhead("IO91SX");

            string encoded = p.EncodeInformationField(Packet.Type.MaidenheadGridLocatorBeacon);

            Assert.Equal("[IO91SX]", encoded);
        }

        [Fact]
        public void DecodeInformationFieldFromSpecExample_StatusReportFormatWithMaidenhead_1()
        {
            Packet p = new Packet();
   
            p.DecodeInformationField(">IO91SX/G");
 
            Position pos = (Position)p.position;
            Assert.Equal(51.98, Math.Round(pos.Coordinates.Latitude, 2));
            Assert.Equal(-0.46, Math.Round(pos.Coordinates.Longitude, 2));
            Assert.Equal('/', pos.SymbolTableIdentifier);
            Assert.Equal('G', pos.SymbolCode);
        }

        [Fact]
        public void DecodeInformationFieldFromSpecExample_StatusReportFormatWithMaidenhead_2()
        {
            Packet p = new Packet();

            p.DecodeInformationField(">IO91/G");
 
            Position pos = (Position)p.position;
            Assert.Equal(51.5, Math.Round(pos.Coordinates.Latitude, 2));
            Assert.Equal(-1.0, Math.Round(pos.Coordinates.Longitude, 2));
            Assert.Equal('/', pos.SymbolTableIdentifier);
            Assert.Equal('G', pos.SymbolCode);
        }

        [Fact]
        public void DecodeInformationFieldFromSpecExample_StatusReportFormatWithMaidenhead_3()
        {
            Packet p = new Packet();
  
            p.DecodeInformationField(">IO91SX/- My house");
    
            Position pos = (Position)p.position;
            Assert.Equal(51.98, Math.Round(pos.Coordinates.Latitude, 2));
            Assert.Equal(-0.46, Math.Round(pos.Coordinates.Longitude, 2));
            Assert.Equal('/', pos.SymbolTableIdentifier);
            Assert.Equal('-', pos.SymbolCode);

            string comment = p.comment;
            Assert.Equal("My house", comment);
        }

        [Fact (Skip = "Issue #24: Fix skipped tests from old repository")]
        public void DecodeInformationFieldFromSpecExample_StatusReportFormatWithMaidenhead_4()
        {
            Packet p = new Packet();
 
            p.DecodeInformationField(">IO91SX/- ^B7");
 
            Position pos = (Position)p.position;
            Assert.Equal(51.98, Math.Round(pos.Coordinates.Latitude, 2));
            Assert.Equal(-0.46, Math.Round(pos.Coordinates.Longitude, 2));
            Assert.Equal('/', pos.SymbolTableIdentifier);
            Assert.Equal('-', pos.SymbolCode);

            string comment = p.comment;
            Assert.Equal("^B7", comment);

            Assert.True(false, "Not handling Meteor Scatter beam");
        }

        [Fact]
        public void EncodeInformationFieldFromSpecExample_StatusReportFormatWithMaidenhead_1()
        {
            Packet p = new Packet();
            p.position = new Position(new GeoCoordinate(51.98, -0.46), '/', 'G');
            string encoded = p.EncodeInformationField(Packet.Type.Status);

            Assert.Equal(">IO91SX/G", encoded);
        }

        [Fact]
        public void EncodeInformationFieldFromSpecExample_StatusReportFormatWithMaidenhead_2()
        {
            Packet p = new Packet();
            p.position = new Position(new GeoCoordinate(51.98, -0.46), '/', 'G', 2);

            string encoded = p.EncodeInformationField(Packet.Type.Status);

            Assert.Equal(">IO91/G", encoded);
        }

        [Fact]
        public void EncodeInformationFieldFromSpecExample_StatusReportFormatWithMaidenhead_3()
        {
            Packet p = new Packet();
            p.position = new Position(new GeoCoordinate(51.98, -0.46), '/', '-');
            p.comment = "My house";

            string encoded = p.EncodeInformationField(Packet.Type.Status);

            Assert.Equal(">IO91SX/- My house", encoded);
        }

        [Fact]
        public void EncodeInformationFieldFromSpecExample_StatusReportFormatWithMaidenhead_4()
        {
            Packet p = new Packet();
            p.position = new Position(new GeoCoordinate(51.98, -0.46), '/', '-');
            p.comment = "My house";

            string encoded = p.EncodeInformationField(Packet.Type.Status);

            Assert.Equal(">IO91SX/- My house", encoded);
        }
    }
}
