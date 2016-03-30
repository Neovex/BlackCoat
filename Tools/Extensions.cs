using System;
using SFML.Window;
using SFML.System;
using BlackCoat.Entities;

namespace BlackCoat
{
    /// <summary>
    /// Provides extension methods for calculating angles based on positions and entities
    /// </summary>
    public static class Extensions
    {
        // Methods #########################################################################
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