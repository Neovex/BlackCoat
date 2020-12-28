using System;

namespace BlackCoat
{
    /// <summary>
    /// Contains all relevant engine constants.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Conversion constant to convert degrees into radians
        /// </summary>
        public const double DEG_TO_RAD = Math.PI / 180;

        /// <summary>
        /// Conversion constant to convert radians into degrees
        /// </summary>
        public const double RAD_TO_DEG = 180 / Math.PI;

        /// <summary>
        /// Defines the fault tolerance when projecting points as a distance in pixels
        /// </summary>
        public const float POINT_PROJECTION_TOLERANCE = 1.5f;

        /// <summary>
        /// Constant for a line break in SFML
        /// </summary>
        public const string NEW_LINE = "\n";

        /// <summary>
        /// Default separator for convenience
        /// </summary>
        public const char SEPERATOR = ';';
    }
}