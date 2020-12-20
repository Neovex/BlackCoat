using SFML.Graphics;

namespace BlackCoat
{
    /// <summary>
    /// Extensions for <see cref="Color"/>.
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// Calculates the inverse of the color disregarding its alpha value.
        /// </summary>
        public static Color Invert(this Color source) => new Color((byte)(byte.MaxValue - source.R), (byte)(byte.MaxValue - source.G), (byte)(byte.MaxValue - source.B));
        
        /// <summary>
        /// Applies an alpha value to a color.
        /// </summary>
        public static Color ApplyAlpha(this Color color, float alpha)
        {
            color.A = (byte)(byte.MaxValue * alpha);
            return color;
        }
    }
}