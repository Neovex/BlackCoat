using BlackCoat.Entities;
using BlackCoat.Tools;
using SFML.Graphics;
using SFML.Window;
using System;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace BlackCoat
{
    /// <summary>
    /// BC Core Class - represents the main controller for all rendering and logic operations.
    /// This class does not support multi threading.
    /// </summary>
    public sealed class Core : IDisposable
    {
        // Events ##########################################################################
        /*public event EventHandler DeviceCreated;
        public event EventHandler DeviceReset;
        public event EventHandler DeviceResetting;
        public event EventHandler DeviceDisposing;*/


        // Variables #######################################################################
        // System
        private RenderWindow _Device;
        private AssetManager _AssetManager;
        private RandomHelper _Random;
        private Boolean _FocusLost = false;
        // Views
        private View _DefaultView;
        private View _OverlayView;
        // Layers
        private GraphicLayer _LayerBackground;
        private GraphicLayer _LayerGame;
        private GraphicLayer _LayerParticles;
        private GraphicLayer _LayerOverlay;
        private GraphicLayer _LayerDebug;
        private GraphicLayer _LayerCursor;
        // Speed
        private RenderStates _RenderHelper = RenderStates.Default;


        // Properties ######################################################################
        // System
        public RenderWindow Device
        {
            get { return _Device; }
            //set
            //{
            //    if (Disposed) throw new ObjectDisposedException("Core");
            //    if (value == null) throw new ArgumentNullException("Render device cannot be null");
            //    _Device = value;
            //    // TODO : update all event listeners and check if some other events need to be (re-)fired
            //}
        }
        public AssetManager AssetManager { get { return _AssetManager; } }
        public RandomHelper Random { get { return _Random; } }
        public Boolean FocusLost { get { return _FocusLost; } }
        public Boolean Disposed { get; private set; }

        // Layers
        public GraphicLayer Layer_BG { get { return _LayerBackground; } }
        public GraphicLayer Layer_Game { get { return _LayerGame; } }
        public GraphicLayer Layer_Particles { get { return _LayerParticles; } }
        public GraphicLayer Layer_Overlay { get { return _LayerOverlay; } }
        public GraphicLayer Layer_Debug { get { return _LayerDebug; } }
        public GraphicLayer Layer_Cursor { get { return _LayerCursor; } }

        // Misc
        public View DefaultView { get { return _DefaultView; } }
        public View OverlayView { get { return _OverlayView; } }
        public Color ClearColor { get; set; }
        public Font DefaultFont { get; private set; }


        /// <summary>
        /// Current Mouse Position
        /// </summary>
        public Vector2i MousePosition
        {
            get
            {
                if (Disposed) throw new ObjectDisposedException("Core");
                return _Device.InternalGetMousePosition();
            }
        }


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
            _DefaultView = _Device.DefaultView;
            _OverlayView = new View(_DefaultView);
            // Device Events
            _Device.Closed += new EventHandler(_Device_Closed);
            _Device.LostFocus += new EventHandler(HandleLostFocus);
            _Device.GainedFocus += new EventHandler(HandleGainedFocus);
            
            // Init Subsystems
            _AssetManager = new AssetManager(this);
            _Random = new RandomHelper();
            ClearColor = Color.Black;
            DefaultFont = new Font(@"C:\Windows\Fonts\arial.ttf"); // FIXME

            // Init Input
            Input.InitializeInternal(this);

            // Create Layer System
            _LayerBackground = new GraphicLayer(this);
            _LayerGame = new GraphicLayer(this);
            _LayerParticles = new GraphicLayer(this);
            _LayerOverlay = new GraphicLayer(this);
            _LayerDebug = new GraphicLayer(this);
            _LayerCursor = new GraphicLayer(this);

            if (debug)
            {
                // Initialize Performance Monitoring
                _LayerDebug.AddChild(new PerformanceMonitor(this));

                // Input
                //_Device.KeyPressed += _Device_KeyPressed;
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

        /// <summary>
        /// Initializes a default Graphic Device primarily for testing purposes
        /// </summary>
        /// <returns>The default device</returns>
        public static RenderWindow CreateDefaultDevice()
        {
            return new RenderWindow(new VideoMode(800, 600), "BlackCoat Game", Styles.Titlebar);
        }

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
        /// Use ShowRenderWindow() to un-hide.
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
            while (_Device.IsOpen())
            {
                _Device.DispatchEvents();
                deltaT = (Single)(timer.Elapsed.TotalMilliseconds / 1000d); // fractal second
                timer.Reset();
                timer.Start();
                if (_FocusLost) // pause updating & relieve host machine
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
        /// Manually runs a single update followed by a single render call.
        /// This method will fail if the the device is invalid
        /// </summary>
        /// <param name="deltaT">Optional time delta between frames in fractional seconds</param>
        public void PerformManualRefresh(float deltaT = 0)
        {
            if (Disposed) throw new ObjectDisposedException("Core");
            if (!_Device.IsOpen()) throw new InvalidOperationException("Device not ready");
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
            _LayerBackground.Update(deltaT);
            _LayerGame.Update(deltaT);
            _LayerParticles.Update(deltaT);
            _LayerOverlay.Update(deltaT);
            _LayerDebug.Update(deltaT);
        }

        // draw layer roots
        private void Draw()
        {
            // Clear Background
            _Device.Clear(ClearColor);

            // Draw Layers
            _Device.SetView(_DefaultView);
            _LayerBackground.Draw();
            _LayerGame.Draw();
            _LayerParticles.Draw();
            _LayerOverlay.Draw();
            // Overlay
            _Device.SetView(_OverlayView);
            _LayerDebug.Draw();
            _LayerCursor.Draw();

            // Present Backbuffer
            _Device.Display();
        }

        /// <summary>
        /// Renders a drawable onto the backbuffer
        /// </summary>
        /// <param name="graphicItem">The item to render</param>
        /// <param name="parent">Scene Graph parent of the item to render</param>
        internal void Render(Drawable graphicItem, Container parent)
        {
            _RenderHelper.Transform = parent.Transform;
            _Device.Draw(graphicItem, _RenderHelper);
        }

        /// <summary>
        /// Renders a drawable onto the backbuffer
        /// </summary>
        /// <param name="graphicItem">The item to render</param>
        internal void Render(Drawable graphicItem)
        {
            _Device.Draw(graphicItem);
        }

        /// <summary>Logs Message to the default output</summary>
        /// <param name="logs">Objects to log</param>
        public void Log(params object[] logs)
        {
            if (Disposed) throw new ObjectDisposedException("Core");
            foreach (var log in logs)
                Console.WriteLine(log);
            Console.WriteLine();
        }

        // Device Event Handlers ############################################################
        private void HandleLostFocus(object sender, EventArgs e)
        {
            _FocusLost = true;
            Input.Reset();
        }

        private void HandleGainedFocus(object sender, EventArgs e)
        {
            _FocusLost = false;
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
                if (_Device.IsOpen()) _Device.Close();
                _Device.Dispose();
            }
            _Device = null;
            
            _AssetManager.Dispose();
            _AssetManager = null;

            Disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}