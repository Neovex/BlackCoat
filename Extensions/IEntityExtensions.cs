﻿using System;
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
        /// <returns>The look angle in degree</returns>
        public static Single LookAt(this IEntity entity, IEntity target)
        {
            return LookAt(entity, target.Position);
        }

        /// <summary>
        /// Retrieves the angle for an entity to look at at a given point
        /// </summary>
        /// <param name="entity">"Viewer" Entity</param>
        /// <param name="target">Position to look at</param>
        /// <returns>The look angle in degree</returns>
        public static Single LookAt(this IEntity entity, Vector2f target)
        {
            return entity.Position.AngleTowards(target);
        }

        /// <summary>
        /// Retrieves the angle for an entity to look at the position of another entity
        /// </summary>
        /// <param name="entity">"Viewer" Entity</param>
        /// <param name="target">Entity to look at</param>
        /// <param name="offset">Offset from the target entity</param>
        /// <returns>The look angle in degree</returns>
        public static Single LookAt(this IEntity entity, IEntity target, Vector2f offset = default(Vector2f))
        {
            return LookAt(entity, target.Position, offset);
        }

        /// <summary>
        /// Retrieves the angle for an entity to look at at a given point
        /// </summary>
        /// <param name="entity">"Viewer" Entity</param>
        /// <param name="target">Position to look at</param>
        /// <param name="offset">Offset from the target position</param>
        /// <returns>The look angle in degree</returns>
        public static Single LookAt(this IEntity entity, Vector2f target, Vector2f offset = default(Vector2f))
        {
            return entity.Position.AngleTowards(new Vector2f(target.X + offset.X, target.Y + offset.Y));
        }
    }
}