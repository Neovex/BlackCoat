using System;
using SFML.Window;
using SFML.System;

namespace BlackCoat.Tools
{
    public static class Direction
    {
        public const Single O  = 0f;// RIGHT
        public const Single SO = 45f;
        public const Single S  = 90f;// DOWN
        public const Single SW = 135f;
        public const Single W  = 180f;// LEFT
        public const Single NW = 225f;
        public const Single N  = 270f;// UP
        public const Single NO = 315f;

        /// <summary>
        /// Retrieves the angle for the viewer to look at a given point
        /// </summary>
        /// <param name="from">Position of the viewer</param>
        /// <param name="to">Position to look at</param>
        /// <returns>The look angle in degree</returns>
        public static Single LookAngle(Vector2f from, Vector2f to)
        {
            return (float)(Math.Atan2(to.Y - from.Y, to.X - from.X) * 180 / Math.PI);
        }
    }
}