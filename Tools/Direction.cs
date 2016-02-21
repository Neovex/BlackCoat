using System;
using SFML.Window;
using SFML.System;
using BlackCoat.Entities;

namespace BlackCoat.Tools
{
    /// <summary>
    /// Provides constants and vector extension methods for calculating angles based on positions and entities
    /// </summary>
    public static class Direction
    {
        // Constants #######################################################################
        public const Single O  = 0f;// RIGHT
        public const Single SO = 45f;
        public const Single S  = 90f;// DOWN
        public const Single SW = 135f;
        public const Single W  = 180f;// LEFT
        public const Single NW = 225f;
        public const Single N  = 270f;// UP
        public const Single NO = 315f;


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
    }
}