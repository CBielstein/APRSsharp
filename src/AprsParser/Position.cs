using System;
using GeoCoordinatePortable;
using System.Text;
using System.Globalization;

namespace AprsSharp.Parsers.Aprs
{
    public class Position
    {
        public GeoCoordinate Coordinates = new GeoCoordinate(0, 0);
        public char SymbolTableIdentifier = '\\';
        public char SymbolCode = '.';
        public int Ambiguity = 0;

        /// <summary>
        /// Decodes from LAT/LONG coordinates
        /// </summary>
        /// <param name="coords">Decodes from LAT/LONG coordinates</param>
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
        /// <param name="amb">Ambiguity to use on the coordinates</param>
        public Position(GeoCoordinate coords, char table, char symbol, int amb)
        {
            Coordinates = coords;
            SymbolTableIdentifier = table;
            SymbolCode = symbol;
            Ambiguity = amb;
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
        /// Initializes the Position object given a GeoCoordinate object
        /// </summary>
        /// <param name="coords">Coordinates to encode</param>
        public Position(GeoCoordinate coords) 
        {
            Coordinates = coords;
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
        /// Decode from Maidenhead Location System gridsquare to a point in the middle of the gridsquare
        /// </summary>
        /// <param name="gridsqure">4 or 6 char Maidenhead gridsquare, optionally followed by symbol table and identifier</param>
        public void DecodeMaidenhead(string gridsquare)
        {
            if (gridsquare == null)
            {
                throw new ArgumentNullException();
            }
            else if (gridsquare.Length != 4 && 
                     gridsquare.Length != 6 &&
                     gridsquare.Length != 8 &&
                     gridsquare.Length != 10)

            {
                throw new ArgumentException("The given Maidenhead Location System gridsquare was " + gridsquare.Length + " characters length when a length of 4, 6, 8, or 10 characters was expected (including an optional two char symbol table and code).");
            }

            string trimmedGridsquare = gridsquare;

            // If the symbol table identifier is not a letter or digit, assume it's the table.
            // Because if it is a letter or digit, we'll assume there is no symbol information.
            // Then trim it off for passing through to the decode functions
            if (!char.IsLetterOrDigit(gridsquare[gridsquare.Length - 2]))
            {
                SymbolTableIdentifier = gridsquare[gridsquare.Length - 2];
                SymbolCode = gridsquare[gridsquare.Length - 1];

                trimmedGridsquare = gridsquare.Substring(0, gridsquare.Length - 2);
            }

            double latitude = DecodeFromGridsquare(trimmedGridsquare, CoordinateSystem.Latitude);
            double longitude = DecodeFromGridsquare(trimmedGridsquare, CoordinateSystem.Longitude);
            Coordinates = new GeoCoordinate(latitude, longitude);
        }

        /// <summary>
        /// Specifies whether a function is referring to latitude or longitude during Maidenhead gridsquare encode/decode
        /// </summary>
        public enum CoordinateSystem
        {
            Latitude,
            Longitude,
        }

        /// <summary>
        /// Given a maidenhead gridsquare, converts to decimal latitude or longitude
        /// </summary>
        /// <param name="gridsquare">A Maidenhead Location System gridsquare</param>
        /// <param name="coordinateDecode">Latitude or longitude for decode</param>
        /// <returns>A double representing latitude [-180.0, 180.0] or longitude [-90.0, 90.0]</returns>
        private double DecodeFromGridsquare(string gridsquare, CoordinateSystem coordinateDecode)
        {
            if (gridsquare == null)
            {
                throw new ArgumentNullException();
            }
            else if (gridsquare.Length != 4 &&
                     gridsquare.Length != 6 &&
                     gridsquare.Length != 8)
            {
                throw new ArgumentException("The given Maidenhead Location System gridsquare was " + gridsquare.Length + " characters length when a length of 4 or 6 characters was expected.");
            }
            else if (coordinateDecode != CoordinateSystem.Latitude &&
                     coordinateDecode != CoordinateSystem.Longitude)
            {
                throw new ArgumentOutOfRangeException("coordinateDecode must be CoordinateSystem.Latitude or CoordinateSystem.Longitude. Given value was " + coordinateDecode);
            }

            double coord = 0;
            double gridsquareSize = 0;

            // Chars alternatingly refer to longitude and latitude
            int index = (coordinateDecode == CoordinateSystem.Latitude) ? 1 : 0;

            // Longitude scales most values by 2 over latitude
            int multiplier = (coordinateDecode == CoordinateSystem.Longitude) ? 2 : 1;

            gridsquare = gridsquare.ToUpperInvariant();

            if (gridsquare.Length >= 2)
            {
                if (gridsquare[index] < 'A' || gridsquare[index] > 'R')
                {
                    throw new ArgumentException("Gridsquare should use A-R for first pair of characters. The given string was " + gridsquare);
                }

                gridsquareSize = 10 * multiplier;
                coord += gridsquareSize * (gridsquare[index] - 'A');
                index += 2;
            }
 
            if (gridsquare.Length >= 4)
            {
                if (!char.IsDigit(gridsquare[index]))
                {
                    throw new ArgumentException("Gridsquare should use 0-9 for second pair of characters. The given string was " + gridsquare);
                }

                gridsquareSize = multiplier;
                coord += gridsquareSize * char.GetNumericValue(gridsquare[index]);
                index += 2;
            }

            if (gridsquare.Length >= 6)
            {
                if (gridsquare[index] < 'A' || gridsquare[index] > 'X')
                {
                    throw new ArgumentException("Gridsquare should use A-X for third pair of characters. The given string was " + gridsquare);
                }

                gridsquareSize = (2.5 * multiplier) / 60.0;
                coord += ((2.5 * multiplier) * (gridsquare[index] - 'A')) / 60.0;
                index += 2;
            }

            if (gridsquare.Length >= 8)
            {
                if (!char.IsDigit(gridsquare[index]))
                {
                    throw new ArgumentException("Gridsquare should use 0-9 for fourth pair of characters. The given string was " + gridsquare);
                }

                gridsquareSize = (0.25 * multiplier) / 60.0;
                coord += ((0.25 * multiplier) * char.GetNumericValue(gridsquare[index])) / 60.0;
                index += 2;
            }

            // center the coordinate in the grid
            coord += gridsquareSize / 2.0;
            return coord - (90.0 * multiplier);
        }

        /// <summary>
        /// Encodes the location as a gridsquare from the coordinates on this object
        /// </summary>
        /// <param name="length">Max number of characters to use in gridsquare encoding, must be {4, 6, 8}. Ambiguity will subtract from this.</param>
        /// <param name="appendSymbol">If true, appends the symbol to the end of the string</param>
        /// <returns>A string representation of the Maidenhead gridsquare</returns>
        public string EncodeGridsquare(int length, bool appendSymbol)
        {
            if (length != 4 &&
                length != 6 &&
                length != 8)
            {
                throw new ArgumentException("Length should be 4, 6, or 8. Given value was " + length);
            }
            else if (Coordinates == null)
            {
                throw new NullReferenceException("Coordinates were null.");
            }
            else if (Ambiguity != 0 &&
                     Ambiguity != 2 &&
                     Ambiguity != 4 &&
                     Ambiguity != 6)
            {
                throw new ArgumentException("Ambiguity must be {0, 2, 4, 6}, but is " + Ambiguity);
            }
            else if (Ambiguity > length - 4)
            {
                throw new ArgumentException("Final length of the string must be at least 4. length - Ambiguity >= 4, but length is "
                                            + length + "and Ambiguity is " + Ambiguity);
            }

            string gridsquare = string.Empty;

            string longitude = EncodeGridsquareElement(Coordinates.Longitude, CoordinateSystem.Longitude, (length - Ambiguity) / 2);
            string latitude = EncodeGridsquareElement(Coordinates.Latitude, CoordinateSystem.Latitude, (length - Ambiguity) / 2);

            for (int i = 0; i < length - Ambiguity; ++i)
            {
                if (i % 2 == 0)
                {
                    gridsquare += longitude[i / 2];
                }
                else
                {
                    gridsquare += latitude[i / 2];
                }
            }

            if (appendSymbol)
            {
                gridsquare += SymbolTableIdentifier;
                gridsquare += SymbolCode;
            }

            return gridsquare;
        }

        /// <summary>
        /// Encode either the latitude or longitude part of a Maidenhead gridsquare
        /// </summary>
        /// <param name="coords">The coordinates to use</param>
        /// <param name="coordinateEncode">Either latitude or longitude encoding</param>
        /// <param name="length">The number of chars for this encoding (should be half the total length of the gridsquare as this is only lat or long)</param>
        /// <returns>A string of the lat or long for the gridsquare encoding</returns>
        private string EncodeGridsquareElement(double coords, CoordinateSystem coordinateEncode, int length)
        {
            if (coordinateEncode != CoordinateSystem.Latitude &&
                coordinateEncode != CoordinateSystem.Longitude)
            {
                throw new ArgumentOutOfRangeException("coordinateEncode must be CoordinateSystem.Latitude or CoordinateSystem.Longitude. Given value was " + coordinateEncode);
            }
            else if (coordinateEncode == CoordinateSystem.Longitude && Math.Abs(coords) > 180)
            {
                throw new ArgumentOutOfRangeException("Longitude coordinates must be inside [-180, 180]");
            }
            else if (coordinateEncode == CoordinateSystem.Latitude && Math.Abs(coords) > 90)
            {
                throw new ArgumentOutOfRangeException("Longitude coordinates must be inside [-90, 90]");
            }

            string encoded = string.Empty;
            int charIndex = 0;
            double stepDivisor = 0;

            // Longitude scales most values by 2 over latitude
            int multiplier = (coordinateEncode == CoordinateSystem.Longitude) ? 2 : 1;

            // no negatives here
            double shiftedCoords = coords + (multiplier * 90.0);

            if (length >= 1)
            {
                stepDivisor = 10.0 * multiplier;
                charIndex = (int)(shiftedCoords / stepDivisor);
                encoded += char.ConvertFromUtf32('A' + charIndex);

                shiftedCoords %= stepDivisor;
            }
 
            if (length >= 2)
            {
                stepDivisor = 1.0 * multiplier;
                charIndex = (int)(shiftedCoords / stepDivisor);
                encoded += charIndex.ToString();

                shiftedCoords %= stepDivisor;
            }

            // moving in to minutes territory
            shiftedCoords *= 60.0;

            if (length >= 3)
            {
                stepDivisor = 2.5 * multiplier;
                charIndex = (int)((shiftedCoords) / stepDivisor);
                encoded += char.ConvertFromUtf32('A' + charIndex);

                shiftedCoords %= stepDivisor;
            }

            if (length >= 4)
            {
                stepDivisor = 0.25 * multiplier;
                charIndex = (int)(shiftedCoords  / stepDivisor);
                encoded += charIndex.ToString();

                shiftedCoords %= stepDivisor;
            }

            return encoded;
        }

        /// <summary>
        /// Decode the latitude section of the coordinate string
        /// </summary>
        /// <param name="coords">APRS encoded latitude coordinates</param>
        /// <returns>Decimal latitude</returns>
        public double DecodeLatitude(string coords)
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
        public string EnforceAmbiguity(string coords, int nEnforceAmbiguity)
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
        public int CountAmbiguity(string coords)
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
        public double DecodeLongitude(string coords)
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

        /// <summary>
        /// Encodes with the values set on the fields using lat/long
        /// </summary>
        /// <returns>A string encoding of an APRS position (lat/long)</returns>
        public string Encode()
        {
            return EncodeLatitude() + 
                SymbolTableIdentifier +
                EncodeLongitude() +
                SymbolCode;
        }

        /// <summary>
        /// Encodes latitude position, enforcing ambiguity
        /// Really just calls through to EncodeCoordinates, but this is easier to test
        /// with PrivateObject
        /// </summary>
        /// <returns>Encoded APRS latitude position</returns>
        public string EncodeLatitude()
        {
            return EncodeCoordinates(EncodeType.Latitude);
        }

        /// <summary>
        /// Encodes longitude position, enforcing ambiguity
        /// Really just calls through to EncodeCoordinates, but this is easier to test
        /// with PrivateObject
        /// </summary>
        /// <returns>Encoded APRS longitude position</returns>
        public string EncodeLongitude()
        {
            return EncodeCoordinates(EncodeType.Longitude);
        }

        /// <summary>
        /// Used to specify the correct encoding type for EncodeCoordinates
        /// </summary>
        private enum EncodeType
        {
            Latitude,
            Longitude,
        }

        /// <summary>
        /// Encodes latitude or longitude position, enforcing ambiguity
        /// </summary>
        /// <param name="type">EncodeType to encode: Latitude or Longitude</param>
        /// <returns>String of APRS latitude or longitude position</returns>
        private string EncodeCoordinates(EncodeType type)
        {
            double coords;
            char direction;
            string decimalFormat;

            if (type == EncodeType.Latitude)
            {
                coords = Coordinates.Latitude;
                direction = coords < 0 ? 'S' : 'N';
                decimalFormat = "D2";
            }
            else if (type == EncodeType.Longitude)
            {
                coords = Coordinates.Longitude;
                direction = coords > 0 ? 'E' : 'W';
                decimalFormat = "D3";
            }
            else
            {
                throw new ArgumentException("Invalid EncodeType: " + type);
            }

            coords = Math.Abs(coords);

            int degrees = (int)Math.Floor(coords);
            int minutes = (int)Math.Floor((coords - degrees) * 60);
            int hundMinutes = (int)Math.Round((((coords - degrees) * 60) - minutes) * 100);

            string encoded = degrees.ToString(decimalFormat, CultureInfo.InvariantCulture) + 
                minutes.ToString("D2", CultureInfo.InvariantCulture) +
                '.' +
                hundMinutes.ToString("D2", CultureInfo.InvariantCulture) +
                direction;

            encoded = EnforceAmbiguity(encoded, Ambiguity);

            return encoded;
        }
    }
}
