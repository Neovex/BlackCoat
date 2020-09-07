using System;
using BlackCoat.Entities;
using BlackCoat.Tools;
using SFML.Graphics;
using SFML.System;

namespace BlackCoat
{
    /// <summary>
    /// Base class for all Scenes
    /// </summary>
    public abstract class Scene : BlackCoatBase
    {
        private PerformanceMonitor _PerformanceMonitor;
        private PropertyInspector _PropertyInspector;
        private Input _DefaultInput;
        private Input _Input;

        // Scene Info
        public String Name { get; protected set; }
        public Boolean Destroyed { get; private set; }

        // Input
        public Input Input
        {
            get => _Input ?? _DefaultInput ?? (_DefaultInput = new Input(_Core));
            protected set => _Input = Layer_Cursor.Input = value;
        }

        // Asset Managers
        protected internal TextureLoader TextureLoader { get; }
        protected internal MusicLoader MusicLoader { get; }
        protected internal FontLoader FontLoader { get; }
        protected internal SfxLoader SfxLoader { get; }

        // Layers
        protected internal Layer Layer_BG { get; }
        protected internal Layer Layer_Game { get; }
        protected internal Layer Layer_Overlay { get; }
        protected internal Layer Layer_Debug { get; }
        protected internal CursorLayer Layer_Cursor { get; }

        // Events

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
            Layer_BG = new Layer(_Core);
            Layer_Game = new Layer(_Core);
            Layer_Overlay = new Layer(_Core);
            // System Layer
            Layer_Debug = new Layer(_Core);
            Layer_Cursor = new CursorLayer(_Core, Input);

            // Handle Debug Features
            HandleDebugChanged(_Core.Debug);
            _Core.DebugChanged += HandleDebugChanged;
            _Core.ConsoleCommand += HandleConsoleCommand;
        }


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

            // Pass debug mode on to the asset managers
            TextureLoader.Debug = debug;
            MusicLoader.Debug = debug;
            FontLoader.Debug = debug;
            SfxLoader.Debug = debug;
        }

        private bool HandleConsoleCommand(string cmd)
        {
            if (_Core.Debug && cmd == "inspect")
            {
                OpenInspector();
                ToggleConsole();
                return true;
            }
            return false;
        }

        protected IPropertyInspector OpenInspector(object target = null)
        {
            _PropertyInspector = _PropertyInspector ?? new PropertyInspector(_Core, TextureLoader);
            _PropertyInspector.Clear();
            _PropertyInspector.Add(target);
            _PropertyInspector.Add(this);
            _PropertyInspector.Show();
            return _PropertyInspector;
        }

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
            Layer_BG.Draw();
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
            Layer_BG.Update(deltaT);
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

            if (_DefaultInput != null) _DefaultInput.Dispose();
            _Input = _DefaultInput = null;

            _Core.DebugChanged -= HandleDebugChanged;
            _Core.ConsoleCommand -= HandleConsoleCommand;

            _PropertyInspector?.Destroy();

            OnDestroy.Invoke();
            Destroy();

            SfxLoader.Dispose();
            FontLoader.Dispose();
            MusicLoader.Dispose();
            TextureLoader.Dispose();

            Layer_BG.Dispose();
            Layer_Game.Dispose();
            Layer_Overlay.Dispose();
            Layer_Debug.Dispose();
            Layer_Cursor.Dispose();

            Log.Debug(Name, "destroyed");
        }

        /// <summary>
        /// Loads the required data for this Scene.
        /// </summary>
        /// <returns>True on success.</returns>
        protected abstract Boolean Load();

        /// <summary>
        /// Updates the Scene and its children.
        /// </summary>
        /// <param name="deltaT">Current frame time.</param>
        protected abstract void Update(float deltaT);

        /// <summary>
        /// Destroys the Scene.
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