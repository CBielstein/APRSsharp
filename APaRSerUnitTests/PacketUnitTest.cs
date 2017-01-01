using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using APaRSer;
using GeoCoordinatePortable;

namespace APaRSerUnitTests
{
    [TestClass]
    public class PacketUnitTest
    {
        //
        // NOTE: Many of these are testing incomplete functionality.
        // Any catch of System.NotImplementedException should be considered for removal in the future.
        //

        /// <summary>
        /// Dcodes a positionless weather report based on the example given in the APRS spec
        /// </summary>
        [TestMethod]
        public void DecodeInformationFieldFromSpecExample_PositionlessWeatherReportFormat()
        {
            Packet p = new Packet();
            PrivateObject pp = new PrivateObject(p);
            try
            {
                pp.Invoke("DecodeInformationField", "_10090556c220s004g005t077r000p000P000h50b09900wRSW");

                throw new System.Exception("Update test. It's now outdated as more functionality has been added.");
            }
            catch (System.NotImplementedException)
            {
                Assert.AreEqual(Packet.Type.WeatherReport, pp.GetField("DecodedType"));
                Assert.AreEqual(false, pp.GetField("HasMessaging"));

                Timestamp ts = (Timestamp)pp.GetField("timestamp");
                Assert.AreEqual(Timestamp.Type.MDHM, ts.DecodedType);
                Assert.AreEqual(10, ts.dateTime.Month);
                Assert.AreEqual(9, ts.dateTime.Day);
                Assert.AreEqual(05, ts.dateTime.Hour);
                Assert.AreEqual(56, ts.dateTime.Minute);
            }
        }

        /// <summary>
        /// Decodes a lat/long position report format with timestamp, no APRS messaging, zulu time, with comment
        /// based on the example given in the APRS spec
        /// </summary>
        [TestMethod]
        public void DecodeInformationFieldFromSpecExample_LatLongPositionReportFormatWithTimestamp_1()
        {
            Packet p = new Packet();
            PrivateObject pp = new PrivateObject(p);

            pp.Invoke("DecodeInformationField", "/092345z4903.50N/07201.75W>Test1234");

            Assert.AreEqual(Packet.Type.PositionWithTimestampNoMessaging, pp.GetField("DecodedType"));
            Assert.AreEqual(false, pp.GetField("HasMessaging"));

            Timestamp ts = (Timestamp)pp.GetField("timestamp");
            Assert.AreEqual(Timestamp.Type.DHMz, ts.DecodedType);
            Assert.AreEqual(9, ts.dateTime.Day);
            Assert.AreEqual(23, ts.dateTime.Hour);
            Assert.AreEqual(45, ts.dateTime.Minute);

            Position pos = (Position)pp.GetField("position");
            Assert.AreEqual(new GeoCoordinate(49.0583, -72.0292), pos.Coordinates);
            Assert.AreEqual('/', pos.SymbolTableIdentifier);
            Assert.AreEqual('>', pos.SymbolCode);

            Assert.AreEqual("Test1234", pp.GetField("comment"));
        }

        /// <summary>
        /// Decodes a lat/long position report format with timestamp, no APRS messaging, zulu time, with comment
        /// based on the example given in the APRS spec
        /// </summary>
        [TestMethod]
        public void EncodeInformationFieldFromSpecExample_LatLongPositionReportFormatWithTimestamp_1()
        {
            Packet p = new Packet();

            DateTime dt = new DateTime(2016, 12, 9, 23, 45, 0, 0, DateTimeKind.Utc);
            p.timestamp = new Timestamp(dt);

            p.HasMessaging = false;

            GeoCoordinate gc = new GeoCoordinate(49.0583, -72.0292);
            p.position = new Position(gc, '/', '>', 0);

            p.comment = "Test1234";

            PrivateObject pp = new PrivateObject(p);
            string encoded = (string)pp.Invoke("EncodeInformationField", Packet.Type.PositionWithTimestampNoMessaging);

            Assert.AreEqual("/092345z4903.50N/07201.75W>Test1234", encoded);
        }

