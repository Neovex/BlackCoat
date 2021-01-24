using System;
using BlackCoat.Entities;
using BlackCoat.Tools;

namespace BlackCoat
{
    /// <summary>
    /// Base class for all Scenes.
    /// </summary>
    public abstract class Scene : BlackCoatBase
    {
        // Variables #######################################################################
        private PerformanceMonitor _PerformanceMonitor;
        private Input _DefaultInput;
        private Input _Input;


        // Properties ######################################################################
        #region Fundamentals
        /// <summary>
        /// Gets or sets the name of this <see cref="Scene"/>.
        /// </summary>
        public String Name { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Scene"/> is destroyed.
        /// </summary>
        public Boolean Destroyed { get; private set; }
   
        /// <summary>
        /// Gets or sets the default input source for his <see cref="Scene"/>.
        /// </summary>
        public Input Input
        {
            get => _Input ?? _DefaultInput ?? (_DefaultInput = new Input(_Core));
            protected set => _Input = value;
        }
        #endregion

        #region Asset Managers
        /// <summary>
        /// Use the <see cref="TextureLoader"/> to load all sorts of pixel based graphics.
        /// </summary>
        protected internal TextureLoader TextureLoader { get; }

        /// <summary>
        /// Use the <see cref="MusicLoader"/> to load the music for your <see cref="Scene"/>.
        /// For short sound effects use the <see cref="Scene.SfxLoader"/> instead.
        /// </summary>
        protected internal MusicLoader MusicLoader { get; }

        /// <summary>
        /// Use the <see cref="FontLoader"/> to load custom fonts for your text based entities.
        /// </summary>
        protected internal FontLoader FontLoader { get; }

        /// <summary>
        /// Use the <see cref="SfxLoader"/> to load short sound effects.
        /// For your scenes music use the <see cref="Scene.MusicLoader"/> instead.
        /// </summary>
        /// <seealso cref="BlackCoat.AssetHandling.SfxManager"/>
        protected internal SfxLoader SfxLoader { get; }
        #endregion

        #region Layers
        /// <summary>
        /// Lowest layer for background graphics or animations.
        /// </summary>
        protected internal Layer Layer_Background { get; }

        /// <summary>
        /// Default layer for your games entities.
        /// </summary>
        protected internal Layer Layer_Game { get; }

        /// <summary>
        /// Top layer for all kinds of special effects overlays like
        /// <see cref="UI.Canvas"/>es, <see cref="Entities.Lights.Lightmap"/>s
        /// or <see cref="ParticleSystem.ParticleEmitterHost"/>s.
        /// </summary>
        protected internal Layer Layer_Overlay { get; }

        /// <summary>
        /// Debug Layer - internal use only - for now.
        /// </summary>
        internal Layer Layer_Debug { get; }

        //fixme!
        protected internal CursorLayer Layer_Cursor { get; }
        #endregion


        // Events ##########################################################################
        /// <summary>
        /// Occurs when the Scene has been successfully initialized.
        /// </summary>
        public event Action Loaded = () => { };

        /// <summary>
        /// Occurs when the Scene has failed to initialize.
        /// </summary>
        public event Action LoadingFailed = () => { };

        /// <summary>
        /// Occurs when the Scene is about to be destroyed.
        /// </summary>
        public event Action OnDestroy = () => { };


        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="Scene" /> class.
        /// </summary>
        /// <param name="core">Engine core.</param>
        /// <param name="name">Optional name of the Scene.</param>
        /// <param name="assetRoot">Optional Asset root path.</param>
        public Scene(Core core, String name = null, String assetRoot = "") : this(core, name, assetRoot, assetRoot, assetRoot, assetRoot)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Scene" /> class.
        /// </summary>
        /// <param name="core">Engine core.</param>
        /// <param name="name">Name of the Scene.</param>
        /// <param name="textures">Texture root path.</param>
        /// <param name="music">Music root path.</param>
        /// <param name="fonts">Font root path.</param>
        /// <param name="sfx">Sound effects root path.</param>
        public Scene(Core core, String name, String textures, String music, String fonts, String sfx, Input input = null) : base(core)
        {
            // Init
            Name = String.IsNullOrWhiteSpace(name) ? GetType().Name : name;
            _Input = input;

            // Create Asset Managers
            TextureLoader = new TextureLoader(textures);
            MusicLoader = new MusicLoader(music);
            FontLoader = new FontLoader(fonts);
            SfxLoader = new SfxLoader(sfx);

            // Create Default Layer Structure
            // Game Layer
            Layer_Background = new Layer(_Core) { Name = nameof(Layer_Background) };
            Layer_Game = new Layer(_Core) { Name = nameof(Layer_Game) };
            Layer_Overlay = new Layer(_Core) { Name = nameof(Layer_Overlay) };
            // System Layer
            Layer_Debug = new Layer(_Core) { Name = nameof(Layer_Debug) };
            Layer_Cursor = new CursorLayer(_Core) { Name = nameof(Layer_Cursor) };
        }


        // Methods #########################################################################
        /// <summary>
        /// Handles the debug changed event.
        /// </summary>
        /// <param name="debug">Current debug value</param>
        private void HandleDebugChanged(bool debug)
        {
            // Show / Hide Performance Monitor
            if (debug)
            {
                _PerformanceMonitor = _PerformanceMonitor ?? new PerformanceMonitor(_Core);
                Layer_Debug.Add(_PerformanceMonitor);
            }
            else if(_PerformanceMonitor != null)
            {
                Layer_Debug.Remove(_PerformanceMonitor);
            }

            // Pass debug mode to the asset managers
            TextureLoader.Debug = debug;
            MusicLoader.Debug = debug;
            FontLoader.Debug = debug;
            SfxLoader.Debug = debug;
        }

        /// <summary>
        /// Manually opens / closes the console.
        /// </summary>
        protected void ToggleConsole()
        {
            if (_Core._Console.IsOpen)
                _Core._Console.Close();
            else
                _Core._Console.Open();
        }

        /// <summary>
        /// Draws this Scene onto the scene.
        /// </summary>
        internal void Draw()
        {
            Layer_Background.Draw();
            Layer_Game.Draw();
            Layer_Overlay.Draw();
            Layer_Debug.Draw();
            Layer_Cursor.Draw();
        }

        /// <summary>
        /// Loads the required data for this Scene.
        /// </summary>
        /// <returns>True on success.</returns>
        internal bool LoadInternal()
        {
            if (Destroyed) throw new InvalidStateException($"Attempt to load destroyed Scene {Name}");

            if (Load())
            {
                HandleDebugChanged(_Core.Debug);
                _Core.DebugChanged += HandleDebugChanged;

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
        /// Updates the Scene and its children.
        /// </summary>
        /// <param name="deltaT">Current frame time.</param>
        internal void UpdateInternal(float deltaT)
        {
            Layer_Background.Update(deltaT);
            Layer_Game.Update(deltaT);
            Layer_Overlay.Update(deltaT);
            Layer_Debug.Update(deltaT);
            Layer_Cursor.Update(deltaT);
            Update(deltaT);
        }

        /// <summary>
        /// Destroys the Scene.
        /// </summary>
        internal void DestroyInternal()
        {
            Destroyed = true;

            _Core.DebugChanged -= HandleDebugChanged;

            OnDestroy.Invoke();
            Destroy();

            SfxLoader.Dispose();
            FontLoader.Dispose();
            MusicLoader.Dispose();
            TextureLoader.Dispose();

            Layer_Background.Dispose();
            Layer_Game.Dispose();
            Layer_Overlay.Dispose();
            Layer_Debug.Dispose();
            Layer_Cursor.Dispose();

            if (_DefaultInput != null) _DefaultInput.Dispose();
            _Input = _DefaultInput = null;

            Log.Debug(Name, "destroyed");
        }

        /// <summary>
        /// Loads the required data for this Scene.
        /// </summary>
        /// <returns>True on success.</returns>
        protected abstract Boolean Load();

        /// <summary>
        /// Called each Frame to update the scenes user code.
        /// </summary>
        /// <param name="deltaT">Current frame time.</param>
        protected abstract void Update(float deltaT);

        /// <summary>
        /// Cleanup everything that is not managed by either the scene graph or the asset loaders.
        /// </summary>
        protected abstract void Destroy();

        /// <summary>Returns a <see cref="System.String" /> that represents this instance.</summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString() => $"{base.ToString()} \"{Name}\"";
    }
}