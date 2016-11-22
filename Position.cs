using System;
using GeoCoordinatePortable;

namespace APaRSer
{
    class Position
    {
        public GeoCoordinate Coordinates = new GeoCoordinate(0, 0);
        public char SymbolTableIdentifier = '.';
        public char SymbolCode = '\\';
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
            // Count ambiguity

            // Convert coordinates from degrees, minutes, tenths of minutes

            throw new NotImplementedException();
        }

        /// <summary>
        /// Decode the longitude section of the coordinate string
        /// </summary>
        /// <param name="coords">APRS encoded latitude coordinates</param>
        /// <returns>Decimal longitude</returns>
        private double DecodeLongitude(string coords)
        {
            // Convert coordinates from egrees, minutes, tenths of minutes

            throw new NotImplementedException();
        }
    }
}
