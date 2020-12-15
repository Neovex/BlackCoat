﻿using System;
using System.IO;
using System.Drawing.Imaging;
using System.Collections.Generic;
using SFML.Graphics;

namespace BlackCoat
{
    /// <summary>
    /// Texture management class. Handles loading/unloading of unmanaged Texture resources.
    /// </summary>
    public class TextureLoader : AssetLoader<Texture>
    {
        // Statics #########################################################################
        public static readonly IEnumerable<String> AvailableFormats = new[] { ".bmp", ".png", ".tga", ".jpg", ".gif", ".psd", ".hdr", ".pic" };


        // Properties ######################################################################
        /// <summary>
        /// Determines if loaded Textures should repeat when the texture rectangle exceeds its dimension.
        /// </summary>
        public Boolean Repeat { get; set; }
        /// <summary>
        /// Determines if a smoothing should be applied onto newly loaded Textures.
        /// </summary>
        public Boolean Smoothing { get; set; }


        // CTOR ############################################################################
        /// <summary>
        /// Creates a new instance of the TextureLoader class.
        /// </summary>
        /// <param name="assetRoot">Optional root path of the managed asset folder</param>
        /// <param name="repeat">Determines if loaded Textures should repeat when the texture rectangle exceeds its dimension</param>
        /// <param name="smoothing">Determines if a smoothing should be applied onto newly loaded Textures</param>
        public TextureLoader(String assetRoot = "", Boolean repeat = false, Boolean smoothing = false) : base(AvailableFormats, assetRoot)
        {
            Repeat = repeat;
            Smoothing = smoothing;
        }


        // Methods #########################################################################
        /// <summary>
        /// Loads or retrieves an already loaded instance of a Texture from a File or Raw Data Source
        /// </summary>
        /// <param name="name">Name of the Texture</param>
        /// <param name="rawData">Optional byte array containing the raw data of the Texture</param>
        /// <returns>The managed Texture</returns>
        public override Texture Load(string name, byte[] rawData = null) => Load(name, Repeat, Smoothing, rawData);

        /// <summary>Loads or retrieves an already loaded instance of a Texture from a File or Raw Data Source</summary>
        /// <param name="name">Name of the Texture</param>
        /// <param name="repeat">Determines if loaded Textures should repeat when the texture rectangle exceeds its dimension.</param>
        /// <param name="smoothing">Determines if a smoothing should be applied onto newly loaded Textures.</param>
        /// <param name="rawData">Optional byte array containing the raw data of the Texture</param>
        /// <returns>The managed Texture</returns>
        public Texture Load(string name, bool? repeat = null, bool? smoothing = null, byte[] rawData = null)
        {
            var tex = base.Load(name, rawData);
            if (tex != null)
            {
                tex.Repeated = repeat ?? Repeat;
                tex.Smooth = smoothing ?? Smoothing;
            }
            return tex;
        }

        /// <summary>
        /// Converts or retrieves an already loaded instance of a Texture from a Bitmap Source
        /// </summary>
        /// <param name="name">Name of the Texture</param>
        /// <param name="bmp">Bitmap to be converted to a Texture</param>
        /// <returns>The managed Texture</returns>
        public Texture Load(string name, System.Drawing.Bitmap bmp)
        {
            // Sanity
            if (Disposed) throw new ObjectDisposedException(nameof(TextureLoader));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (_Assets.ContainsKey(name)) return _Assets[name];
            if (bmp == null) throw new ArgumentNullException(nameof(bmp));
            if (bmp.Size.Width > Texture.MaximumSize || bmp.Size.Height > Texture.MaximumSize)
                throw new ArgumentException($"Bitmap size exceeds capabilities of graphic adapter: {Texture.MaximumSize} pixel");
            // Conversion
            Byte[] data = null;
            using (var strm = new MemoryStream())
            {
                bmp.Save(strm, ImageFormat.Png);
                data = strm.ToArray();
            }
            return Load(name, data);
        }

        /// <summary>
        /// Creates a rectangular Texture or retrieves an already created instance
        /// </summary>
        /// <param name="width">Width of the Image that should be created</param>
        /// <param name="height">Height of the Image that should be created</param>
        /// <param name="color">Color of the Image as hex value 0xAARRGGBB</param>
        /// <param name="name">Name of the generated Texture</param>
        /// <returns>The new or present Texture</returns>
        [Obsolete("CreateTexture is deprecated, please use a Rectangle or a PrerenderedContainer instead.")]
        public Texture CreateTexture(UInt32 width, UInt32 height, UInt32 color, String name)
        {
            if (Disposed) throw new ObjectDisposedException("TextureManager");
            if (_Assets.ContainsKey(name)) return _Assets[name];
            var c = new Color((Byte)((color >> 0x10) & 0xff),
                              (Byte)((color >> 0x08) & 0xff),
                              (Byte) (color & 0xff),
                              (Byte)((color >> 0x18) & 0xff));
            var img = new Texture(new Image(width, height, c));
            if (!String.IsNullOrEmpty(name)) _Assets.Add(name, img);
            return img;
        }
    }
}