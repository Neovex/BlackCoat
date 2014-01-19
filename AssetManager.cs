using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using System.IO;

namespace BlackCoat
{
    public class AssetManager
    {
        // Variables #######################################################################
        private Core _Core;
        private Dictionary<String, Texture> _Textures = new Dictionary<String, Texture>();
        private Dictionary<String, Font> _Fonts = new Dictionary<String, Font>();

        /// <summary>
        /// Rootfolder to look for Assets
        /// </summary>
        public String RootFolder { get; set; }

        /// <summary>
        /// Determines if a smoothing should be apllied onto newly loaded Textures
        /// </summary>
        public Boolean SmoothTexturesOnLoad { get; set; }


        // CTOR ############################################################################
        public AssetManager(Core core)
        {
            _Core = core;
            RootFolder = String.Empty;
            SmoothTexturesOnLoad = false;
        }


        // Methods #########################################################################
        /// <summary>
        /// Loads a Texture from a File or retrieves an already loaded instance
        /// </summary>
        /// <param name="name">Name of the Ressource</param>
        /// <returns>Texture instance</returns>
        public Texture LoadTexture(String name)
        {
            return LoadTexture(name, SmoothTexturesOnLoad);
        }

        /// <summary>
        /// Loads an Texture from a File or retrieves an already loaded instance
        /// </summary>
        /// <param name="name">Name of the Ressource</param>
        /// <param name="smothing">Determines if the loaded Texture should be smoothed</param>
        /// <returns>Texture instance</returns>
        public Texture LoadTexture(String name, Boolean smothing)
        {
            if (_Textures.ContainsKey(name)) return _Textures[name];

            try
            {
                var img = new Texture(Path.Combine(RootFolder, String.Concat(name, ".png")));
                img.Smooth = smothing;
                _Textures.Add(name, img);
                return img;
            }
            catch
            {
                _Core.Log("Could not load image", name);
            }
            return null;
        }

        /// <summary>
        /// Creates a rectangular Texture or retrieves an already created instance
        /// </summary>
        /// <param name="width">Width of the Image that should be created</param>
        /// <param name="height">Height of the Image that should be created</param>
        /// <param name="color">Color of the Image as hex value 0xAARRGGBB</param>
        /// <param name="name">Optional name of the Ressource</param>
        /// <returns>The new or present Texture</returns>
        public Texture CreateTexture(UInt32 width, UInt32 height, UInt32 color, String name = "")
        {
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
        /// <param name="name">Name of the Ressource</param>
        /// <returns>The new Font</returns>
        public Font LoadFont(String name)
        {
            if (_Fonts.ContainsKey(name)) return _Fonts[name];

            try
            {
                var fnt = new Font("assets\\" + name + ".png");
                _Fonts.Add(name, fnt);
                return fnt;
            }
            catch
            {
                _Core.Log("Could not load font", name);
            }
            return null;
        }

        /// <summary>
        /// Unloads an Texture with the given name
        /// </summary>
        /// <param name="name">Name of the Ressource</param>
        public void FreeTexture(String name)
        {
            if (_Textures.ContainsKey(name))
            {
                _Textures[name].Dispose();
                _Textures.Remove(name);
            }
        }

        /// <summary>
        /// Unloads an font with the given name
        /// </summary>
        /// <param name="name">Name of the Ressource</param>
        public void FreeFont(String name)
        {
            if (_Fonts.ContainsKey(name))
            {
                _Fonts[name].Dispose();
                _Fonts.Remove(name);
            }
        }

        /// <summary>
        /// Releases all ressources
        /// </summary>
        internal void Dispose()
        {
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
        }
    }
}