using System;
using BlackCoat.Entities;
using SFML.System;

namespace BlackCoat
{
    /// <summary>
    /// Provides extension methods for all entity types.
    /// </summary>
    public static class IEntityExtensions
    {
        /// <summary>
        /// Retrieves the angle for an entity to look at the position of another entity
        /// </summary>
        /// <param name="entity">"Viewer" Entity</param>
        /// <param name="target">Entity to look at</param>
        /// <param name="offset">Offset from the target entity</param>
        public static void LookAt(this IEntity entity, IEntity target, Vector2f offset = default(Vector2f))
        {
            entity.Rotation = entity.Position.AngleTowards(target.Position + offset);
        }

        /// <summary>
        /// Retrieves the angle for an entity to look at at a given point
        /// </summary>
        /// <param name="entity">"Viewer" Entity</param>
        /// <param name="target">Position to look at</param>
        /// <param name="offset">Offset from the target position</param>
        public static void LookAt(this IEntity entity, Vector2f target, Vector2f offset = default(Vector2f))
        {
            entity.Rotation = entity.Position.AngleTowards(target + offset);
        }

        /// <summary>
        /// Creates a human readable string from an entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Id string</returns>
        public static string CreateIdString(this IEntity entity)
        {
            if (entity == null) return null;
            var name = String.Empty;
            if (!String.IsNullOrEmpty(entity.Name)) name = $"\"{entity.Name}\" ";
            return $"{name}{entity.GetType().Name} Pos: {entity.Position.X} x {entity.Position.Y}";
        }
    }
}