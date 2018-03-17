using System;
using BlackCoat.Entities;
using BlackCoat.Tools;
using SFML.Graphics;
using SFML.System;

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

        // Asset Managers
        protected TextureLoader TextureLoader { get; set; }
        protected MusicLoader MusicLoader { get; set; }
        protected FontLoader FontLoader { get; set; }
        protected SfxLoader SfxLoader { get; set; }

        // Layers
        protected Layer Layer_BG { get; private set; }
        protected Layer Layer_Game { get; private set; }
        protected Layer Layer_Overlay { get; private set; }
        protected Layer Layer_Debug { get; private set; }
        protected CursorLayer Layer_Cursor { get; private set; }


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
        /// Initializes a new instance of the <see cref="BaseGamestate" /> class.
        /// </summary>
        /// <param name="core">Engine core.</param>
        /// <param name="name">Optional name of the state.</param>
        /// <param name="assetRoot">Optional Asset root path.</param>
        public BaseGamestate(Core core, String name = null, String assetRoot = "") : this(core, name, assetRoot, assetRoot, assetRoot, assetRoot)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseGamestate" /> class.
        /// </summary>
        /// <param name="core">Engine core.</param>
        /// <param name="name">Name of the state.</param>
        /// <param name="textures">Texture root path.</param>
        /// <param name="music">Music root path.</param>
        /// <param name="fonts">Font root path.</param>
        /// <param name="sfx">Sound effects root path.</param>
        public BaseGamestate(Core core, String name, String textures, String music, String fonts, String sfx)
        {
            // Init
            _Core = core;
            Name = String.IsNullOrWhiteSpace(name) ? GetType().Name : name;

            // Create Asset Managers
            TextureLoader = new TextureLoader(textures);
            MusicLoader = new MusicLoader(music);
            FontLoader = new FontLoader(fonts);
            SfxLoader = new SfxLoader(sfx);

            // Create Default Layer Structure
            // Game Layer
            Layer_BG = new Layer(_Core);
            Layer_Game = new Layer(_Core);
            Layer_Overlay = new Layer(_Core);
            // System Layer
            Layer_Debug = new Layer(_Core);
            Layer_Cursor = new CursorLayer(_Core);

            // Initialize Debug Overlay
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
            Layer_BG.Update(deltaT);
            Layer_Game.Update(deltaT);
            Layer_Overlay.Update(deltaT);
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
            FontLoader.Dispose();
            MusicLoader.Dispose();
            SfxLoader.Dispose();
            TextureLoader.Dispose();
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

        /// <summary>
        /// Replaces the cursor with a texture or restores the original.
        /// </summary>
        /// <param name="texture">The texture to replace the cursor or null to restore system default.</param>
        /// <param name="origin">The optional origin of the texture.</param>
        public void SetCursor(Texture texture, Vector2f origin = new Vector2f()) => Layer_Cursor.SetCursor(texture, origin);
    }
}