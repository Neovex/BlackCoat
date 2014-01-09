using System;
using System.Threading;
using BlackCoat.Tools;
using SFML.Graphics;
using SFML.Window;

namespace BlackCoat
{
    public sealed class Core
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
        private Color _ClearColor = Color.Black;
        private RandomHelper _Random = new RandomHelper();
        private Boolean _FocusLost = false;
        // Layers
        private GraphicLayer _LayerBackground;
        private GraphicLayer _LayerGame;
        private GraphicLayer _LayerParticles;
        private GraphicLayer _LayerOverlay;
        private GraphicLayer _LayerDebug;
        private GraphicLayer _LayerCursor;


        // Properties ######################################################################
        // System
        public Input Input { get { return _Device.Input; } }
        public AssetManager AssetManager { get { return _AssetManager; } }
        public RandomHelper Random { get { return _Random; } }
        public Color ClearColor
        {
            get { return _ClearColor; }
            set { _ClearColor = value; }
        }

        // Layers
        public GraphicLayer Layer_BG { get { return _LayerBackground; } }
        public GraphicLayer Layer_Game { get { return _LayerGame; } }
        public GraphicLayer Layer_Particles { get { return _LayerParticles; } }
        public GraphicLayer Layer_Overlay { get { return _LayerOverlay; } }
        public GraphicLayer Layer_Debug { get { return _LayerDebug; } }
        public GraphicLayer Layer_Cursor { get { return _LayerCursor; } }

        /// <summary>
        /// Current Mouse Position
        /// </summary>
        public Vector2 MousePosition { get { return new Vector2(Input.GetMouseX(), Input.GetMouseY()); } }


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
            _Device.Closed += new EventHandler(_Device_Closed);
            _Device.LostFocus += new EventHandler(HandleLostFocus);
            _Device.GainedFocus += new EventHandler(HandleGainedFocus);

            // Init Content Manager
            _AssetManager = new AssetManager(this);

            // Create Default Font
            //DefaultFont = ContentManager.Load<SpriteFont>("DefaultFont");

            // Create Layersystem
            _LayerBackground = new GraphicLayer(this);
            _LayerGame = new GraphicLayer(this);
            _LayerParticles = new GraphicLayer(this);
            _LayerOverlay = new GraphicLayer(this);
            _LayerDebug = new GraphicLayer(this);
            _LayerCursor = new GraphicLayer(this);

            if (debug)
            {
                // Intialize Performance Monitoring
                PerformanceMonitor perfMon = new PerformanceMonitor(this);
                _LayerDebug.AddChild(perfMon);

            }
            //_Device.CurrentView.Zoom(0.5f);//debug overview
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
        /// <param name="skipValidityCheck">Skips the device validation (not recommented but required for non-standart resolutions)</param>
        /// <returns>The Initialized RenderWindow or null if the Device could not be created</returns>
        public static RenderWindow CreateDevice(UInt32 deviceWidth, UInt32 deviceHeight, String title, Styles style, UInt32 antialiasing, Boolean skipValidityCheck = false)
        {
            var settings = new WindowSettings(24, 8, antialiasing);
            var videoMode = new VideoMode(deviceWidth, deviceHeight);
            if(skipValidityCheck || videoMode.IsValid())
            {
                return new RenderWindow(videoMode, title, style, settings);
            }
            return null;
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
            _Device.Show(true);
        }

        /// <summary>
        /// Hides the Renderwindow to the User without closing it.
        /// Use ShowRenderWindow() to unhide.
        /// </summary>
        public void HideRenderWindow()
        {
            _Device.Show(false);
        }

        /// <summary>
        /// Begins the Update / Rendering Loop.
        /// This method is blocking until Exit() is called.
        /// </summary>
        /// <param name="updateGameDelegate">Update callback for the calling class</param>
        public void Run(Action<float> updateGameDelegate)
        {
            ShowRenderWindow();
            float deltaT = 0;
            while (_Device.IsOpened())
            {
                _Device.DispatchEvents();
                deltaT = _Device.GetFrameTime();
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
            _AssetManager.Dispose();
            // Todo : Check remaining cleanup here
        }

        /// <summary>
        /// Exits the Update / Rendering Loop (if present), closes and disposes the Renderwindow
        /// </summary>
        public void Exit()
        {
            _Device.Close();
        }
        
        // layer update root
        private void Update(Single deltaT)
        {
            // check special input
            if (Input.IsKeyDown(KeyCode.Escape)) Exit();
            
            // update layers
            _LayerBackground.Update(deltaT);
            _LayerGame.Update(deltaT);
            _LayerParticles.Update(deltaT);
            _LayerOverlay.Update(deltaT);
            _LayerDebug.Update(deltaT);
            _LayerCursor.Update(deltaT);
        }

        // layer draw root
        private void Draw()
        {
            // Clear Background
            _Device.Clear(_ClearColor);

            // Draw Layers
            _LayerBackground.Draw();
            _LayerGame.Draw();
            _LayerParticles.Draw();
            _LayerOverlay.Draw();
            _LayerDebug.Draw();
            _LayerCursor.Draw();

            // Present Backbuffer
            _Device.Display();
        }

        /// <summary>
        /// Draws an Drawable element onto the Backbuffer
        /// </summary>
        /// <param name="target">The element to draw</param>
        public void Render(Drawable element)
        {
            _Device.Draw(element);
        }

        public void Log(params object[] list) // TODO : cleanup!
        {
            for (int i = 0; i < list.Length; i++)
            {
                Console.WriteLine(list[i].ToString());
            }
            Console.WriteLine();
        }

        // Device Event Handlers ############################################################
        private void HandleLostFocus(object sender, EventArgs e)
        {
            _FocusLost = true;
        }

        private void HandleGainedFocus(object sender, EventArgs e)
        {
            _FocusLost = false;
        }

        private void _Device_Closed(object sender, EventArgs e)
        {
            Exit();
        }
    }
}