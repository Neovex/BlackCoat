using System;
using SFML.System;

namespace BlackCoat
{
    /// <summary>
    /// Provides extension methods for all vector types.
    /// </summary>
    public static class VectorExtensions
    {
        // MATH

        /// <summary>
        /// Retrieves the angle for the viewer to look at a given point
        /// </summary>
        /// <param name="from">Position of the viewer</param>
        /// <param name="to">Position to look at</param>
        /// <returns>The look angle in degree</returns>
        public static Single AngleTowards(this Vector2f from, Vector2f to)
        {
            return (Single)(Math.Atan2(to.Y - from.Y, to.X - from.X) * Constants.RAD_TO_DEG);
        }

        public static Single Angle(this Vector2f vector)
        {
            return (Single)(Math.Atan2(vector.X, vector.Y) * Constants.RAD_TO_DEG);
        }

        /// <summary>
        /// Projected Length of the Vector to a surface of the provided angle
        /// </summary>
        public static Double DotProduct(this Vector2f vector, Single angle)
        {
            return vector.Length() * Math.Cos(angle * Constants.DEG_TO_RAD);
        }

        /// <summary>
        /// Projected Length of the Vector to a surface represented by a vector
        /// </summary>
        public static Double DotProduct(this Vector2f vector, Vector2f axis)
        {
            return DotProduct(vector, axis.Angle());
        }

        /// <summary>
        /// Length of the Vector
        /// </summary>
        public static Double Length(this Vector2f v)
        {
            return Math.Sqrt(v.X * v.X + v.Y * v.Y);
        }

        /// <summary>
        /// Calculates a new Vector based on a given direction and optional length.
        /// </summary>
        /// <param name="angle">Direction</param>
        /// <param name="length">Optional length</param>
        /// <returns>Movement Vector - multiply with a distance/speed to </returns>
        public static Vector2f VectorFromAngle(float angle, float length = 1)
        {
            return new Vector2f((float)(Math.Cos(angle * Constants.DEG_TO_RAD)) * length,
                                (float)(Math.Sin(angle * Constants.DEG_TO_RAD)) * length);
        }


        // CONVERSIONS

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