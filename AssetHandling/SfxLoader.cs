using System;
using System.IO;
using System.Collections.Generic;
using SFML.Audio;

namespace BlackCoat
{
    /// <summary>
    /// Sound management class. Handles loading/unloading of unmanaged sound resources.
    /// </summary>
    public class SfxLoader : AssetLoader<SoundBuffer>
    {
        // Statics #########################################################################
        public static readonly IEnumerable<String> AvailableFormats = new[] { ".wav", ".ogg", ".flac" };


        // CTOR ############################################################################
        /// <summary>
        /// Creates a new instance of the SfxLoader class.
        /// </summary>
        /// <param name="assetRoot">Optional root path of the managed asset folder</param>
        public SfxLoader(String assetRoot = "") : base(AvailableFormats, assetRoot)
        {
        }


        // Methods #########################################################################
        /// <summary>
        /// Loads or retrieves an already loaded instance of a Sound from a Stream Source
        /// </summary>
        /// <param name="name">Name of the Resource</param>
        /// <param name="stream">Readable stream containing the raw data of the sound</param>
        /// <returns>The managed Sound</returns>
        public SoundBuffer Load(string name, Stream stream)
        {
            // Sanity
            if (Disposed) throw new ObjectDisposedException(nameof(SfxLoader));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (_Assets.ContainsKey(name)) return _Assets[name];
            if (stream == null || !stream.CanRead) throw new ArgumentNullException(nameof(stream));
            // Load
            var data = new byte[stream.Length];
            stream.Read(data, 0, data.Length);
            return Load(name, data);
        }
    }
}