using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using SFML.Graphics;
using SFML.Window;

using BlackCoat.Entities;
using BlackCoat.Tools;

namespace BlackCoat
{
    /// <summary>
    /// Render Core Class - represents the main controller for all rendering and logic operations.
    /// This class does not support multi threading.
    /// </summary>
    public sealed class Core : IDisposable
    {
        // Variables #######################################################################
        // System
        private RenderWindow _Device;


        // Properties ######################################################################
        // System
        public AssetManager AssetManager { get; private set; }
        public RandomHelper Random { get; private set; }
        public Boolean FocusLost { get; private set; }
        public Boolean Disposed { get; private set; }

        // Layers
        public Layer Layer_BG { get; private set; }
        public Layer Layer_Game { get; private set; }
        public Layer Layer_Particles { get; private set; }
        public Layer Layer_Overlay { get; private set; }
        public Layer Layer_Debug { get; private set; }
        public Layer Layer_Cursor { get; private set; }

        // Misc
        public Color ClearColor { get; set; }
        public Font DefaultFont { get; private set; }
        public View DefaultView { get; private set; }


        // CTOR ############################################################################
        /// <summary>
        /// Creates a new Instance of the BlackCoat Core class
        /// </summary>
        /// <param name="device">Render Device used by the Core - use of the static creation methods is recommended</param>
        public Core(RenderWindow device) : this(device, false)
        { }

        /// <summary>
        /// Creates a new Instance of the BlackCoat Core class
        /// </summary>
        /// <param name="device">Render Device used by the Core - the use the static creation methods is recommended</param>
        /// <param name="debug">Determines if the Core should enable debug features</param>
        public Core(RenderWindow device, Boolean debug)
        {
            // Save Device Ref
            if (device == null) throw new ArgumentNullException("device");
            _Device = device;
            DefaultView = new View(_Device.GetView());
            // Device Events
            _Device.Closed += new EventHandler(_Device_Closed);
            _Device.LostFocus += new EventHandler(HandleLostFocus);
            _Device.GainedFocus += new EventHandler(HandleGainedFocus);
            
            // Init Subsystems
            FocusLost = false;
            AssetManager = new AssetManager(this);
            Random = new RandomHelper();
            ClearColor = Color.Black;
            DefaultFont = new Font(@"C:\Windows\Fonts\arial.ttf"); // TODO : FIXME

            // Init Input
            Input.Initialize(_Device);

            // Create Layer System
            Layer_BG = new Layer(this);
            Layer_Game = new Layer(this);
            Layer_Particles = new Layer(this);
            Layer_Overlay = new Layer(this);
            Layer_Debug = new Layer(this);
            Layer_Cursor = new Layer(this);

            if (debug)
            {
                // Initialize Performance Monitoring
                Layer_Debug.AddChild(new PerformanceMonitor(this, _Device));

                // Input
                _Device.KeyPressed += _Device_KeyPressed;
                //_Device.CurrentView.Zoom(0.5f);//debug overview
            }
        }

        ~Core()
        {
            if (!Disposed) Dispose();
        }
        

        private void _Device_KeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Escape) Exit();
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
        /// Begins the Update / Rendering Loop.
        /// This method is blocking until Exit() is called.
        /// </summary>
        /// <param name="updateGameDelegate">Update callback for the calling class</param>
        public void Run(Action<float> updateGameDelegate)
        {
            if (Disposed) throw new ObjectDisposedException("Core");
            ShowRenderWindow();
            Stopwatch timer = new Stopwatch();
            Single deltaT = 0;
            while (_Device.IsOpen)
            {
                _Device.DispatchEvents();
                deltaT = (float)(timer.Elapsed.TotalMilliseconds / 1000d); // fractal second
                timer.Reset();
                timer.Start();
                if (FocusLost) // pause updating & relieve host machine
                {
                    Thread.Sleep(1);
                }
                else // run updates
                {
                    Update(deltaT);
                    updateGameDelegate(deltaT);
                }
                Draw();
            }
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
        
        // layer update root
        private void Update(Single deltaT)
        {
            // update layers
            Layer_BG.Update(deltaT);
            Layer_Game.Update(deltaT);
            Layer_Particles.Update(deltaT);
            Layer_Overlay.Update(deltaT);
            Layer_Debug.Update(deltaT);
        }

        // draw layer roots
        private void Draw()
        {
            // Clear Background
            _Device.Clear(ClearColor);
            
            // Draw Layers
            Layer_BG.Draw();
            Layer_Game.Draw();
            Layer_Particles.Draw();
            Layer_Overlay.Draw();
            // Overlay
            Layer_Debug.Draw();
            Layer_Cursor.Draw();

            // Present Backbuffer
            _Device.Display();
        }

        public void Draw(IEntity e)
        {
            if (!e.Visible) return;
            if (e.View != null) _Device.SetView(e.View);
            if (e.Parent != null)
            {
                var state = e.RenderState;
                state.Transform = e.Parent.Transform;
                e.RenderState = state;
            }
            _Device.Draw(e, e.RenderState);
        }

        public void Draw(Vertex[] vertices, PrimitiveType type, RenderStates states)
        {
            _Device.Draw(vertices, type, states);
        }

        /// <summary>Logs Messages to the Console</summary>
        /// <param name="logs">Objects to log</param>
        internal void Log(params object[] logs)
        {
            Console.WriteLine(String.Join(" ", logs.Select(l => l.ToString())));
        }

        // Device Event Handlers ############################################################
        private void HandleLostFocus(object sender, EventArgs e)
        {
            FocusLost = true;
            Input.Reset();
        }

        private void HandleGainedFocus(object sender, EventArgs e)
        {
            FocusLost = false;
        }

        private void _Device_Closed(object sender, EventArgs e)
        {
            Exit();
        }


        /// <summary>
        /// Releases all used unmanaged resources.
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
        public static RenderWindow DefaultDevice
        {
            get
            {
                return new RenderWindow(new VideoMode(800, 600), "Default", Styles.Titlebar);
            }
        }

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