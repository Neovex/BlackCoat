using System;
using System.Linq;
using System.Threading;
using System.Diagnostics;

using SFML.System;
using SFML.Window;
using SFML.Graphics;

using BlackCoat.Tools;
using BlackCoat.Animation;
using BlackCoat.Properties;
using BlackCoat.Collision;


namespace BlackCoat
{
    /// <summary>
    /// Black Coat Core Class - represents the main controller for all rendering, logic and engine related operations.
    /// This class does not support multi-threading.
    /// </summary>
    public sealed class Core : IDisposable
    {
        internal static int DRAW_CALLS = 0;

        // Events ##########################################################################
        /// <summary>
        /// The Update Event is raised for each frame. This event can be used to update code areas which are not directly linked to the engine itself.
        /// </summary>
        public event Action<float> OnUpdate = d => { };

        /// <summary>
        /// Occurs when a console command is issued that is not handled by the engine itself.
        /// </summary>
        public event Func<String, Boolean> ConsoleCommand = c => false;

        /// <summary>
        /// Occurs when the engine enters or leaves the debug mode.
        /// </summary>
        public event Action<Boolean> DebugChanged = d => { };


        // Variables #######################################################################
        private RenderWindow _Device;
        private Stopwatch _Timer;
        private Boolean _Debug;
        private CollisionSystem _CollisionSystem;
        private Tools.Console _Console;


        // Properties ######################################################################
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Core"/> is currently in debug mode.
        /// </summary>
        public Boolean Debug
        {
            get { return _Debug; }
            set
            {
                if (_Debug != value)
                {
                    _Debug = value;
                    if (_Debug)
                    {
                        _Device.KeyPressed += QuitOnEsc;
                        Log.Info("Debug enabled");
                    }
                    else
                    {
                        _Device.KeyPressed -= QuitOnEsc;
                        Log.Info("Debug disabled");
                    }
                    DebugChanged.Invoke(_Debug);
                }
            }
        }

        /// <summary>
        /// Random Number Generator with float and integer support.
        /// </summary>
        public RandomHelper Random { get; private set; }

        /// <summary>
        /// Game State Manager. Manages the current Gamestate as well as Gamestate transitions.
        /// </summary>
        public StateManager StateManager { get; private set; }

        /// <summary>
        /// Animation Manager and Factory. Used primarily to make stuff move.
        /// </summary>
        public AnimationManager AnimationManager { get; private set; }

        /// <summary>
        /// Offers methods to calculate collisions between geometric primitives. Must never be null.
        /// </summary>
        public CollisionSystem CollisionSystem
        {
            get { return _CollisionSystem; }
            set
            {
                if (value == null) throw new Exception($"{nameof(CollisionSystem)} must never be null");
                _CollisionSystem = value;
            }
        }

        /// <summary>
        /// Color used to clear the screen of the contents from the last rendered frame.
        /// </summary>
        public Color ClearColor { get; set; }

        /// <summary>
        /// Determines if the render window is no longer in focus.
        /// </summary>
        public Boolean FocusLost { get; private set; }

        /// <summary>
        /// Determines if the Engine Core has been disposed.
        /// </summary>
        public Boolean Disposed { get; private set; }

        /// <summary>
        /// Size of the current Render Device
        /// </summary>
        public Vector2u DeviceSize { get { return _Device.Size; } }

        /// <summary>
        /// Default Render View - uses full with and height of the rendering device.
        /// </summary>
        internal View DefaultView { get; private set; }

        /// <summary>
        /// Default font of the Engine. The <see cref="TextItem"/> class needs it to display text when no font is loaded.
        /// </summary>
        public Font DefaultFont { get; private set; }


        // CTOR ############################################################################
        /// <summary>
        /// Creates a new Instance of the BlackCoat Core class
        /// </summary>
        /// <param name="device">Render Device used by the Core - use of the static creation methods is recommended</param>
        public Core(RenderWindow device)
        {
            Log.Info("Initializing Black Coat Engine...");

            // Init Backend
            _Device = device ?? throw new ArgumentNullException(nameof(device));
            _Timer = new Stopwatch();

            // Init Defaults
            ClearColor = Color.Black;
            FocusLost = false;
            Disposed = false;
            DefaultView = _Device.DefaultView;
            DefaultFont = new Font(Resources.Squares_Bold_Free);
            for (uint i = 4; i <= 32; i += 2) InitializeFontHack(DefaultFont, i); // HACK

            // Device Events
            _Device.Resized += HandleWindowResized;
            _Device.Closed += HandleWindowClose;
            _Device.LostFocus += HandleLostFocus;
            _Device.GainedFocus += HandleGainedFocus;

            // Init Subsystems
            Random = new RandomHelper();
            StateManager = new StateManager(this);
            AnimationManager = new AnimationManager();
            CollisionSystem = new CollisionSystem();

            // Init Input
            Input.Initialize(_Device);

            // Init Console
            _Console = new Tools.Console(this, _Device);
            _Console.Command += HandleConsoleCommand;

            Log.Info("Black Coat Engine Creation Completed. - Version", GetType().Assembly.GetName().Version);
        }

