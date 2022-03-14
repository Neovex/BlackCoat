using SFML.Graphics;
using SFML.System;

namespace BlackCoat
{
    public static class ViewExtensions
    {
        /// <summary>
        /// Maps a view vector to device coordinates.
        /// </summary>
        /// <param name="view">The target view.</param>
        /// <param name="deviceSize">Size of the utilized render view.</param>
        /// <param name="target">The target coordinates to map.</param>
        public static Vector2i MapToPixel(this View view, Vector2f deviceSize, Vector2f target)
        {
            var ratio = view.Size.DivideBy(deviceSize);
            var offset = view.Center - view.Size / 2;
            return (target - offset).DivideBy(ratio).ToVector2i();
        }

        /// <summary>
        /// Maps a device vector to view coordinates.
        /// </summary>
        /// <param name="view">The target view.</param>
        /// <param name="deviceSize">Size of the utilized render view.</param>
        /// <param name="target">The target coordinates to map.</param>
        public static Vector2f MapToView(this View view, Vector2f deviceSize, Vector2f target)
        {
            var ratio = view.Size.DivideBy(deviceSize);
            var offset = view.Center - view.Size / 2;
            return target.MultiplyBy(ratio) + offset;
        }
    }
}