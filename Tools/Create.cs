using System;
using System.Collections.Generic;
using System.Linq;
using SFML.System;
using SFML.Graphics;

namespace BlackCoat
{
    /// <summary>
    /// Helper class containing some convenient factory functions.
    /// </summary>
    public static class Create
    {
        /// <summary>
        /// Creates a new <see cref="SFML.System.Vector2f"/> based on a given direction and optional length.
        /// </summary>
        /// <param name="angle">Direction</param>
        /// <param name="length">Optional length</param>
        /// <returns>A new Vector</returns>
        public static Vector2f Vector2fFromAngle(float angle, float length = 1)
        {
            if (length == 0) return default(Vector2f);
            return new Vector2f((float)(Math.Cos(angle * Constants.DEG_TO_RAD)) * length,
                                (float)(Math.Sin(angle * Constants.DEG_TO_RAD)) * length);
        }

        /// <summary>
        /// Creates a new <see cref="SFML.System.Vector2f"/> based on a given direction and optional length.
        /// This Method is faster than VectorFromAngle but less precise. <seealso cref="MathHelper"/>
        /// </summary>
        /// <param name="angle">Direction</param>
        /// <param name="length">Optional length</param>
        /// <returns>A new Vector</returns>
        public static Vector2f Vector2fFromAngleLookup(float angle, float length = 1)
        {
            if (length == 0) return default(Vector2f);
            return new Vector2f(MathHelper.Cos(angle) * length, MathHelper.Sin(angle) * length);
        }

        /// <summary>
        /// Creates a new <see cref="SFML.System.Vector2f"/> with equal x and y components.
        /// </summary>
        /// <param name="value">Value for x AND y component</param>
        /// <returns>A new Vector</returns>
        public static Vector2f Vector2f(float value)
        {
            return new Vector2f(value, value);
        }

        /// <summary>
        /// Creates a new <see cref="SFML.System.Vector2i"/> with equal x and y components.
        /// </summary>
        /// <param name="value">Value for x AND y component</param>
        /// <returns>A new Vector</returns>
        public static Vector2i Vector2i(int value)
        {
            return new Vector2i(value, value);
        }

        /// <summary>
        /// Creates a new <see cref="SFML.System.Vector2u"/> with equal x and y components.
        /// </summary>
        /// <param name="value">Value for x AND y component</param>
        /// <returns>A new Vector</returns>
        public static Vector2u Vector2u(uint value)
        {
            return new Vector2u(value, value);
        }

        /// <summary>
        /// Enumerates the specified items.
        /// </summary>
        /// <typeparam name="T">Item Type</typeparam>
        /// <param name="items">The items.</param>
        /// <returns>Enumeration of Items</returns>
        public static IEnumerable<T> Enumerable<T>(params T[] items) => items ?? throw new ArgumentException("No items");

        /// <summary>
        /// Creates a human readable string for the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Id string</returns>
        public static string IdString(IEntity entity)
        {
            if (entity == null) return null;
            var name = String.Empty;
            if (!String.IsNullOrEmpty(entity.Name)) name = $"\"{entity.Name}\" ";
            return $"{name}{entity.GetType().Name} Pos: {entity.Position.X} x {entity.Position.Y}";
        }
    }
}