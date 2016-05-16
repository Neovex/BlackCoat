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

        public String Name { get; protected set; }
        public Boolean Paused { get; set; }
        // Layers
        public Layer Layer_BG { get; private set; }
        public Layer Layer_Game { get; private set; }
        public Layer Layer_Particles { get; private set; }
        public Layer Layer_Overlay { get; private set; }
        public Layer Layer_Debug { get; private set; }
        public Layer Layer_Cursor { get; private set; }

        public BaseGameState(Core core, String name = null)
        {
            _Core = core;
            _Core.DebugChanged += HandleDebugChanged;
            Name = String.IsNullOrWhiteSpace(name) ? GetType().Name : name;

            // Create Default Layer Structure
            // Game Layer
            Layer_BG = new Layer(_Core);
            Layer_Game = new Layer(_Core);
            Layer_Particles = new Layer(_Core);
            Layer_Overlay = new Layer(_Core);
            // System Layer
            Layer_Debug = new Layer(_Core);
            Layer_Cursor = new Layer(_Core);
        }

        private void HandleDebugChanged(bool obj)
        {
            if (obj)
            {
                _PerformanceMonitor = _PerformanceMonitor ?? new PerformanceMonitor(_Core);
                Layer_Debug.AddChild(_PerformanceMonitor);
            }
            else
            {
                Layer_Debug.RemoveChild(_PerformanceMonitor);
            }
        }

        public abstract Boolean Load();
        public abstract void Update(float deltaT);
        public abstract void Destroy();

        internal void UpdateInternal(float deltaT)
        {
            if (!Paused)
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

        public override string ToString() { return String.Concat(GetType().Name, "\"", Name, "\""); }
    }
}