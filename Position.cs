using System;
using GeoCoordinatePortable;
using System.Text;

namespace APaRSer
{
    public class Position
    {
        public GeoCoordinate Coordinates = new GeoCoordinate(0, 0);
        public char SymbolTableIdentifier = '\\';
        public char SymbolCode = '.';
        public int Ambiguity = 0;

        public Position(string coords)
        {
            Decode(coords);
        }

        /// <summary>
        /// Initializes the Position object given a GeoCoordinate object
        /// </summary>
        /// <param name="coords">Coordinates to encode</param>
        /// <param name="table">The primary or secondary symbole table</param>
        /// <param name="symbol">The APRS symbol code from the table given</param>
        public Position(GeoCoordinate coords, char table, char symbol)
        {
            Coordinates = coords;
            SymbolTableIdentifier = table;
            SymbolCode = symbol;
        }

        /// <summary>
        /// Defaults to the default null position and unknown/indeterminate position symbol
        /// </summary>
        public Position() { }

        /// <summary>
        /// Takes an encoded APRS coordinate string and uses it to initialize to GeoCoordinate
        /// </summary>
        /// <param name="coords">A string of APRS encoded coordinates</param>
        public void Decode(string coords)
        {
            if (coords == null)
            {
                throw new ArgumentNullException();
            }
            else if (coords.Length != 19)
            {
                throw new ArgumentException("The given APRS coordinates is " + coords.Length + " coords long instead of the expected 19: " + coords);
            }

            Ambiguity = 0;
            double latitude = DecodeLatitude(coords.Substring(0, 8));
            double longitude = DecodeLongitude(coords.Substring(9, 9));

            SymbolTableIdentifier = coords[8];
            SymbolCode = coords[18];
            Coordinates = new GeoCoordinate(latitude, longitude);
        }

        /// <summary>
        /// Decode the latitude section of the coordinate string
        /// </summary>
        /// <param name="coords">APRS encoded latitude coordinates</param>
        /// <returns>Decimal latitude</returns>
        private double DecodeLatitude(string coords)
        {
            // Ensure latitude is well formatted
            if (coords == null)
            {
                throw new ArgumentNullException();
            }

            string upperCoords = coords.ToUpperInvariant();
            if (upperCoords.Length != 8)
            {
                throw new ArgumentException("Latitude coordinates should be 8 characters. Given string was " + upperCoords.Length);
            }

            char direction = upperCoords[7];
            if (upperCoords[7] != 'N' && upperCoords[7] != 'S')
            {
                throw new ArgumentException("Coordinates should end in N or S. Given string ended in " + direction);
            }
            if (upperCoords[4] != '.')
            {
                throw new ArgumentException("Coordinates should have '.' at index 4. Given string had " + upperCoords[4]);
            }

            // Count ambiguity and ensure no out of place spaces
            Ambiguity = CountAmbiguity(upperCoords);

            // replace spaces with zeros
            string modifiedCoords = upperCoords.Replace(' ', '0');

            // Break out strings and convert values
            string degreesStr = modifiedCoords.Substring(0, 2);
            string minutesStr = modifiedCoords.Substring(2, 2);
            string hundMinutesStr = modifiedCoords.Substring(5, 2);

            double degrees = double.Parse(degreesStr);
            if (degrees < 00 || degrees > 90)
            {
                throw new ArgumentOutOfRangeException("Degrees must be in range [00,90]. Found: " + degrees + " in coord string " + modifiedCoords + " from given coord string " + coords);
            }

            double minutes = double.Parse(minutesStr);
            double hundMinutes = double.Parse(hundMinutesStr);

            double retval = degrees + (((minutes + (hundMinutes / 100)) / 60.0));

            if (direction == 'S')
            {
                retval *= -1;
            }

            // limit significant figures
            return Math.Round(retval, 4);
        }