        ~Core()
        {
            if (!Disposed) Dispose();
        }

        // HACK
        public void InitializeFontHack(Font font, uint charSize = 10, bool bold = false)
        {
            var text = new Text("0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ.:-_,;#'+*~´`\\?ß=()&/$%\"§!^°", font, charSize);
            if (bold) text.Style = Text.Styles.Bold;
            text.Draw(_Device, RenderStates.Default);
            text.Dispose();
        }


        // Methods #########################################################################
        /// <summary>
        /// Displays the Renderwindow to the User.
        /// </summary>
        public void ShowRenderWindow()
        {
            if (Disposed) throw new ObjectDisposedException("Core");
            _Device.SetVisible(true);
        }

        /// <summary>
        /// Hides the Renderwindow to the User without closing it.
        /// Use ShowRenderWindow() to reveal it again.
        /// </summary>
        public void HideRenderWindow()
        {
            if (Disposed) throw new ObjectDisposedException("Core");
            _Device.SetVisible(false);
        }

        /// <summary>
        /// Sets the icon of the render window and the associated task bar button.
        /// </summary>
        /// <param name="icon">Texture used to set the icon from</param>
        public void SetRenderWindowIcon(Texture icon)
        {
            if (icon == null) throw new ArgumentNullException("icon");
            using (var img = icon.CopyToImage())
            {
                _Device.SetIcon(img.Size.X, img.Size.Y, img.Pixels);
            }
        }

        /// <summary>
        /// Begins the Update / Rendering Loop.
        /// This method is blocking until Exit() is called.
        /// </summary>
        public void Run()
        {
            if (Disposed) throw new ObjectDisposedException("Core");
            Log.Info("Engine Started");
            ShowRenderWindow();
            while (_Device.IsOpen)
            {
                _Device.DispatchEvents();
                if (FocusLost) // pause updating & relieve host machine
                {
                    Thread.Sleep(1);
                }
                else // run updates
                {
                    var deltaT = (float)(_Timer.Elapsed.TotalMilliseconds / 1000d);// fractal second
                    _Timer.Restart();
                    Update(deltaT);
                }
                Draw();
            }
            _Timer.Stop();
            Log.Info("Engine stopped.");
        }

        /// <summary>
        /// Performs a single update followed by a single render call. Resulting in a single Device refresh.
        /// </summary>
        /// <param name="deltaT">Optional time delta between frames in fractal seconds</param>
        public void ManualRefresh(float deltaT = 0)
        {
            if (Disposed) throw new ObjectDisposedException("Core");
            if (!_Device.IsOpen) throw new InvalidOperationException("Device not ready");
            _Device.DispatchEvents();
            Update(deltaT);
            Draw();
        }

        /// <summary>
        /// Exits the Update / Rendering Loop (if present), closes and disposes the Renderwindow
        /// </summary>
        public void Exit()
        {
            if (Disposed) throw new ObjectDisposedException("Core");
            _Device.Close();
        }

        /// <summary>
        /// Calls the Update methods of the SceneGraph Hierarchy
        /// </summary>
        /// <param name="deltaT">Frame time</param>
        private void Update(Single deltaT)
        {
            // Update Scene Graph
            StateManager.Update(deltaT);

            // Update running Animations
            AnimationManager.Update(deltaT);

            // Raise Update event for external updates
            OnUpdate(deltaT);
        }

        /// <summary>
        /// Draws the SceneGraph to the Render Device
        /// </summary>
        private void Draw()
        {
            // Clear Background
            _Device.Clear(ClearColor);

            // Draw Scene
            DRAW_CALLS = 0;
            StateManager.Draw();
            _Console.Draw();

            // Present Backbuffer
            _Device.Display();
        }

        /// <summary>
        /// Draws the provided Entity onto the scene.
        /// </summary>
        /// <param name="entity">The Entity to draw</param>
        public void Draw(IEntity entity)
        {
            if (!entity.Visible) return;

            var state = entity.RenderState;
            if (entity.Parent != null)
            {
                // get transformation inheritance of all parents and update the renderstate
                // in the SFML draw method the combined Parent transformations are multiplied by the entities own transformation
                state.Transform = entity.Parent.Transform;

                // get alpha inheritance and update the entities color alpha component
                var color = entity.Color;
                color.A = (Byte)(entity.Alpha * Byte.MaxValue);
                entity.Color = color;
            }

            if (entity.RenderTarget == null)
            {
                if (entity.View == null) // view inheritance is handled by the property
                {
                    entity.Draw(_Device, state);
                }
                else
                {
                    _Device.SetView(entity.View);
                    entity.Draw(_Device, state);
                    _Device.SetView(DefaultView);
                }
            }
            else // handle custom render targets
            {
                if (entity.View == null)
                {
                    entity.Draw(entity.RenderTarget, state);
                }
                else
                {
                    var restore = entity.RenderTarget.GetView();
                    entity.RenderTarget.SetView(entity.View);
                    entity.Draw(entity.RenderTarget, state);
                    entity.RenderTarget.SetView(restore);
                }
            }
            DRAW_CALLS++;
        }

