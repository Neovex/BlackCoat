using System;
using SFML.System;

namespace BlackCoat
{
    /// <summary>
    /// Provides extension methods for .NET types
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Calculates a movement Vector based on a given direction.
        /// </summary>
        /// <param name="direction">Direction angle</param>
        /// <returns>Movement Vector - multiply with a distance/speed to </returns>
        public static Vector2f ToMovementVector(this float direction)
        {
            var pos = new Vector2f();
            pos.X = (float)(Math.Cos(direction * Constants.DEG_TO_RAD));
            pos.Y = (float)(Math.Sin(direction * Constants.DEG_TO_RAD));
            return pos;
        }
    }
}