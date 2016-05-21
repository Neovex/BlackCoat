﻿using System;
using System.Collections.Generic;
using System.IO;
using SFML.Graphics;
using SFML.Audio;

namespace BlackCoat
{
    /// <summary>
    /// TODO
    /// </summary>
    public class SfxManager : AssetManager<SoundBuffer>
    {
        public static readonly IEnumerable<String> AvailableFormats = new[] { ".wav", ".ogg", ".flac" };

        // CTOR ############################################################################
        /// <summary>
        /// Creates a new instance of the SfxManager class.
        /// </summary>
        /// <param name="assetRoot">Optional root path of the managed asset folder</param>
        public SfxManager(String assetRoot = "") : base(AvailableFormats, assetRoot)
        {
        }
    }
}