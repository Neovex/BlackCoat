using System;
using System.Collections.Generic;
using System.IO;
using SFML.Graphics;
using SFML.Audio;

namespace BlackCoat
{
    /// <summary>
    /// Todo
    /// </summary>
    public class MusicManager : AssetManager<Music>
    {
        public static readonly IEnumerable<String> AvailableFormats = new[] { ".wav", ".ogg", ".flac" };

        // Variables #######################################################################


        // Properties ######################################################################


        // CTOR ############################################################################
        /// <summary>
        /// Creates a new instance of the AssetManager class.
        /// </summary>
        /// <param name="core">Engine Core</param>
        /// <param name="assetRoot">Optional root path of the managed asset folder</param>
        public MusicManager(String assetRoot = "") : base(AvailableFormats, assetRoot)
        {
        }


        // Methods #########################################################################
    }
}