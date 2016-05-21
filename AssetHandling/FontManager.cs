using System;
using System.Collections.Generic;
using System.IO;
using SFML.Graphics;
using SFML.Audio;

namespace BlackCoat
{
    /// <summary>
    /// TODO
    /// </summary>
    public class FontManager : AssetManager<Font>
    {
        public static readonly IEnumerable<String> AvailableFormats = new[] { ".ttf", ".cff", ".fnt" }; // TODO: find and add file remaining endings for TrueType, Type 1, CFF, OpenType, SFNT, X11 PCF, Windows FNT, BDF, PFR and Type 42.

        // CTOR ############################################################################
        /// <summary>
        /// Creates a new instance of the FontManager class.
        /// </summary>
        /// <param name="core">Engine Core</param>
        /// <param name="assetRoot">Optional root path of the managed asset folder</param>
        public FontManager(String assetRoot = "") : base(AvailableFormats, assetRoot)
        {
        }
    }
}