        /// <summary>
        /// Dcodes a lat/long position report format with timestamp, with APRS messaging, local time, with comment
        /// based on the example given in the APRS spec
        /// </summary>
        [TestMethod]
        public void DecodeInformationFieldFromSpecExample_LatLongPositionReportFormatWithTimestamp_2()
        {
            Packet p = new Packet();
            PrivateObject pp = new PrivateObject(p);

            pp.Invoke("DecodeInformationField", "@092345/4903.50N/07201.75W>Test1234");

            Assert.AreEqual(Packet.Type.PositionWithTimestampWithMessaging, pp.GetField("DecodedType"));
            Assert.AreEqual(true, pp.GetField("HasMessaging"));

            Timestamp ts = (Timestamp)pp.GetField("timestamp");
            Assert.AreEqual(Timestamp.Type.DHMl, ts.DecodedType);
            Assert.AreEqual(9, ts.dateTime.Day);
            Assert.AreEqual(23, ts.dateTime.Hour);
            Assert.AreEqual(45, ts.dateTime.Minute);

            Position pos = (Position)pp.GetField("position");
            Assert.AreEqual(new GeoCoordinate(49.0583, -72.0292), pos.Coordinates);
            Assert.AreEqual('/', pos.SymbolTableIdentifier);
            Assert.AreEqual('>', pos.SymbolCode);

            Assert.AreEqual("Test1234", pp.GetField("comment"));
        }

        /// <summary>
        /// Dcodes a lat/long position report format with timestamp, with APRS messaging, local time, with comment
        /// based on the example given in the APRS spec
        /// </summary>
        [TestMethod]
        public void EncodeInformationFieldFromSpecExample_LatLongPositionReportFormatWithTimestamp_2()
        {
            Packet p = new Packet();

            DateTime dt = new DateTime(2016, 12, 9, 23, 45, 00, DateTimeKind.Local);
            p.timestamp = new Timestamp(dt);

            p.HasMessaging = true;

            p.position = new Position(new GeoCoordinate(49.0583, -72.0292), '/', '>', 0);

            p.comment = "Test1234";

            PrivateObject pp = new PrivateObject(p);

            string encoded = (string)pp.Invoke("EncodeInformationField", Packet.Type.PositionWithTimestampWithMessaging, Timestamp.Type.DHMl);

            Assert.AreEqual("@092345/4903.50N/07201.75W>Test1234", encoded);
        }

        /// <summary>
        /// Lat/Long Position Report Format — with Data Extension and Timestamp 
        /// with timestamp, with APRS messaging, local time, course/speed.
        /// </summary>
        [TestMethod]
        public void DecodeInformationFieldFromSpecExample_LatLongPositionReportFormatWithDataExtensionAndTimestamp_1()
        {
            Packet p = new Packet();
            PrivateObject pp = new PrivateObject(p);

            pp.Invoke("DecodeInformationField", "@092345/4903.50N/07201.75W>088/036");

            Assert.AreEqual(Packet.Type.PositionWithTimestampWithMessaging, pp.GetField("DecodedType"));
            Assert.AreEqual(true, pp.GetField("HasMessaging"));

            Timestamp ts = (Timestamp)pp.GetField("timestamp");
            Assert.AreEqual(Timestamp.Type.DHMl, ts.DecodedType);
            Assert.AreEqual(9, ts.dateTime.Day);
            Assert.AreEqual(23, ts.dateTime.Hour);
            Assert.AreEqual(45, ts.dateTime.Minute);

            Assert.Fail("Not yet handling data extension.");
        }

