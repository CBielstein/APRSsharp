using System;

namespace APaRSer
{
    public class Timestamp
    {
        public DateTime dateTime = DateTime.Now.ToUniversalTime();
        public Type DecodedType = Type.NotDecoded;

        public enum Type
        {
            /// <summary>
            /// Days/Hours/Minutes zulu
            /// </summary>
            DHMz,
            /// <summary>
            /// Days/Hours/Minutes local
            /// </summary>
            DHMl,
            /// <summary>
            /// Hours/Minutes/Seconds (always zulu)
            /// </summary>
            HMS,
            /// <summary>
            /// Hours/Minutes/Seconds (always zulu)
            /// </summary>
            MDHM,
            /// <summary>
            /// Not a decoded timestamp
            /// </summary>
            NotDecoded,
        }

        /// <summary>
        /// Initializes this object by decoding the given timestamp in to it
        /// </summary>
        /// <param name="timestamp">APRS timestamp</param>
        public Timestamp(string timestamp)
        {
            Decode(timestamp);
        }

        /// <summary>
        /// Initializes a Timestamp
        /// </summary>
        public Timestamp() { }

        /// <summary>
        /// Initializes the Timestamp given a DateTime object
        /// </summary>
        /// <param name="dt">DateTime object to use for this Timestamp</param>
        public Timestamp(DateTime dt)
        {
            dateTime = dt;
        }

        /// <summary>
        /// Copy constructor. Initializes a Timestamp from a different Timestamp
        /// </summary>
        /// <param name="ts">Timestamp to clone</param>
        public Timestamp(Timestamp ts)
        {
            dateTime = ts.dateTime;
            DecodedType = ts.DecodedType;
        }

