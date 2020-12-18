using System;
using System.Collections.Generic;
using SFML.Audio;

namespace BlackCoat
{
    /// <summary>
    /// Music management class. Handles loading/unloading of unmanaged Music resources.
    /// </summary>
    public class MusicLoader : AssetLoader<Music>
    {
        // Statics #########################################################################
        public static readonly IEnumerable<String> AvailableFormats = new[]
        {
          ".ogg", ".wav", ".flac", ".aiff", ".au", ".raw",
          ".paf", ".svx", ".nist", ".voc", ".ircam", ".w64",
          ".mat4", ".mat5", ".pvf", ".htk", ".sds", ".avr",
          ".sd2", ".caf", ".wve",".mpc2k", ".rf64"
        };


        // CTOR ############################################################################
        /// <summary>
        /// Creates a new instance of the MusicLoader class.
        /// </summary>
        /// <param name="core">Engine Core</param>
        /// <param name="assetRoot">Optional root path of the managed asset folder</param>
        public MusicLoader(String assetRoot = "") : base(AvailableFormats, assetRoot)
        {
        }
    }
}