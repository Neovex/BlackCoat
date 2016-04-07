using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using SFML.System;
using SFML.Window;
using SFML.Graphics;

using BlackCoat.Entities;
using BlackCoat.Tools;
using BlackCoat.Animation;

namespace BlackCoat
{
    /// <summary>
    /// Render Core Class - represents the main controller for all rendering, logic and engine related operations.
    /// This class does not support multi-threading.
    /// </summary>
    public sealed class Core : IDisposable
    {
        // Events ##########################################################################
        /// <summary>
        /// Update Event is raised for each frame. This event can be used to update external components such as a game instance.
        /// </summary>
        public event Action<float> OnUpdate = d => { };
        public event Action<String> OnLog = System.Console.WriteLine;
        public event Func<String, Boolean> ConsoleCommand = c => false;

        
        // Variables #######################################################################
        private RenderWindow _Device;
        private Stopwatch _Timer;
        private Tools.Console _Console;


        // Properties ######################################################################
        // Subsystems
        /// <summary>
        /// Default Asset Manager. Handles loading/unloading of assets located in its specified root folder.
        /// </summary>
        public AssetManager AssetManager { get; private set; }
        /// <summary>
        /// Random Number Generator with float and integer support.
        /// </summary>
        public RandomHelper Random { get; private set; }
        /// <summary>
        /// Animation Manager and Factory. Used primarly to make stuff move.
        /// </summary>
        public AnimationManager AnimationManager { get; private set; }

        // Layers
        public Layer Layer_BG { get; private set; }
        public Layer Layer_Game { get; private set; }
        public Layer Layer_Particles { get; private set; }
        public Layer Layer_Overlay { get; private set; }
        public Layer Layer_Debug { get; private set; }
        public Layer Layer_Cursor { get; private set; }

        // Misc
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
        internal View DefaultView { get { return _Device.DefaultView; } }
        /// <summary>
        /// Default font of the Engine. The <see cref="TextItem"/> class needs it to display text when no font is loaded.
        /// </summary>
        internal Font DefaultFont { get; private set; }


        // CTOR ############################################################################
        /// <summary>
        /// Creates a new Instance of the BlackCoat Core class
        /// </summary>
        /// <param name="device">Render Device used by the Core - use of the static creation methods is recommented</param>
        public Core(RenderWindow device) : this(device, false)
        { }

        /// <summary>
        /// Creates a new Instance of the BlackCoat Core class
        /// </summary>
        /// <param name="device">Render Device used by the Core - use of the static creation methods is recommented</param>
        /// <param name="debug">Determines if the Core should enable debug features</param>
        public Core(RenderWindow device, Boolean debug)
        {
            // Init Core Systems
            if (device == null) throw new ArgumentNullException("device");
            _Device = device;
            _Timer = new Stopwatch();

            // Init Defaults
            ClearColor = Color.Black;
            FocusLost = false;
            Disposed = false;
            DefaultFont = new Font(@"C:\Windows\Fonts\arial.ttf"); // TODO : FIXME -> hardcoded path

            // Device Events
            _Device.Closed += new EventHandler(_Device_Closed);
            _Device.LostFocus += new EventHandler(HandleLostFocus);
            _Device.GainedFocus += new EventHandler(HandleGainedFocus);

            // Init Subsystems
            AssetManager = new AssetManager(this);
            Random = new RandomHelper();
            AnimationManager = new AnimationManager();

            // Init Input
            Input.Initialize(_Device);

            // Create Layer System
            Layer_BG = new Layer(this);
            Layer_Game = new Layer(this);
            Layer_Particles = new Layer(this);
            Layer_Overlay = new Layer(this);
            Layer_Debug = new Layer(this);
            Layer_Cursor = new Layer(this);

            // Init Console
            _Console = new Tools.Console(this, _Device);
            _Console.Command += HandleConsoleCommand;

            if (debug)
            {
                // Initialize Performance Monitoring
                Layer_Debug.AddChild(new PerformanceMonitor(this, _Device));

                // Input
                _Device.KeyPressed += (s, e) => { if (e.Code == Keyboard.Key.Escape) Exit(); };
                //DefaultView.Zoom(2f);//debug overview
            }
        }