        /// <summary>
        /// Lat/Long Position Report Format — with Data Extension and Timestamp 
        /// with timestamp, APRS messaging, hours/mins/secs time, PHG
        /// </summary>
        [TestMethod]
        public void DecodeInformationFieldFromSpecExample_LatLongPositionReportFormatWithDataExtensionAndTimestamp_2()
        {
            Packet p = new Packet();
            PrivateObject pp = new PrivateObject(p);

            pp.Invoke("DecodeInformationField", "@234517h4903.50N/07201.75W>PHG5132");

            Assert.AreEqual(Packet.Type.PositionWithTimestampWithMessaging, pp.GetField("DecodedType"));
            Assert.AreEqual(true, pp.GetField("HasMessaging"));

            Timestamp ts = (Timestamp)pp.GetField("timestamp");
            Assert.AreEqual(Timestamp.Type.HMS, ts.DecodedType);
            Assert.AreEqual(23, ts.dateTime.Hour);
            Assert.AreEqual(45, ts.dateTime.Minute);
            Assert.AreEqual(17, ts.dateTime.Second);

            Assert.Fail("Not yet handling data extension.");
        }

        /// <summary>
        /// Lat/Long Position Report Format — with Data Extension and Timestamp 
        /// with timestamp, APRS messaging, zulu time, radio range.
        /// </summary>
        [TestMethod]
        public void DecodeInformationFieldFromSpecExample_LatLongPositionReportFormatWithDataExtensionAndTimestamp_3()
        {
            Packet p = new Packet();
            PrivateObject pp = new PrivateObject(p);

            pp.Invoke("DecodeInformationField", "@092345z4903.50N/07201.75W>RNG0050");

            Assert.AreEqual(Packet.Type.PositionWithTimestampWithMessaging, pp.GetField("DecodedType"));
            Assert.AreEqual(true, pp.GetField("HasMessaging"));

            Timestamp ts = (Timestamp)pp.GetField("timestamp");
            Assert.AreEqual(Timestamp.Type.DHMz, ts.DecodedType);
            Assert.AreEqual(09, ts.dateTime.Day);
            Assert.AreEqual(23, ts.dateTime.Hour);
            Assert.AreEqual(45, ts.dateTime.Minute);

            Assert.Fail("Not yet handling data extensions.");
        }

        /// <summary>
        /// Lat/Long Position Report Format — with Data Extension and Timestamp 
        /// with timestamp, hours/mins/secs time, DF, no APRS messaging
        /// </summary>
        [TestMethod]
        public void DecodeInformationFieldFromSpecExample_LatLongPositionReportFormatWithDataExtensionAndTimestamp_4()
        {
            Packet p = new Packet();
            PrivateObject pp = new PrivateObject(p);

            pp.Invoke("DecodeInformationField", "/234517h4903.50N/07201.75W>DFS2360");

            Assert.AreEqual(Packet.Type.PositionWithTimestampNoMessaging, pp.GetField("DecodedType"));
            Assert.AreEqual(false, pp.GetField("HasMessaging"));

            Timestamp ts = (Timestamp)pp.GetField("timestamp");
            Assert.AreEqual(Timestamp.Type.HMS, ts.DecodedType);
            Assert.AreEqual(23, ts.dateTime.Hour);
            Assert.AreEqual(45, ts.dateTime.Minute);
            Assert.AreEqual(17, ts.dateTime.Second);

            Position pos = (Position)pp.GetField("position");
            Assert.AreEqual(new GeoCoordinate(49.0583, -72.0292), pos.Coordinates);
            Assert.AreEqual('/', pos.SymbolTableIdentifier);
            Assert.AreEqual('>', pos.SymbolCode);

            Assert.Fail("Not yet handling DF data.");
        }

        /// <summary>
        /// Lat/Long Position Report Format — with Data Extension and Timestamp 
        /// weather report
        /// </summary>
        [TestMethod]
        public void DecodeInformationFieldFromSpecExample_LatLongPositionReportFormatWithDataExtensionAndTimestamp_5()
        {
            Packet p = new Packet();
            PrivateObject pp = new PrivateObject(p);

            pp.Invoke("DecodeInformationField", "@092345z4903.50N/07201.75W_090/000g000t066r000p000…dUII");

            Assert.AreEqual(Packet.Type.WeatherReport, pp.GetField("DecodedType"));
            Assert.AreEqual(false, pp.GetField("HasMessaging"));

            Timestamp ts = (Timestamp)pp.GetField("timestamp");
            Assert.AreEqual(Timestamp.Type.DHMz, ts.DecodedType);
            Assert.AreEqual(09, ts.dateTime.Day);
            Assert.AreEqual(23, ts.dateTime.Hour);
            Assert.AreEqual(45, ts.dateTime.Minute);

            Assert.Fail("Not yet handling weather reports");
        }

