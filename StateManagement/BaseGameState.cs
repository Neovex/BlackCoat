using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlackCoat;
using BlackCoat.Entities;
using BlackCoat.Tools;

namespace BlackCoat
{
    public abstract class BaseGameState
    {
        protected Core _Core;
        private PerformanceMonitor _PerformanceMonitor;

        // State Info
        public String Name { get; protected set; }
        public Boolean Paused { get; set; }

        // Asset Managers
        public FontManager FontManager { get; protected set; }
        public MusicManager MusicManager { get; protected set; }
        public SfxManager SfxManager { get; protected set; }
        public TextureManager TextureManager { get; protected set; }

        // Layers
        public Layer Layer_BG { get; private set; }
        public Layer Layer_Game { get; private set; }
        public Layer Layer_Particles { get; private set; }
        public Layer Layer_Overlay { get; private set; }
        public Layer Layer_Debug { get; private set; }
        public Layer Layer_Cursor { get; private set; }


        /// <param name="root">Optional root path of the managed asset folder</param>
        public BaseGameState(Core core, String name = null, String root = "")
        {
            // Init
            _Core = core;
            Name = String.IsNullOrWhiteSpace(name) ? GetType().Name : name;

            // Create Asset Managers
            FontManager = new FontManager(root);
            MusicManager = new MusicManager(root);
            SfxManager = new SfxManager(root);
            TextureManager = new TextureManager(root);

            // Create Default Layer Structure
            // Game Layer
            Layer_BG = new Layer(_Core);
            Layer_Game = new Layer(_Core);
            Layer_Particles = new Layer(_Core);
            Layer_Overlay = new Layer(_Core);
            // System Layer
            Layer_Debug = new Layer(_Core);
            Layer_Cursor = new Layer(_Core);

            // Debug Overlay
            HandleDebugChanged(_Core.Debug);
            _Core.DebugChanged += HandleDebugChanged;
        }

        private void HandleDebugChanged(bool debug)
        {
            if (debug)
            {
                _PerformanceMonitor = _PerformanceMonitor ?? new PerformanceMonitor(_Core);
                Layer_Debug.AddChild(_PerformanceMonitor);
            }
            else if(_PerformanceMonitor != null)
            {
                Layer_Debug.RemoveChild(_PerformanceMonitor);
            }
        }

        public abstract Boolean Load();
        public abstract void Update(float deltaT);
        public abstract void Destroy();

        internal void UpdateInternal(float deltaT)
        {
            if (!Paused) // TODO: good?
            {
                Layer_BG.Update(deltaT);
                Layer_Game.Update(deltaT);
                Layer_Particles.Update(deltaT);
                Layer_Overlay.Update(deltaT);
            }
            Layer_Debug.Update(deltaT);
            Layer_Cursor.Update(deltaT);
            Update(deltaT);
        }

        internal void Draw()
        {
            Layer_BG.Draw();
            Layer_Game.Draw();
            Layer_Particles.Draw();
            Layer_Overlay.Draw();
            Layer_Debug.Draw();
            Layer_Cursor.Draw();
        }

        public override string ToString() { return String.Concat("\"", Name, "\""); }
    }
}