        ~Core()
        {
            if (!Disposed) Dispose();
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
        /// Sets the icon of the render window and the associated taskbar button.
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
            // Update SceneGraph
            Layer_BG.Update(deltaT);
            Layer_Game.Update(deltaT);
            Layer_Particles.Update(deltaT);
            Layer_Overlay.Update(deltaT);
            Layer_Debug.Update(deltaT);

            // Raise Update event for external updates
            OnUpdate(deltaT);

            // Update running Animations
            AnimationManager.Update(deltaT);
        }

        /// <summary>
        /// Draws the SceneGraph to the Render Device
        /// </summary>
        private void Draw()
        {
            // Clear Background
            _Device.Clear(ClearColor);
            
            // SceneGraph
            Layer_BG.Draw();
            Layer_Game.Draw();
            Layer_Particles.Draw();
            Layer_Overlay.Draw();
            // System Overlay
            Layer_Debug.Draw();
            Layer_Cursor.Draw();

            // Present Backbuffer
            _Device.Display();
        }

        /// <summary>
        /// Draws the provided Entity onto the scene.
        /// </summary>
        /// <param name="e">The Entity to draw</param>
        public void Draw(IEntity e)
        {
            if (!e.Visible) return;
            _Device.SetView(e.View ?? DefaultView);
            if (e.Parent != null)
            {
                var state = e.RenderState;
                state.Transform = e.Parent.Transform;
                e.RenderState = state;
            }
            _Device.Draw(e, e.RenderState);
        }

        /// <summary>
        /// Draws the provided vertex array onto the scene.
        /// </summary>
        /// <param name="vertices">Vertex array to draw</param>
        /// <param name="type">Type of primitives to draw</param>
        /// <param name="states">Additional state information for rendering</param>
        public void Draw(Vertex[] vertices, PrimitiveType type, RenderStates states)
        {
            _Device.Draw(vertices, type, states);
        }

        /// <summary>Logs Messages to the Console and all registered log handlers</summary>
        /// <param name="logs">Objects to log</param>
        public void Log(params object[] logs)
        {
            OnLog(String.Join(" ", logs.Select(l => l == null ? "null" : l.ToString())));
        }

        /// <summary>
        /// Handles Core Commands and broadcasts other Commands to the remaining application
        /// </summary>
        /// <param name="cmd">Console input</param>
        private void HandleConsoleCommand(String cmd)
        {
            // Handle Core Commands first
            switch (cmd)
            {
                case "exit":
                case "quit":
                    Exit();
                    return;
                case "togglefullscreen":
                    Log("togglefullscreen - not yet implemented");
                    return;
            }

            // Then broadcast Commands to all other systems and the client application
            if (ConsoleCommand.GetInvocationList().All(listener => !(bool)listener.DynamicInvoke(cmd)))
            {
                Log("Unknown Command:", cmd);
            }
        }

        #region Device Eventhandlers
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

        private void _Device_Closed(object sender, EventArgs e)
        {
            Exit();
        }
        #endregion

        /// <summary>
        /// Releases all unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (Disposed) return;

            if (_Device.CPointer != IntPtr.Zero)
            {
                if (_Device.IsOpen) _Device.Close();
                _Device.Dispose();
            }
            _Device = null;

            DefaultFont.Dispose();
            DefaultFont = null;

            AssetManager.Dispose();
            AssetManager = null;

            Disposed = true;
            GC.SuppressFinalize(this);
        }


        #region STATICS
        /// <summary>
        /// Initializes a default Graphic Device primarily for testing purposes
        /// </summary>
        /// <returns>The default device</returns>
        public static RenderWindow DefaultDevice { get { return new RenderWindow(new VideoMode(800, 600), "Default", Styles.Titlebar); } }

        /// <summary>
        /// Initializes a default Graphic Device mimiking the current desktop
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
        /// <param name="antialiasing">Determines the Antialiasing</param>
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
        /// <param name="antialiasing">Determines the Antialiasing</param>
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