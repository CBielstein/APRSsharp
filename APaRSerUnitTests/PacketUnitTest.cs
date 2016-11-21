using Microsoft.VisualStudio.TestTools.UnitTesting;
using APaRSer;

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
        /// Dcodes a lat/long position report format with timestamp, no APRS messaging, zulu time, with comment
        /// based on the example given in the APRS spec
        /// </summary>
        [TestMethod]
        public void DecodeInformationFieldFromSpecExample_LatLongPositionReportFormatWithTimestamp_1()
        {
            Packet p = new Packet();
            PrivateObject pp = new PrivateObject(p);
            try
            {
                pp.Invoke("DecodeInformationField", "/092345z4903.50N/07201.75W>Test1234");

                throw new System.Exception("Update test. It's now outdated as more functionality has been added.");
            }
            catch (System.NotImplementedException)
            {
                Assert.AreEqual(Packet.Type.PositionWithTimestampNoMessaging, pp.GetField("DecodedType"));
                Assert.AreEqual(false, pp.GetField("HasMessaging"));

                Timestamp ts = (Timestamp)pp.GetField("timestamp");
                Assert.AreEqual(Timestamp.Type.DHMz, ts.DecodedType);
                Assert.AreEqual(9, ts.dateTime.Day);
                Assert.AreEqual(23, ts.dateTime.Hour);
                Assert.AreEqual(45, ts.dateTime.Minute);
            }
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
            try
            {
                pp.Invoke("DecodeInformationField", "@092345/4903.50N/07201.75W>Test1234");

                throw new System.Exception("Update test. It's now outdated as more functionality has been added.");
            }
            catch (System.NotImplementedException)
            {
                Assert.AreEqual(Packet.Type.PositionWithTimestampWithMessaging, pp.GetField("DecodedType"));
                Assert.AreEqual(true, pp.GetField("HasMessaging"));

                Timestamp ts = (Timestamp)pp.GetField("timestamp");
                Assert.AreEqual(Timestamp.Type.DHMl, ts.DecodedType);
                Assert.AreEqual(9, ts.dateTime.Day);
                Assert.AreEqual(23, ts.dateTime.Hour);
                Assert.AreEqual(45, ts.dateTime.Minute);
            }
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
            try
            {
                pp.Invoke("DecodeInformationField", "@092345/4903.50N/07201.75W>088/036");

                throw new System.Exception("Update test. It's now outdated as more functionality has been added.");
            }
            catch (System.NotImplementedException)
            {
                Assert.AreEqual(Packet.Type.PositionWithTimestampWithMessaging, pp.GetField("DecodedType"));
                Assert.AreEqual(true, pp.GetField("HasMessaging"));

                Timestamp ts = (Timestamp)pp.GetField("timestamp");
                Assert.AreEqual(Timestamp.Type.DHMl, ts.DecodedType);
                Assert.AreEqual(9, ts.dateTime.Day);
                Assert.AreEqual(23, ts.dateTime.Hour);
                Assert.AreEqual(45, ts.dateTime.Minute);
            }
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
            try
            {
                pp.Invoke("DecodeInformationField", "@234517h4903.50N/07201.75W>PHG5132");

                throw new System.Exception("Update test. It's now outdated as more functionality has been added.");
            }
            catch (System.NotImplementedException)
            {
                Assert.AreEqual(Packet.Type.PositionWithTimestampWithMessaging, pp.GetField("DecodedType"));
                Assert.AreEqual(true, pp.GetField("HasMessaging"));

                Timestamp ts = (Timestamp)pp.GetField("timestamp");
                Assert.AreEqual(Timestamp.Type.HMS, ts.DecodedType);
                Assert.AreEqual(23, ts.dateTime.Hour);
                Assert.AreEqual(45, ts.dateTime.Minute);
                Assert.AreEqual(17, ts.dateTime.Second);
            }
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
            try
            {
                pp.Invoke("DecodeInformationField", "@092345z4903.50N/07201.75W>RNG0050");

                throw new System.Exception("Update test. It's now outdated as more functionality has been added.");
            }
            catch (System.NotImplementedException)
            {
                Assert.AreEqual(Packet.Type.PositionWithTimestampWithMessaging, pp.GetField("DecodedType"));
                Assert.AreEqual(true, pp.GetField("HasMessaging"));

                Timestamp ts = (Timestamp)pp.GetField("timestamp");
                Assert.AreEqual(Timestamp.Type.DHMz, ts.DecodedType);
                Assert.AreEqual(09, ts.dateTime.Day);
                Assert.AreEqual(23, ts.dateTime.Hour);
                Assert.AreEqual(45, ts.dateTime.Minute);
            }
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
            try
            {
                pp.Invoke("DecodeInformationField", "/234517h4903.50N/07201.75W>DFS2360");

                throw new System.Exception("Update test. It's now outdated as more functionality has been added.");
            }
            catch (System.NotImplementedException)
            {
                Assert.AreEqual(Packet.Type.PositionWithTimestampNoMessaging, pp.GetField("DecodedType"));
                Assert.AreEqual(false, pp.GetField("HasMessaging"));

                Timestamp ts = (Timestamp)pp.GetField("timestamp");
                Assert.AreEqual(Timestamp.Type.HMS, ts.DecodedType);
                Assert.AreEqual(23, ts.dateTime.Hour);
                Assert.AreEqual(45, ts.dateTime.Minute);
                Assert.AreEqual(17, ts.dateTime.Second);
            }
        }

        // TODO: Enable when weather reports are working
        /*
        /// <summary>
        /// Lat/Long Position Report Format — with Data Extension and Timestamp 
        /// weather report
        /// </summary>
        [TestMethod]
        public void DecodeInformationFieldFromSpecExample_LatLongPositionReportFormatWithDataExtensionAndTimestamp_5()
        {
            Packet p = new Packet();
            PrivateObject pp = new PrivateObject(p);
            try
            {
                pp.Invoke("DecodeInformationField", " @092345z4903.50N/07201.75W_090/000g000t066r000p000…dUII ");

                throw new System.Exception("Update test. It's now outdated as more functionality has been added.");
            }
            catch (System.NotImplementedException)
            {
                Assert.AreEqual(Packet.Type.WeatherReport, pp.GetField("DecodedType"));
                Assert.AreEqual(false, pp.GetField("HasMessaging"));

                Timestamp ts = (Timestamp)pp.GetField("timestamp");
                Assert.AreEqual(Timestamp.Type.DHMz, ts.DecodedType);
                Assert.AreEqual(09, ts.dateTime.Day);
                Assert.AreEqual(23, ts.dateTime.Hour);
                Assert.AreEqual(45, ts.dateTime.Minute);
            }
        }
        */

        /// <summary>
        ///  DF Report Format — with Timestamp
        ///  with timestamp, course/speed/bearing/NRQ, with APRS messaging. 
        /// </summary>
        [TestMethod]
        public void DecodeInformationFieldFromSpecExample_DFReportFormat_1()
        {
            Packet p = new Packet();
            PrivateObject pp = new PrivateObject(p);
            try
            {
                pp.Invoke("DecodeInformationField", "@092345z4903.50N/07201.75W\088/036/270/729");

                throw new System.Exception("Update test. It's now outdated as more functionality has been added.");
            }
            catch (System.NotImplementedException)
            {
                Assert.AreEqual(Packet.Type.PositionWithTimestampWithMessaging, pp.GetField("DecodedType"));
                Assert.AreEqual(true, pp.GetField("HasMessaging"));

                Timestamp ts = (Timestamp)pp.GetField("timestamp");
                Assert.AreEqual(Timestamp.Type.DHMz, ts.DecodedType);
                Assert.AreEqual(09, ts.dateTime.Day);
                Assert.AreEqual(23, ts.dateTime.Hour);
                Assert.AreEqual(45, ts.dateTime.Minute);
            }
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
            try
            {
                pp.Invoke("DecodeInformationField", "/092345z4903.50N/07201.75W\000/000/270/729");

                throw new System.Exception("Update test. It's now outdated as more functionality has been added.");
            }
            catch (System.NotImplementedException)
            {
                Assert.AreEqual(Packet.Type.PositionWithTimestampNoMessaging, pp.GetField("DecodedType"));
                Assert.AreEqual(false, pp.GetField("HasMessaging"));

                Timestamp ts = (Timestamp)pp.GetField("timestamp");
                Assert.AreEqual(Timestamp.Type.DHMz, ts.DecodedType);
                Assert.AreEqual(09, ts.dateTime.Day);
                Assert.AreEqual(23, ts.dateTime.Hour);
                Assert.AreEqual(45, ts.dateTime.Minute);
            }
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
            try
            {
                pp.Invoke("DecodeInformationField", "@092345z/5L!!<*e7>{?!");

                throw new System.Exception("Update test. It's now outdated as more functionality has been added.");
            }
            catch (System.NotImplementedException)
            {
                Assert.AreEqual(Packet.Type.PositionWithTimestampWithMessaging, pp.GetField("DecodedType"));
                Assert.AreEqual(true, pp.GetField("HasMessaging"));

                Timestamp ts = (Timestamp)pp.GetField("timestamp");
                Assert.AreEqual(Timestamp.Type.DHMz, ts.DecodedType);
                Assert.AreEqual(09, ts.dateTime.Day);
                Assert.AreEqual(23, ts.dateTime.Hour);
                Assert.AreEqual(45, ts.dateTime.Minute);
            }
        }

        /// <summary>
        ///  Complete Weather Report Format — with Lat/Long position and Timestamp
        /// </summary>
        [TestMethod]
        public void DecodeInformationFieldFromSpecExample_CompleteWeatherReportFormatwithLatLongPositionAndTimestamp()
        {
            Packet p = new Packet();
            PrivateObject pp = new PrivateObject(p);
            try
            {
                pp.Invoke("DecodeInformationField", "@092345z4903.50N/07201.75W_220/004g005t-07r000p000P000h50b09900wRSW");

                throw new System.Exception("Update test. It's now outdated as more functionality has been added.");
            }
            catch (System.NotImplementedException)
            {
                Assert.AreEqual(Packet.Type.PositionWithTimestampWithMessaging, pp.GetField("DecodedType"));
                Assert.AreEqual(true, pp.GetField("HasMessaging"));

                Timestamp ts = (Timestamp)pp.GetField("timestamp");
                Assert.AreEqual(Timestamp.Type.DHMz, ts.DecodedType);
                Assert.AreEqual(09, ts.dateTime.Day);
                Assert.AreEqual(23, ts.dateTime.Hour);
                Assert.AreEqual(45, ts.dateTime.Minute);
            }
        }

        /// <summary>
        ///  Complete Weather Report Format — with Compressed Lat/Long position, with Timestamp
        /// </summary>
        [TestMethod]
        public void DecodeInformationFieldFromSpecExample_CompleteWeatherReportFormatWithCompressedLatLongPositionWithTimestamp()
        {
            Packet p = new Packet();
            PrivateObject pp = new PrivateObject(p);
            try
            {
                pp.Invoke("DecodeInformationField", "@092345z/5L!!<*e7 _7P[g005t077r000p000P000h50b09900wRSW");

                throw new System.Exception("Update test. It's now outdated as more functionality has been added.");
            }
            catch (System.NotImplementedException)
            {
                Assert.AreEqual(Packet.Type.PositionWithTimestampWithMessaging, pp.GetField("DecodedType"));
                Assert.AreEqual(true, pp.GetField("HasMessaging"));

                Timestamp ts = (Timestamp)pp.GetField("timestamp");
                Assert.AreEqual(Timestamp.Type.DHMz, ts.DecodedType);
                Assert.AreEqual(09, ts.dateTime.Day);
                Assert.AreEqual(23, ts.dateTime.Hour);
                Assert.AreEqual(45, ts.dateTime.Minute);
            }
        }
    }
}