        /// <summary>
        ///  DF Report Format — with Timestamp
        ///  with timestamp, course/speed/bearing/NRQ, with APRS messaging. 
        /// </summary>
        [TestMethod]
        public void DecodeInformationFieldFromSpecExample_DFReportFormat_1()
        {
            Packet p = new Packet();
            PrivateObject pp = new PrivateObject(p);

            pp.Invoke("DecodeInformationField", "@092345z4903.50N/07201.75W\088/036/270/729");

            Assert.AreEqual(Packet.Type.PositionWithTimestampWithMessaging, pp.GetField("DecodedType"));
            Assert.AreEqual(true, pp.GetField("HasMessaging"));

            Timestamp ts = (Timestamp)pp.GetField("timestamp");
            Assert.AreEqual(Timestamp.Type.DHMz, ts.DecodedType);
            Assert.AreEqual(09, ts.dateTime.Day);
            Assert.AreEqual(23, ts.dateTime.Hour);
            Assert.AreEqual(45, ts.dateTime.Minute);

            Assert.Fail("Not yet handling DF Report.");
        }

        /// <summary>
        ///  DF Report Format — with Timestamp
        ///   with timestamp, bearing/NRQ, no course/speed, no APRS messaging.
        /// </summary>
        [TestMethod]
        public void DecodeInformationFieldFromSpecExample_DFReportFormat_2()
        {
            Packet p = new Packet();
            PrivateObject pp = new PrivateObject(p);

            pp.Invoke("DecodeInformationField", @"/092345z4903.50N/07201.75W\000/000/270/729");

            Assert.AreEqual(Packet.Type.PositionWithTimestampNoMessaging, pp.GetField("DecodedType"));
            Assert.AreEqual(false, pp.GetField("HasMessaging"));

            Timestamp ts = (Timestamp)pp.GetField("timestamp");
            Assert.AreEqual(Timestamp.Type.DHMz, ts.DecodedType);
            Assert.AreEqual(09, ts.dateTime.Day);
            Assert.AreEqual(23, ts.dateTime.Hour);
            Assert.AreEqual(45, ts.dateTime.Minute);

            Position pos = (Position)pp.GetField("position");
            Assert.AreEqual(new GeoCoordinate(49.0583, -72.0292), pos.Coordinates);
            Assert.AreEqual('/', pos.SymbolTableIdentifier);
            Assert.AreEqual('\\', pos.SymbolCode);

            Assert.Fail("Not yet handling bearing, course/speed.");
        }

        /// <summary>
        ///  Compressed Lat/Long Position Report Format — with Timestamp
        ///  with APRS messaging, timestamp, radio range
        /// </summary>
        [TestMethod]
        public void DecodeInformationFieldFromSpecExample_CompressedLatLongPositionReportFormat()
        {
            Packet p = new Packet();
            PrivateObject pp = new PrivateObject(p);

            pp.Invoke("DecodeInformationField", "@092345z/5L!!<*e7>{?!");

            Assert.AreEqual(Packet.Type.PositionWithTimestampWithMessaging, pp.GetField("DecodedType"));
            Assert.AreEqual(true, pp.GetField("HasMessaging"));

            Timestamp ts = (Timestamp)pp.GetField("timestamp");
            Assert.AreEqual(Timestamp.Type.DHMz, ts.DecodedType);
            Assert.AreEqual(09, ts.dateTime.Day);
            Assert.AreEqual(23, ts.dateTime.Hour);
            Assert.AreEqual(45, ts.dateTime.Minute);

            Assert.Fail("Not yet handling compressed latlong position report format.");
        }

