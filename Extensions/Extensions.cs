using System;
using SFML.Graphics;

namespace BlackCoat
{
    /// <summary>
    /// Provides extension methods for engine and library types
    /// </summary>
    public static class Extensions
    {
        private static readonly String _DefaultCodepage = "123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ.:-_,;#'+*~´`\\?ß=()&/$%\"§!^°";

        /// <summary>
        /// Pre-renders the font texture for all glyphs of the default code page.
        /// </summary>
        /// <param name="charSize">Size of the characters to be pre-rendered.</param>
        /// <param name="bold">Determines if the characters should be rendered in bold.</param>
        public static void PreloadDefaultCodepage(this Font font, uint charSize = 10, bool bold = false)
        {
            foreach (var character in _DefaultCodepage)
            {
                font.GetGlyph(character, charSize, bold);
            }
        }
    }
}