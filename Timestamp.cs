using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APaRSer
{
    public class Timestamp
    {
        public DateTime dateTime;
        public bool WasZuluTime;

        /// <summary>
        /// Initializes this object by decoding the given timestamp in to it
        /// </summary>
        /// <param name="timestamp">APRS timestamp</param>
        public Timestamp(string timestamp)
        {
            Decode(timestamp);
        }

        /// <summary>
        /// Initializes a timestamp to the current time
        /// </summary>
        public Timestamp() { }

        public Timestamp(DateTime dt)
        {
            dateTime = dt;
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
            else if (timestamp.Length != 7)
            {
                throw new ArgumentException("The given APRS timestamp is " + timestamp.Length + " characters long instead of the required 7.");
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
            if (timestamp == null)
            {
                throw new ArgumentNullException();
            }
            else if (timestamp.Length != 7)
            {
                throw new ArgumentException("timestamp is not in APRS datetime format. Length is " + timestamp.Length + " when it should be 7");
            }

            char timeIndicator = timestamp[6];

            if (timeIndicator == 'z')
            {
                WasZuluTime = true;
            }
            else if (timeIndicator != '/')
            {
                WasZuluTime = false;
            }
            else
            {
                throw new ArgumentException("timestamp is not in DHM format as time indicator is " + timeIndicator + " when it should be z or /");
            }

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

            FindCorrectYearAndMonth(day, DateTime.Now, out year, out month);
            DateTimeKind dtKind = WasZuluTime ? DateTimeKind.Utc : DateTimeKind.Local;

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
        private void FindCorrectYearAndMonth(int day, DateTime hint, out int year, out int month)
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Takes a Month/Day/Hours/Minutes formmatted APRS timestamp and decodes it in to this object
        /// </summary>
        /// <param name="timestamp">Month/Day/Hours/Minutes forammted APRS timestamp</param>
        private void DecodeMDHM(string timestamp)
        {
            throw new NotImplementedException();
        }
    }
}
