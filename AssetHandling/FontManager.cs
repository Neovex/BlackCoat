using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace BlackCoat
{
    /// <summary>
    /// Font management class. Handles loading/unloading of unmanaged font resources.
    /// </summary>
    public class FontManager : AssetManager<Font>
    {
        // Statics #########################################################################
        public static readonly IEnumerable<String> AvailableFormats = new[] { ".ttf", ".cff", ".fnt", ".ttf", ".otf", ".eot" };


        // CTOR ############################################################################
        /// <summary>
        /// Creates a new instance of the FontManager class.
        /// </summary>
        /// <param name="assetRoot">Optional root path of the managed asset folder</param>
        public FontManager(String assetRoot = "") : base(AvailableFormats, assetRoot)
        {
        }
    }
}