        /// <summary>
        ///  Complete Weather Report Format — with Lat/Long position and Timestamp
        /// </summary>
        [TestMethod]
        public void DecodeInformationFieldFromSpecExample_CompleteWeatherReportFormatwithLatLongPositionAndTimestamp()
        {
            Packet p = new Packet();
            PrivateObject pp = new PrivateObject(p);

            pp.Invoke("DecodeInformationField", "@092345z4903.50N/07201.75W_220/004g005t-07r000p000P000h50b09900wRSW");

            Assert.AreEqual(Packet.Type.PositionWithTimestampWithMessaging, pp.GetField("DecodedType"));
            Assert.AreEqual(true, pp.GetField("HasMessaging"));

            Timestamp ts = (Timestamp)pp.GetField("timestamp");
            Assert.AreEqual(Timestamp.Type.DHMz, ts.DecodedType);
            Assert.AreEqual(09, ts.dateTime.Day);
            Assert.AreEqual(23, ts.dateTime.Hour);
            Assert.AreEqual(45, ts.dateTime.Minute);

            Assert.Fail("Not yet handling weather information.");
        }

        /// <summary>
        ///  Complete Weather Report Format — with Compressed Lat/Long position, with Timestamp
        /// </summary>
        [TestMethod]
        public void DecodeInformationFieldFromSpecExample_CompleteWeatherReportFormatWithCompressedLatLongPositionWithTimestamp()
        {
            Packet p = new Packet();
            PrivateObject pp = new PrivateObject(p);

            pp.Invoke("DecodeInformationField", "@092345z/5L!!<*e7 _7P[g005t077r000p000P000h50b09900wRSW");

            Assert.AreEqual(Packet.Type.PositionWithTimestampWithMessaging, pp.GetField("DecodedType"));
            Assert.AreEqual(true, pp.GetField("HasMessaging"));

            Timestamp ts = (Timestamp)pp.GetField("timestamp");
            Assert.AreEqual(Timestamp.Type.DHMz, ts.DecodedType);
            Assert.AreEqual(09, ts.dateTime.Day);
            Assert.AreEqual(23, ts.dateTime.Hour);
            Assert.AreEqual(45, ts.dateTime.Minute);

            Assert.Fail("Not yet handling weather or compressed lat long position.");
        }

        /// <summary>
        ///  Complete Lat/Long Position Report Format — without Timestamp
        /// no timestamp, no APRS messaging, with comment
        /// </summary>
        [TestMethod]
        public void DecodeInformationFieldFromSpecExample_CompleteLatLongPositionReportFormatWithoutTimestamp_1()
        {
            Packet p = new Packet();
            PrivateObject pp = new PrivateObject(p);
            pp.Invoke("DecodeInformationField", "!4903.50N/07201.75W-Test 001234");

            Assert.AreEqual(Packet.Type.PositionWithoutTimestampNoMessaging, pp.GetField("DecodedType"));
            Assert.AreEqual(false, pp.GetField("HasMessaging"));
            Assert.AreEqual("Test 001234", pp.GetField("comment"));

            Position pos = (Position)pp.GetField("position");
            Assert.AreEqual(new GeoCoordinate(49.0583, -72.0292), pos.Coordinates);
            Assert.AreEqual('/', pos.SymbolTableIdentifier);
            Assert.AreEqual('-', pos.SymbolCode);
        }

        /// <summary>
        ///  Complete Lat/Long Position Report Format — without Timestamp
        /// no timestamp, no APRS messaging, with comment
        /// </summary>
        [TestMethod]
        public void EncodeInformationFieldFromSpecExample_CompleteLatLongPositionReportFormatWithoutTimestamp_1()
        {
            Packet p = new Packet();

            p.HasMessaging = false;

            p.comment = "Test 001234";

            Position pos = new Position(new GeoCoordinate(49.0583, -72.0292), '/', '-', 0);

            PrivateObject pp = new PrivateObject(p);
            string encoded = (string)pp.Invoke("EncodeInformationField", Packet.Type.PositionWithoutTimestampNoMessaging);
            Assert.AreEqual("!4903.50N/07201.75W-Test 001234", encoded);
        }

