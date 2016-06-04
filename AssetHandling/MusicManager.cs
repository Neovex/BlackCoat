using System;
using System.Collections.Generic;
using SFML.Audio;

namespace BlackCoat
{
    /// <summary>
    /// Music management class. Handles loading/unloading of unmanaged Music resources.
    /// </summary>
    public class MusicManager : AssetManager<Music>
    {
        // Statics #########################################################################
        public static readonly IEnumerable<String> AvailableFormats = new[] { ".wav", ".ogg", ".flac" };


        // CTOR ############################################################################
        /// <summary>
        /// Creates a new instance of the AssetManager class.
        /// </summary>
        /// <param name="core">Engine Core</param>
        /// <param name="assetRoot">Optional root path of the managed asset folder</param>
        public MusicManager(String assetRoot = "") : base(AvailableFormats, assetRoot)
        {
        }
    }
}