        /// <summary>
        /// Handles Core Commands and broadcasts other Commands to the remaining application
        /// </summary>
        /// <param name="cmd">Console input</param>
        private void HandleConsoleCommand(String cmd)
        {
            // Handle Core Commands first
            switch (cmd.ToLower())
            {
                case "exit":
                case "quit":
                    Log.Debug("Beginning Engine Shutdown");
                    Exit();
                    return;
                case "togglefullscreen":
                case "tfs":
                    Log.Warning("togglefullscreen - not yet implemented"); // TODO
                    return;
                case "toggledebug":
                case "debug":
                    Debug = !Debug;
                    return;
            }

            // Then broadcast Commands to all other systems and the client application
            if (ConsoleCommand.GetInvocationList().All(listener => !(bool)listener.DynamicInvoke(cmd)))
            {
                Log.Warning("Unknown Command:", cmd);
            }
        }

        #region Debug Handler
        private void QuitOnEsc(object s, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Escape) Exit();
        }
        #endregion

        #region Device Event handlers
        private void HandleWindowResized(object sender, SizeEventArgs e)
        {
            // TODO : device resize
            Log.Fatal("Resize not handled!");
        }

        private void HandleWindowClose(object sender, EventArgs e)
        {
            // Needed for when the close button is clicked
            Exit();
        }

        private void HandleLostFocus(object sender, EventArgs e)
        {
            _Timer.Stop();
            FocusLost = true;
            Input.Reset();
        }

        private void HandleGainedFocus(object sender, EventArgs e)
        {
            FocusLost = false;
            _Timer.Start();
        }
        #endregion

        /// <summary>
        /// Releases all unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;

            StateManager.Destroy();

            if (_Device.CPointer != IntPtr.Zero)
            {
                if (_Device.IsOpen) _Device.Close();
                _Device.Dispose();
            }
            _Device = null;

            DefaultFont.Dispose();
            DefaultFont = null;

            Log.Info("Engine Destroyed");
        }


        #region STATICS
        /// <summary>
        /// Initializes a default Graphic Device primarily for testing purposes
        /// </summary>
        /// <returns>The default device</returns>
        public static RenderWindow DefaultDevice { get { return new RenderWindow(new VideoMode(800, 600), "Default", Styles.Titlebar); } }

        /// <summary>
        /// Initializes a default Graphic Device mimicking the current desktop
        /// </summary>
        /// <returns>The desktop device</returns>
        public static RenderWindow DesktopDevice { get { return new RenderWindow(VideoMode.DesktopMode, String.Empty, Styles.Fullscreen); } }

        /// <summary>
        /// Initializes a new Graphic Device
        /// </summary>
        /// <param name="deviceWidth">With of the Backbuffer</param>
        /// <param name="deviceHeight">Height of the Backbuffer</param>
        /// <param name="title">Title of the Renderwindow</param>
        /// <param name="style">Display Style of the Device/Window</param>
        /// <param name="antialiasing">Determines the Anti-aliasing</param>
        /// <param name="skipValidityCheck">Skips the device validation (not recommended but required for non-standard resolutions)</param>
        /// <returns>The Initialized RenderWindow or null if the Device could not be created</returns>
        public static RenderWindow CreateDevice(UInt32 deviceWidth, UInt32 deviceHeight, String title, Styles style, UInt32 antialiasing, Boolean skipValidityCheck = false)
        {
            var settings = new ContextSettings(24, 8, antialiasing);
            var videoMode = new VideoMode(deviceWidth, deviceHeight);
            if (skipValidityCheck || videoMode.IsValid())
            {
                return new RenderWindow(videoMode, title, style, settings);
            }
            return null;
        }
        /// <summary>
        /// Initializes a new Graphic Device based on a window or control handle
        /// </summary>
        /// <param name="handle">Handle to create the device on</param>
        /// <param name="antialiasing">Determines the Anti-aliasing</param>
        /// <returns>The Initialized RenderWindow instance based on the device</returns>
        public static RenderWindow CreateDevice(IntPtr handle, UInt32 antialiasing)
        {
            if (handle == IntPtr.Zero) throw new NullReferenceException("Device creation failed since handle is Zero");
            var settings = new ContextSettings(24, 8, antialiasing);
            return new RenderWindow(handle, settings);
        }
        #endregion
    }
}