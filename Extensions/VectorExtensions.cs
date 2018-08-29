using System;
using SFML.System;

namespace BlackCoat
{
    /// <summary>
    /// Provides extension methods for all vector types.
    /// </summary>
    public static class VectorExtensions
    {
        // VECTOR MATH

        /// <summary>
        /// Retrieves the angle in degree for the viewer to look at a given point
        /// </summary>
        public static Single AngleTowards(this Vector2f from, Vector2f to)
        {
            return to.ToLocal(from).Angle();
        }

        /// <summary>
        /// Calculates the Angle in degree between the Vector and the X axis
        /// </summary>
        public static Single Angle(this Vector2f vector)
        {
            var angle = Math.Atan2(vector.Y, vector.X) * Constants.RAD_TO_DEG;
            if (angle < 0) angle += 360;
            return (Single)angle;
        }

        /// <summary>
        /// Multiplied Length based on angle and length of another Vector
        /// </summary>
        //public static Double DotProduct(this Vector2f vector, Single angle, Double length)
        //{
        //    return vector.Length() * length * Math.Cos(angle * Constants.DEG_TO_RAD);
        //}

        /// <summary>
        /// Multiplied Lengths of 2 Vectors
        /// </summary>
        public static Double DotProduct(this Vector2f vector, Vector2f axis)
        {
            return vector.X * axis.X + vector.Y * axis.Y;
        }

        /// <summary>
        /// Projected Length of the Vector to a surface represented by another Vector that will be normalized
        /// </summary>
        public static Double ProjectedLength(this Vector2f vector, Vector2f axis)
        {
            return vector.DotProduct(axis.Normalize());
        }

        /// <summary>
        /// Projected Length of the Vector to a surface of the provided angle
        /// </summary>
        //public static Double ProjectedLength(this Vector2f vector, Single angle)
        //{
        //    return vector.DotProduct(angle, 1);
        //}

        /// <summary>
        /// Projects a Vector onto another calculation its new Position
        /// </summary>
        public static Vector2f ProjectOnto(Vector2f vector, Vector2f axis)
        {
            axis = axis.Normalize();
            return (float)vector.DotProduct(axis) * axis;
        }


        /// <summary>
        /// Creates a new Vector pointing in the same direction as the original but with a fixed length of 1
        /// </summary>
        public static Vector2f Normalize(this Vector2f v)
        {
            return v / (float)v.Length();
        }

        /// <summary>
        /// Length of the Vector
        /// </summary>
        public static Double Length(this Vector2f v)
        {
            return Math.Sqrt(v.LengthSquared());
        }
        /// <summary>
        /// Squared Length of the Vector
        /// </summary>
        public static Single LengthSquared(this Vector2f v)
        {
            return v.X * v.X + v.Y * v.Y;
        }

        /// <summary>
        /// Creates a new Vector perpendicular to the original on the left side
        /// </summary>
        public static Vector2f FaceVector(this Vector2f v)
        {
            return new Vector2f(-v.Y, v.X);
        }

        /// <summary>
        /// Projects the Vector into a child coordinate system.
        /// </summary>
        /// <param name="root">Root of the child coordinate system inside the vectors current coordinate system</param>
        public static Vector2f ToLocal(this Vector2f v, Vector2f root)
        {
            return v - root;
        }

        /// <summary>
        /// Projects the Vector into a parent coordinate system.
        /// </summary>
        /// <param name="root">Root of the vectors current coordinate system</param>
        public static Vector2f ToGlobal(this Vector2f v, Vector2f root)
        {
            return v + root;
        }

        /// <summary>
        /// Calculates the distance between 2 Vectors
        /// </summary>
        /// <param name="other">The other Vector</param>
        public static Double DistanceBetween(this Vector2f v, Vector2f other)
        {
            return v.ToLocal(other).Length();
        }

        /// <summary>
        /// Calculates the distance between 2 Vectors but without calculating the square root
        /// </summary>
        /// <param name="other">The other Vector</param>
        public static Double DistanceBetweenSquared(this Vector2f v, Vector2f other)
        {
            return v.ToLocal(other).LengthSquared();
        }


        // CONVERSIONS

        public static Vector2i ToVector2i(this Vector2f v)
        {
            return new Vector2i((int)v.X, (int)v.Y);
        }

        public static Vector2u ToVector2u(this Vector2f v)
        {
            return new Vector2u((uint)v.X, (uint)v.Y);
        }

        public static Vector3f ToVector3f(this Vector2f v)
        {
            return new Vector3f(v.X, 0, v.Y);
        }

        public static Vector2f ToVector2f(this Vector3f v)
        {
            return new Vector2f(v.X, v.Z);
        }

        public static Vector2u ToVector2u(this Vector2i v)
        {
            return new Vector2u((uint)v.X, (uint)v.Y);
        }

        public static Vector2f ToVector2f(this Vector2i v)
        {
            return new Vector2f(v.X, v.Y);
        }

        public static Vector2i ToVector2i(this Vector2u v)
        {
            return new Vector2i((int)v.X, (int)v.Y);
        }

        public static Vector2f ToVector2f(this Vector2u v)
        {
            return new Vector2f(v.X, v.Y);
        }
    }
}