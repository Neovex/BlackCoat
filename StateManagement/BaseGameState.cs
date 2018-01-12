using System;
using BlackCoat.Entities;
using BlackCoat.Tools;

namespace BlackCoat
{
    /// <summary>
    /// Base class for all Gamestates
    /// </summary>
    public abstract class BaseGamestate
    {
        protected Core _Core;
        private PerformanceMonitor _PerformanceMonitor;

        // State Info
        public String Name { get; protected set; }
        public Boolean Paused { get; protected set; }

        // Asset Managers
        protected FontManager FontManager { get; set; }
        protected MusicManager MusicManager { get; set; }
        protected SfxManager SfxManager { get; set; }
        protected TextureManager TextureManager { get; set; }

        // Layers
        protected Layer Layer_BG { get; private set; }
        protected Layer Layer_Game { get; private set; }
        protected Layer Layer_Particles { get; private set; }
        protected Layer Layer_Overlay { get; private set; }
        protected Layer Layer_Debug { get; private set; }
        protected Layer Layer_Cursor { get; private set; }


        /// <summary>
        /// Occurs when the State has been successfully initialized.
        /// </summary>
        public event Action Loaded = () => { };
        /// <summary>
        /// Occurs when the State has failed to initialize.
        /// </summary>
        public event Action LoadingFailed = () => { };
        /// <summary>
        /// Occurs when the State is about to be destroyed.
        /// </summary>
        public event Action OnDestroy = () => { };


        /// <summary>
        /// Initializes a new instance of the <see cref="BaseGamestate"/> class.
        /// </summary>
        /// <param name="core">Engine core.</param>
        /// <param name="name">State name.</param>
        /// <param name="root">Optional Asset root path.</param>
        public BaseGamestate(Core core, String name = null, String root = "") // todo : a single root for all assets does not seem to be so great - recheck that
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

            // Handle Debug Overlay
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

        /// <summary>
        /// Draws this state onto the scene.
        /// </summary>
        internal void Draw()
        {
            Layer_BG.Draw();
            Layer_Game.Draw();
            Layer_Particles.Draw();
            Layer_Overlay.Draw();
            Layer_Debug.Draw();
            Layer_Cursor.Draw();
        }

        /// <summary>
        /// Loads the required data for this state.
        /// </summary>
        /// <returns>True on success.</returns>
        internal bool LoadInternal()
        {
            if (Load())
            {
                Loaded.Invoke();
                return true;
            }
            else
            {
                LoadingFailed.Invoke();
                return false;
            }
        }

        /// <summary>
        /// Updates the State and its children.
        /// </summary>
        /// <param name="deltaT">Current frame time.</param>
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

        /// <summary>
        /// Destroys the state.
        /// </summary>
        internal void DestroyInternal()
        {
            OnDestroy.Invoke();
            Destroy();
            FontManager.Dispose();
            MusicManager.Dispose();
            SfxManager.Dispose();
            TextureManager.Dispose();
            Log.Debug(Name, "destroyed");
        }

        /// <summary>
        /// Loads the required data for this state.
        /// </summary>
        /// <returns>True on success.</returns>
        protected abstract Boolean Load();

        /// <summary>
        /// Updates the State and its children.
        /// </summary>
        /// <param name="deltaT">Current frame time.</param>
        protected abstract void Update(float deltaT);

        /// <summary>
        /// Destroys the state.
        /// </summary>
        protected abstract void Destroy();

        /// <summary>Returns a <see cref="System.String" /> that represents this instance.</summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString() => $"{base.ToString()} \"{Name}\"";
    }
}