        /// <summary>
        ///  Complete Lat/Long Position Report Format — without Timestamp
        /// no timestamp, no APRS messaging, altitude = 1234 ft. 
        /// </summary>
        [TestMethod]
        public void DecodeInformationFieldFromSpecExample_CompleteLatLongPositionReportFormatWithoutTimestamp_2()
        {
            Packet p = new Packet();
            PrivateObject pp = new PrivateObject(p);
            pp.Invoke("DecodeInformationField", "!4903.50N/07201.75W-Test /A=001234");

            Assert.AreEqual(Packet.Type.PositionWithoutTimestampNoMessaging, pp.GetField("DecodedType"));
            Assert.AreEqual(false, pp.GetField("HasMessaging"));
            Assert.AreEqual("Test /A=001234", pp.GetField("comment"));

            Position pos = (Position)pp.GetField("position");
            Assert.AreEqual(new GeoCoordinate(49.0583, -72.0292), pos.Coordinates);
            Assert.AreEqual('/', pos.SymbolTableIdentifier);
            Assert.AreEqual('-', pos.SymbolCode);

            Assert.Fail("Unhandled altitude.");
        }

        /// <summary>
        ///  Complete Lat/Long Position Report Format — without Timestamp
        /// no timestamp, no APRS messaging, location to nearest degree.
        /// </summary>
        [TestMethod]
        public void DecodeInformationFieldFromSpecExample_CompleteLatLongPositionReportFormatWithoutTimestamp_3()
        {
            Packet p = new Packet();
            PrivateObject pp = new PrivateObject(p);
            pp.Invoke("DecodeInformationField", "!49  .  N/072  .  W-");

            Assert.AreEqual(Packet.Type.PositionWithoutTimestampNoMessaging, pp.GetField("DecodedType"));
            Assert.AreEqual(false, pp.GetField("HasMessaging"));
            Assert.AreEqual(null, pp.GetField("comment"));

            Position pos = (Position)pp.GetField("position");
            Assert.AreEqual(new GeoCoordinate(49, -72), pos.Coordinates);
            Assert.AreEqual('/', pos.SymbolTableIdentifier);
            Assert.AreEqual('-', pos.SymbolCode);
            Assert.AreEqual(4, pos.Ambiguity);
        }

        /// <summary>
        ///  Complete Lat/Long Position Report Format — without Timestamp
        /// no timestamp, no APRS messaging, location to nearest degree.
        /// </summary>
        [TestMethod]
        public void EncodeInformationFieldFromSpecExample_CompleteLatLongPositionReportFormatWithoutTimestamp_3()
        {
            Packet p = new Packet();

            p.HasMessaging = false;

            Position pos = new Position(new GeoCoordinate(49, -72), '/', '-', 4);

            PrivateObject pp = new PrivateObject(p);
            string encoded = (string)pp.Invoke("EncodeInformationField", Packet.Type.PositionWithoutTimestampNoMessaging);
            Assert.AreEqual("!49  .  N/072  .  W-", encoded);
        }

        [TestMethod]
        public void GetTypeChar_DoNotUse()
        {
            Packet p = new Packet();
            PrivateObject pp = new PrivateObject(p);

            try
            {
                pp.Invoke("GetTypeChar", Packet.Type.DoNotUse);

                Assert.Fail("Should have thrown exception.");
            }
            catch (ArgumentException)
            {
                return;
            }
        }

        [TestMethod]
        public void GetTypeChar_1()
        {
            Packet p = new Packet();
            PrivateObject pp = new PrivateObject(p);

            char value = (char)pp.Invoke("GetTypeChar", Packet.Type.PositionWithoutTimestampWithMessaging);

            Assert.AreEqual('=', value);
        }

        [TestMethod]
        public void GetDataType_1()
        {
            Packet p = new Packet();
            PrivateObject pp = new PrivateObject(p);

            Packet.Type value = (Packet.Type)pp.Invoke("GetDataType", "/092345z4903.50N/07201.75W>Test1234");

            Assert.AreEqual(Packet.Type.PositionWithTimestampNoMessaging, value);
        }
    }
}
