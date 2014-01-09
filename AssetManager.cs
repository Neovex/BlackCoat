using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace BlackCoat
{
    public class AssetManager
    {
        // Variables #######################################################################
        private Core _Core;
        private Dictionary<String, Image> _Images = new Dictionary<String, Image>();
        private Dictionary<String, Font> _Fonts = new Dictionary<String, Font>();


        // CTOR ############################################################################
        public AssetManager(Core core)
        {
            _Core = core;
        }


        // Methods #########################################################################
        /// <summary>
        /// Loads an Image from a File or retrieves an already loaded instance
        /// </summary>
        /// <param name="name">Name of the Ressource</param>
        /// <returns>Image instance</returns>
        public Image LoadImage(String name)
        {
            return LoadImage(name, true);
        }

        /// <summary>
        /// Loads an Image from a File or retrieves an already loaded instance
        /// </summary>
        /// <param name="name">Name of the Ressource</param>
        /// <param name="smothing">Determines if the loaded Image should be smoothed</param>
        /// <returns>Image instance</returns>
        public Image LoadImage(String name, Boolean smothing)
        {
            if (_Images.ContainsKey(name)) return _Images[name];

            try
            {
                var img = new Image(String.Format("assets\\{0}.png", name));
                img.Smooth = smothing;
                _Images.Add(name, img);
                return img;
            }
            catch
            {
                _Core.Log("Could not load image", name);
            }
            return null;
        }

        /// <summary>
        /// Creates a rectangular Image or retrieves an already created instance
        /// </summary>
        /// <param name="width">Width of the Image that should be created</param>
        /// <param name="height">Height of the Image that should be created</param>
        /// <param name="color">Color of the Image as hex value 0xAARRGGBB</param>
        /// <param name="name">Optional name of the Ressource</param>
        /// <returns>The new Image</returns>
        public Image CreateImage(UInt32 width, UInt32 height, UInt32 color, String name = "")
        {
            if (_Images.ContainsKey(name)) return _Images[name];
            var c = new Color((Byte) ((color >> 0x10) & 0xff), 
                              (Byte) ((color >> 0x08) & 0xff),
                              (Byte)  (color &  0xff), 
                              (Byte) ((color >> 0x18) & 0xff));
            var img = new Image(width, height, c);
            if (!String.IsNullOrEmpty(name)) _Images.Add(name, img);
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
        /// Unloads an image with the given name
        /// </summary>
        /// <param name="name">Name of the Ressource</param>
        public void FreeImage(String name)
        {
            if (_Images.ContainsKey(name))
            {
                _Images[name].Dispose();
                _Images.Remove(name);
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
            foreach (var img in _Images.Values)
            {
                img.Dispose();
            }
            _Images.Clear();

            foreach (var fnt in _Fonts.Values)
            {
                fnt.Dispose();
            }
            _Fonts.Clear();
        }
    }
}