        /// <summary>
        /// Converts a string to have the given amount of ambiguity
        /// </summary>
        /// <param name="coords">String to convert</param>
        /// <param name="nEnforceAmbiguity">Amount of ambiguity to emplace</param>
        /// <returns></returns>
        private string EnforceAmbiguity(string coords, int nEnforceAmbiguity)
        {
            if (coords == null)
            {
                throw new ArgumentNullException();
            }
            else if (nEnforceAmbiguity > coords.Length - 2)
            {
                throw new ArgumentOutOfRangeException("Only " + (coords.Length - 2) + " numbers can be filtered, but " + nEnforceAmbiguity + " were requested for string " + coords);
            }

            StringBuilder sb = new StringBuilder(coords);
            int remainingAmbiguity = nEnforceAmbiguity;

            for (int i = coords.Length - 2; i >= 0 && remainingAmbiguity > 0; --i)
            {
                if (i == coords.Length - 4)
                {
                    continue;
                }

                sb[i] = ' ';
                --remainingAmbiguity;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Counts the number of spaces from right to left and returns the number for ambiguity.
        /// Also ensures there are no spaces left of a digit, which is an invalid APRS ambiguity syntax.
        /// </summary>
        /// <param name="coords">An APRS latitude or longitude string</param>
        /// <returns></returns>
        private int CountAmbiguity(string coords)
        {
            int ambiguity = 0;
            bool foundDigit = false;
            for (int i = coords.Length - 2; i >= 0; --i)
            {
                // skip the period
                if (i == coords.Length - 4)
                {
                    continue;
                }
                else if (coords[i] == ' ')
                {
                    if (foundDigit)
                    {
                        throw new ArgumentException("Coordinates shoud only have spaces growing from right. Error on string: " + coords);
                    }
                    else
                    {
                        ++ambiguity;
                    }
                }
                else
                {
                    foundDigit = true;
                }
            }

            return ambiguity;
        }

        /// <summary>
        /// Decode the longitude section of the coordinate string
        /// </summary>
        /// <param name="coords">APRS encoded latitude coordinates</param>
        /// <returns>Decimal longitude</returns>
        private double DecodeLongitude(string coords)
        {
            if (coords == null)
            {
                throw new ArgumentNullException();
            }

            string upperCoords = coords.ToUpperInvariant();
            if (upperCoords.Length != 9)
            {
                throw new ArgumentException("Longitude format calls for 9 characters. Given string is " + coords.Length);
            }

            char direction = upperCoords[8];
            if (direction != 'W' && direction != 'E')
            {
                throw new ArgumentException("Coordinates should end in E or W. Given string ended in " + direction);
            }
            else if (upperCoords[5] != '.')
            {
                throw new ArgumentException("Coordinates should have '.' at index 5. Given string had " + upperCoords[5]);
            }

            // This ensures no spaces are out of place
            CountAmbiguity(upperCoords);

            // Enforce ambiguity from latitude and replace spaces
            string modifiedCoords = EnforceAmbiguity(upperCoords, Ambiguity);
            modifiedCoords = modifiedCoords.Replace(' ', '0');

            // Break out strings and convert values
            string degreesStr = modifiedCoords.Substring(0, 3);
            string minutesStr = modifiedCoords.Substring(3, 2);
            string hundMinuteStr = modifiedCoords.Substring(6, 2);

            double degrees = double.Parse(degreesStr);
            if (degrees < 000 || degrees > 180)
            {
                throw new ArgumentOutOfRangeException("Degrees longitude must be in range [000, 180]. Found: " + degrees + " in coord string " + modifiedCoords + " from given coord string " + coords);
            }

            double minutes = double.Parse(minutesStr);
            double hundMinutes = double.Parse(hundMinuteStr);

            double retval = degrees + (((minutes + (hundMinutes / 100)) / 60.0));

            if (direction == 'W')
            {
                retval *= -1;
            }

            // limit significant figures
            return Math.Round(retval, 4);
        }
    }
}
