using System;
using SFML.System;
using BlackCoat.Entities;

namespace BlackCoat
{
    /// <summary>
    /// Provides extension methods for all entity types.
    /// </summary>
    public static class EntityExtensions
    {
        /// <summary>
        /// Retrieves the angle for an entity to look at the position of another entity
        /// </summary>
        /// <param name="target">Entity to look at</param>
        /// <param name="offset">Offset from the target entity</param>
        public static void LookAt(this IEntity entity, IEntity target, Vector2f offset = default)
        {
            entity.Rotation = entity.Position.AngleTowards(target.Position + offset);
        }

        /// <summary>
        /// Retrieves the angle for an entity to look at at a given point
        /// </summary>
        /// <param name="target">Position to look at</param>
        public static void LookAt(this IEntity entity, Vector2f target)
        {
            entity.Rotation = entity.Position.AngleTowards(target);
        }

        /// <summary>
        /// Creates a human readable string from an entity.
        /// </summary>
        /// <returns>Id string</returns>
        public static string CreateIdString(this IEntity entity)
        {
            if (entity == null) return null;
            var name = String.Empty;
            if (!String.IsNullOrEmpty(entity.Name)) name = $" \"{entity.Name}\"";
            return $"{entity.GetType().Name}{name} ({entity.Position.X} x {entity.Position.Y})";
        }

        /// <summary>
        /// Moves an entity by the specified distance
        /// </summary>
        /// <param name="distance">Distance to move</param>
        public static void Translate(this IEntity entity, Vector2f distance)
        {
            entity.Position += distance;
        }

        /// <summary>
        /// Moves an entity by the specified distance
        /// </summary>
        /// <param name="x">Distance to move on the X axis</param>
        /// <param name="y">Distance to move on the Y axis</param>
        public static void Translate(this IEntity entity, float x, float y)
        {
            entity.Position += new Vector2f(x, y);
        }

        /// <summary>
        /// Rotates an entity by the specified angle
        /// </summary>
        /// <param name="angle">Angle to rotate</param>
        public static void Rotate(this IEntity entity, float angle)
        {
            entity.Rotation += angle;
        }


        /// <summary>
        /// Scales an entity by the specified modifier
        /// </summary>
        /// <param name="scale">Scale modifier</param>
        public static void Resize(this IEntity entity, Vector2f modifier)
        {
            entity.Scale += modifier;
        }

        /// <summary>
        /// Scales an entity by the specified modifiers
        /// </summary>
        /// <param name="x">X Scale modifier</param>
        /// <param name="y">Y Scale modifier</param>
        public static void Resize(this IEntity entity, float x, float y)
        {
            entity.Scale += new Vector2f(x, y);
        }
    }
}