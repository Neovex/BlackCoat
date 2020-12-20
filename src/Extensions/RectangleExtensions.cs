using SFML.System;
using SFML.Graphics;

namespace BlackCoat
{
    /// <summary>
    /// Provides extension methods for all SFML Rectangle types.
    /// </summary>
    public static class RectangleExtensions
    {
        /// <summary>
        /// Extracts the position of the <see cref="FloatRect"/>
        /// </summary>
        /// <returns>The position as <see cref="Vector2f"/></returns>
        public static Vector2f Position(this FloatRect src) => new Vector2f(src.Left, src.Top);

        /// <summary>
        /// Extracts the size of the <see cref="FloatRect"/>
        /// </summary>
        /// <returns>The size (width x height) as <see cref="Vector2f"/></returns>
        public static Vector2f Size(this FloatRect src) => new Vector2f(src.Width, src.Height);


        /// <summary>
        /// Extracts the position of the <see cref="IntRect"/>
        /// </summary>
        /// <returns>The position as <see cref="Vector2f"/></returns>
        public static Vector2i Position(this IntRect src) => new Vector2i(src.Left, src.Top);

        /// <summary>
        /// Extracts the size of the <see cref="IntRect"/>
        /// </summary>
        /// <returns>The size (width x height) as <see cref="Vector2f"/></returns>
        public static Vector2i Size(this IntRect src) => new Vector2i(src.Width, src.Height);

        /// <summary>
        /// Converts a 4 int tuple (x, y, width, height) into a <see cref="IntRect"/>.
        /// </summary>
        /// <returns>A <see cref="IntRect"/></returns>
        public static IntRect ToIntRect(this (int x, int y, int width, int height) src) 
            => new IntRect(src.x, src.y, src.width, src.height);

        /// <summary>
        /// Converts a 4 int tuple (x, y, width, height) into a <see cref="FloatRect"/>.
        /// </summary>
        /// <returns>A <see cref="FloatRect"/></returns>
        public static FloatRect ToFloatRect(this (int x, int y, int width, int height) src) 
            => new FloatRect(src.Item1, src.Item2, src.Item3, src.Item4);

        /// <summary>
        /// Converts a 4 float tuple (x, y, width, height) into a <see cref="FloatRect"/>.
        /// </summary>
        /// <returns>A <see cref="FloatRect"/></returns>
        public static FloatRect ToFloatRect(this (float x, float y, float width, float height) src) 
            => new FloatRect(src.x, src.y, src.width, src.height);

        /// <summary>
        /// Converts a 4 double tuple (x, y, width, height) into a <see cref="FloatRect"/>.
        /// </summary>
        /// <returns>A <see cref="FloatRect"/></returns>
        public static FloatRect ToFloatRect(this (double x, double y, double width, double height) src)
            => new FloatRect((float)src.x, (float)src.y, (float)src.width, (float)src.height);
    }
}