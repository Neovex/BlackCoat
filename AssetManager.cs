using System;
using System.Collections.Generic;
using System.IO;
using SFML.Graphics;

namespace BlackCoat
{
    /// <summary>
    /// Asset management class. Handles loading/unloading of assets located in its specified root folder.
    /// Multiple instances of the AssetManager class can be used to simplify asset memory management.
    /// </summary>
    public class AssetManager : IDisposable
    {
        // Variables #######################################################################
        private Core _Core;
        private Dictionary<String, Texture> _Textures = new Dictionary<String, Texture>();
        private Dictionary<String, Font> _Fonts = new Dictionary<String, Font>();

        /// <summary>
        /// Root-folder to look for Assets
        /// </summary>
        public String RootFolder { get; set; }

        /// <summary>
        /// Determines if a smoothing should be applied onto newly loaded Textures
        /// </summary>
        public Boolean SmoothTexturesOnLoad { get; set; }


        // Properties ######################################################################
        /// <summary>
        /// Gets a value indicating whether this AssetManager is disposed.
        /// </summary>
        public bool Disposed { get; private set; }


        // CTOR ############################################################################
        /// <summary>
        /// Creates a new instance of the AssetManager class.
        /// </summary>
        /// <param name="core">Engine Core</param>
        /// <param name="assetRoot">Optional root path of the managed asset folder</param>
        public AssetManager(Core core, String assetRoot = "")
        {
            _Core = core;
            RootFolder = assetRoot;
            SmoothTexturesOnLoad = false;
        }

        ~AssetManager()
        {
            if (!Disposed) Dispose();
        }


        // Methods #########################################################################
        /// <summary>
        /// Loads a Texture from a File or retrieves an already loaded instance
        /// </summary>
        /// <param name="name">Name of the Resource</param>
        /// <returns>Texture instance</returns>
        public Texture LoadTexture(String name)
        {
            return LoadTexture(name, SmoothTexturesOnLoad);
        }

        /// <summary>
        /// Loads an Texture from a File or retrieves an already loaded instance
        /// </summary>
        /// <param name="name">Name of the Texture</param>
        /// <param name="smothing">Determines if the loaded Texture should be smoothed</param>
        /// <returns>Texture instance</returns>
        public Texture LoadTexture(String name, Boolean smothing)
        {
            if (Disposed) throw new ObjectDisposedException("AssetManager");
            if (_Textures.ContainsKey(name)) return _Textures[name];

            var path = Path.Combine(RootFolder, String.Concat(name, ".png"));
            try
            {
                var img = new Texture(path);
                img.Smooth = smothing;
                _Textures.Add(name, img);
                return img;
            }
            catch (Exception e)
            {
                _Core.Log("Could not load texture", name, "from", path, "because of", e);
            }
            return null;
        }

        /// <summary>
        /// Loads an Texture from a Byte Array
        /// </summary>
        /// <param name="name">Name of the Texture</param>
        /// <param name="textureData">Byte Array containing the raw texture</param>
        /// <param name="smothing">Determines if the loaded Texture should be smoothed</param>
        /// <returns>Texture instance</returns>
        public Texture LoadTexture(String name, Byte[] textureData, Boolean smothing)
        {
            if (Disposed) throw new ObjectDisposedException("AssetManager");
            if (_Textures.ContainsKey(name)) throw new ArgumentException("A texture with the provided name already exists");

            try
            {
                var img = new Texture(textureData);
                img.Smooth = smothing;
                _Textures.Add(name, img);
                return img;
            }
            catch (Exception e)
            {
                _Core.Log("Could not create texture", name, "because of", e);
            }
            return null;
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
            if (Disposed) throw new ObjectDisposedException("AssetManager");
            if (_Textures.ContainsKey(name)) return _Textures[name];
            var c = new Color((Byte) ((color >> 0x10) & 0xff), 
                              (Byte) ((color >> 0x08) & 0xff),
                              (Byte)  (color &  0xff), 
                              (Byte) ((color >> 0x18) & 0xff));
            var img = new Texture(new Image(width, height, c));
            if (!String.IsNullOrEmpty(name)) _Textures.Add(name, img);
            return img;
        }

        /// <summary>
        /// Loads a Font from a File or retrieves an already loaded instance
        /// </summary>
        /// <param name="name">Name of the Resource</param>
        /// <returns>The new Font</returns>
        public Font LoadFont(String name)
        {
            if (Disposed) throw new ObjectDisposedException("AssetManager");
            if (_Fonts.ContainsKey(name)) return _Fonts[name];

            var path = Path.Combine(RootFolder, String.Concat(name, ".ttf"));
            try
            {
                var font = new Font(path);
                _Fonts.Add(name, font);
                return font;
            }
            catch (Exception e)
            {
                _Core.Log("Could not load font", name, "from", path, "because of", e);
            }
            return null;
        }

        /// <summary>
        /// Loads a new Font from a Byte Array
        /// </summary>
        /// <param name="name">Name of the Resource</param>
        /// <param name="fontData">The Font as raw data byte array</param>
        /// <returns>The new Font</returns>
        public Font LoadFont(String name, Byte[] fontData)
        {
            if (Disposed) throw new ObjectDisposedException("AssetManager");
            if (_Fonts.ContainsKey(name)) throw new ArgumentException("A font with the provided name already exists");

            try
            {
                var fnt = new Font(fontData);
                _Fonts.Add(name, fnt);
                return fnt;
            }
            catch (Exception e)
            {
                _Core.Log("Could not load font", name, "because of", e);
            }
            return null;
        }

        /// <summary>
        /// Unloads an Texture with the given name
        /// </summary>
        /// <param name="name">Name of the Resource</param>
        public void FreeTexture(String name)
        {
            if (Disposed) throw new ObjectDisposedException("AssetManager");
            if (_Textures.ContainsKey(name))
            {
                _Textures[name].Dispose();
                _Textures.Remove(name);
            }
        }

        /// <summary>
        /// Unloads an font with the given name
        /// </summary>
        /// <param name="name">Name of the Resource</param>
        public void FreeFont(String name)
        {
            if (Disposed) throw new ObjectDisposedException("AssetManager");
            if (_Fonts.ContainsKey(name))
            {
                _Fonts[name].Dispose();
                _Fonts.Remove(name);
            }
        }

        /// <summary>
        /// Releases all used unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (Disposed) return;
            foreach (var tex in _Textures.Values)
            {
                tex.Dispose();
            }
            _Textures.Clear();

            foreach (var fnt in _Fonts.Values)
            {
                fnt.Dispose();
            }
            _Fonts.Clear();

            Disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}