        /// <summary>
        /// Encodes the data from the stored datetime as a string with the requested type
        /// </summary>
        /// <param name="type">The APRS timestamp type. Read about the types in the documentation for Timestamp.Type</param>
        /// <returns>String. 7-8 characters as defined by the APRS spec</returns>
        public string Encode(Timestamp.Type type)
        {
            switch (type)
            {
                case Type.DHMz:
                    return EncodeDHM(true /* isZulu*/);
                case Type.DHMl:
                    return EncodeDHM(false /* isZulu */);
                case Type.HMS:
                    return EncodeHMS();
                case Type.MDHM:
                    return EncodeMDHM();
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Encodes to a Day/Hour/Minute (DHM) string in zulu time or local time.
        /// </summary>
        /// <param name="isZulu">If true, encodes in DHM with zulu time, else local time. Zulu time should be used.</param>
        /// <returns>DHM encoded APRS timestamp string</returns>
        private string EncodeDHM(bool isZulu)
        {
            string encodedPacket = String.Empty;
            DateTime convertedDateTime = isZulu ? dateTime.ToUniversalTime() : dateTime.ToLocalTime();

            // Add day
            encodedPacket += dateTime.Day.ToString("D2");

            // Add hour
            encodedPacket += dateTime.Hour.ToString("D2");

            // Add minute
            encodedPacket += dateTime.Minute.ToString("D2");

            // Add time indicator
            encodedPacket += isZulu ? "z" : "/";

            return encodedPacket;
        }

        /// <summary>
        /// Encodes to an Hour/Minute/Second (HMS) string in zulu time
        /// </summary>
        /// <returns>HMS encoded APRS timestamp string</returns>
        private string EncodeHMS()
        {
            string encodedPacket = String.Empty;
            DateTime convertedDateTime = dateTime.ToUniversalTime();

            // Add hour
            encodedPacket += dateTime.Hour.ToString("D2");

            // Add minute
            encodedPacket += dateTime.Minute.ToString("D2");

            // Add second
            encodedPacket += dateTime.Second.ToString("D2");

            // Add the time indicator
            encodedPacket += "h";

            return encodedPacket;
        }

        /// <summary>
        /// Encodes to an Month/Day/Hour/Minute (MDHM) string in zulu time
        /// </summary>
        /// <returns>MDHM encoded APRS timestamp string</returns>
        private string EncodeMDHM()
        {
            string encodedPacket = String.Empty;
            DateTime convertedDateTime = dateTime.ToUniversalTime();

            // Add month
            encodedPacket += dateTime.Month.ToString("D2");

            // Add day
            encodedPacket += dateTime.Day.ToString("D2");

            // Add hour
            encodedPacket += dateTime.Hour.ToString("D2");

            // Add minute
            encodedPacket += dateTime.Minute.ToString("D2");

            return encodedPacket;
        }

        /// <summary>
        /// Takes the APRS Time Format string, detects the formatting, and decodes it in to this object
        /// </summary>
        /// <param name="timestamp">APRS timestamp</param>
        public void Decode(string timestamp)
        {
            if (timestamp == null)
            {
                throw new ArgumentNullException();
            }
            else if (timestamp.Length != 7 && timestamp.Length != 8)
            {
                throw new ArgumentException("The given APRS timestamp is " + timestamp.Length + " characters long instead of the required 7-8.");
            }

            char timeIndicator = timestamp[6];

            if (timeIndicator == 'z' || timeIndicator == '/')
            {
                DecodeDHM(timestamp);
            }
            else if (timeIndicator == 'h')
            {
                DecodeHMS(timestamp);
            }
            else if (char.IsNumber(timeIndicator))
            {
                DecodeMDHM(timestamp);
            }
            else
            {
                throw new ArgumentException("timestamp was not a valid APRS format (did not have a valid Time Indicator character)");
            }
        }

        /// <summary>
        /// Takes a Day/Hours/Minutes formatted APRS timestamp and decodes it in to this object
        /// </summary>
        /// <param name="timestamp">Day/Hours/Minutes formatted APRS timestamp</param>
        private void DecodeDHM(string timestamp)
        {
            bool wasZuluTime = true;

            if (timestamp == null)
            {
                throw new ArgumentNullException();
            }
            else if (timestamp.Length != 7)
            {
                throw new ArgumentException("timestamp is not in APRS DHM datetime format. Length is " + timestamp.Length + " when it should be 7");
            }

            char timeIndicator = timestamp[6];

            if (timeIndicator == 'z')
            {
                wasZuluTime = true;
            }
            else if (timeIndicator == '/')
            {
                wasZuluTime = false;
            }
            else
            {
                throw new ArgumentException("timestamp is not in DHM format as time indicator is " + timeIndicator + " when it should be z or /");
            }

            DecodedType = wasZuluTime ? Type.DHMz : Type.DHMl;

            int numTryParse = 0;
            if (!int.TryParse(timestamp.Substring(0, 6), out numTryParse))
            {
                throw new ArgumentException("timestamp contained non-numeric values in the first 6 spaces: " + timestamp);
            }

            string dayStr = timestamp.Substring(0, 2);
            string hourStr = timestamp.Substring(2, 2);
            string minuteStr = timestamp.Substring(4, 2);

            int day = int.Parse(dayStr);
            int hour = int.Parse(hourStr);
            int minute = int.Parse(minuteStr);
            int year = 0;
            int month = 0;
            DateTime hint = wasZuluTime ? DateTime.Now.ToUniversalTime() : DateTime.Now;

            FindCorrectYearAndMonth(day, hint, out year, out month);
            DateTimeKind dtKind = wasZuluTime ? DateTimeKind.Utc : DateTimeKind.Local;

            dateTime = new DateTime(year, month, day, hour, minute, 0, dtKind);
        }

        /// <summary>
        /// /// <summary>
        /// If we're given a time in DHM, we need to find the correct month and year to fill in for DateTime.
        /// Assuming the day is in the past and the most recent occurance of that day in a month, this function finds the year and month.
        /// </summary>
        /// </summary>
        /// <param name="day">The day number to find</param>
        /// /// <param name="hint">A hint to the timeframe we're looking for. Generally, DateTime.Now.</param>
        /// <param name="year">The year in which the most recent occurance of that day number occured</param>
        /// <param name="month">The month in which the most recent occurance of that day number occured</param>
        private void FindCorrectYearAndMonth(
            int day, 
            DateTime hint, 
            out int year, 
            out int month)
        {
            if (day > 31 || day < 1)
            {
                throw new ArgumentOutOfRangeException("Day must be in range [1, 31], but the passed in day number was " + day);
            }

            int currYear = hint.Year;
            int currMonth = hint.Month;
            int currDay = hint.Day;

            // If that day number has already happened this month, we're done!
            if (day <= currDay)
            {
                year = currYear;
                month = currMonth;
            }
            else
            {
                DateTime itrDate = hint;
                while (itrDate.Day != day)
                {
                    itrDate = itrDate.AddDays(-1);
                }

                month = itrDate.Month;
                year = itrDate.Year;
            }
        }

        /// <summary>
        /// Takes an Hours/Minutes/Seconds formatted APRS timestamp and decodes it in to this object
        /// </summary>
        /// <param name="timestamp">Hours/Minutes/Seconds formatted APRS timestamp</param>
        private void DecodeHMS(string timestamp)
        {
            if (timestamp == null)
            {
                throw new ArgumentNullException();
            }
            else if (timestamp.Length != 7 || timestamp[6] != 'h')
            {
                throw new ArgumentException("timestamp is not in APRS HMS datetime format. Length is " + timestamp.Length + " when it should be 7 or format marker is " + timestamp[6] + " when it should be 'h'");
            }

            DecodedType = Type.HMS;

            string hourStr = timestamp.Substring(0, 2);
            string minuteStr = timestamp.Substring(2, 2);
            string secondStr = timestamp.Substring(4, 2);

            int hour = int.Parse(hourStr);
            int minute = int.Parse(minuteStr);
            int second = int.Parse(secondStr);
            int year = 0;
            int month = 0;
            int day = 0;
            DateTime hint = DateTime.Now.ToUniversalTime();

            FindCorrectDayMonthAndYear(
                hour,
                minute,
                second,
                hint,
                out day,
                out month,
                out year);

            DateTimeKind dtKind = DateTimeKind.Utc;
            dateTime = new DateTime(year, month, day, hour, minute, second, dtKind);
        }

        /// <summary>
        /// Given an hour, minute, and second, determines if this should be the same day as the hint (generally DateTime.Now) or the day before.
        /// </summary>
        /// <param name="hour">Hour of packet</param>
        /// <param name="minute">Minute of packet</param>
        /// <param name="second">Second of packet</param>
        /// <param name="hint">Usually DateTime.Now</param>
        /// <param name="day">Found day</param>
        /// <param name="month">Found month</param>
        /// <param name="year">Found year</param>
        public void FindCorrectDayMonthAndYear(
            int hour, 
            int minute, 
            int second, 
            DateTime hint, 
            out int day, 
            out int month, 
            out int year)
        {

            DateTime packetTime = new DateTime(hint.Year, hint.Month, hint.Day, hour, minute, second, hint.Kind);
            TimeSpan diff = hint.Subtract(packetTime);

            // allow for a few minutes of clock drift between receiver and transmitter
            if (diff.TotalMinutes < -5)
            {
                // assume it's from yesterday
                packetTime = packetTime.AddDays(-1);
            }

            day = packetTime.Day;
            month = packetTime.Month;
            year = packetTime.Year;
        }

        /// <summary>
        /// Takes a Month/Day/Hours/Minutes formmatted APRS timestamp and decodes it in to this object
        /// </summary>
        /// <param name="timestamp">Month/Day/Hours/Minutes forammted APRS timestamp</param>
        private void DecodeMDHM(string timestamp)
        {
            if (timestamp == null)
            {
                throw new ArgumentNullException();
            }
            else if (timestamp.Length != 8)
            {
                throw new ArgumentException("timestamp is not in APRS MDHM datetime format. Length is " + timestamp.Length + " when it should be 8");
            }

            DecodedType = Type.MDHM;
            
            string monthStr = timestamp.Substring(0, 2);
            string dayStr = timestamp.Substring(2, 2);
            string hourStr = timestamp.Substring(4, 2);
            string minuteStr = timestamp.Substring(6, 2);

            int month = int.Parse(monthStr);
            int day = int.Parse(dayStr);
            int hour = int.Parse(hourStr);
            int minute = int.Parse(minuteStr);
            int year = 0;
            DateTime hint = DateTime.Now.ToUniversalTime();


            FindCorrectYear(
                month,
                day,
                hour,
                minute,
                hint,
                out year);

            DateTimeKind dtKind = DateTimeKind.Utc;
            dateTime = new DateTime(year, month, day, hour, minute, 0 /* second */, dtKind);
        }

        /// <summary>
        /// Given a month, day, hour, and minute, determines the appropriate year (in the past as determiend by hint)
        /// </summary>
        /// <param name="month">Packet month</param>
        /// <param name="day">Packet day</param>
        /// <param name="hour">Packet hour</param>
        /// <param name="minute">Packet minute</param>
        /// <param name="hint">Usually DateTime.Now</param>
        /// <param name="year">Found year</param>
        private void FindCorrectYear(
            int month,
            int day,
            int hour,
            int minute,
            DateTime hint,
            out int year)
        {
            DateTime packet;
            int numRetries = 0;

            // Retries on four separate years to find a year
            // that could match the day and month (incl. leap)
            while (true)
            {
                try
                {
                    packet = new DateTime(
                        hint.Year - numRetries,
                        month,
                        day,
                        hour,
                        minute,
                        0,
                        hint.Kind);

                    // again, use five minute clock drift
                    if (hint.Subtract(packet).TotalMinutes > -5)
                    {
                        break;
                    }

                    ++numRetries;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    ++numRetries;
                    if (numRetries >= 4)
                    {
                        throw e;
                    }
                }
            }

            year = packet.Year;
        }
    }
}
