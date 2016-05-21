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
    public class TextureManager : AssetManager<Texture>
    {
        public static readonly IEnumerable<String> AvailableFormats = new[] { ".bmp", ".png", ".tga", ".jpg", ".gif", ".psd", ".hdr", ".pic" };

        // Properties ######################################################################
        /// <summary>
        /// Determines if a smoothing should be applied onto newly loaded Textures
        /// </summary>
        public Boolean ApplySmoothing { get; set; }


        // CTOR ############################################################################
        /// <summary>
        /// Creates a new instance of the TextureManager class.
        /// </summary>
        /// <param name="assetRoot">Optional root path of the managed asset folder</param>
        public TextureManager(String assetRoot = "") : base(AvailableFormats, assetRoot)
        {
            ApplySmoothing = false;
        }


        // Methods #########################################################################
        /// <summary>
        /// Loads a Texture from a File or retrieves an already loaded instance
        /// </summary>
        /// <param name="name">Name of the Resource</param>
        /// <returns>Texture instance</returns>
        public override Texture Load(string name, byte[] rawData = null)
        {
            var tex = base.Load(name, rawData);
            if (tex != null) tex.Smooth = ApplySmoothing;
            return tex;
        }

        /// <summary>
        /// Creates a rectangular Texture or retrieves an already created instance
        /// </summary>
        /// <param name="width">Width of the Image that should be created</param>
        /// <param name="height">Height of the Image that should be created</param>
        /// <param name="color">Color of the Image as hex value 0xAARRGGBB</param>
        /// <param name="name">Name of the generated Texture</param>
        /// <returns>The new or present Texture</returns>
        public Texture CreateTexture(UInt32 width, UInt32 height, UInt32 color, String name)
        {
            if (Disposed) throw new ObjectDisposedException("TextureManager");
            if (_Assets.ContainsKey(name)) return _Assets[name];
            var c = new Color((Byte)((color >> 0x10) & 0xff),
                              (Byte)((color >> 0x08) & 0xff),
                              (Byte)(color & 0xff),
                              (Byte)((color >> 0x18) & 0xff));
            var img = new Texture(new Image(width, height, c));
            if (!String.IsNullOrEmpty(name)) _Assets.Add(name, img);
            return img;
        }
    }
}