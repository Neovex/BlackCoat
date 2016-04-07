using System;
using SFML.System;

namespace BlackCoat
{
    /// <summary>
    /// Provides extension methods for all vector types.
    /// </summary>
    public static class VectorExtensions
    {
        /// <summary>
        /// Retrieves the angle for the viewer to look at a given point
        /// </summary>
        /// <param name="from">Position of the viewer</param>
        /// <param name="to">Position to look at</param>
        /// <returns>The look angle in degree</returns>
        public static Single AngleTowards(this Vector2f from, Vector2f to)
        {
            return (float)(Math.Atan2(to.Y - from.Y, to.X - from.X) * 180 / Math.PI);
        }

        public static Vector2i ToVector2i(this Vector2f v)
        {
            return new Vector2i((int)v.X, (int)v.Y);
        }

        public static Vector2f ToVector2f(this Vector2i v)
        {
            return new Vector2f(v.X, v.Y);
        }

        public static Vector2f ToVector2f(this Vector2u v)
        {
            return new Vector2f(v.X, v.Y);
        }
    }
}