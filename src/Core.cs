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
        /// Expected return value: true when the command could be processed otherwise false.
        /// </summary>
        public event Func<String, Boolean> ConsoleCommand = c => false;

        /// <summary>
        /// Occurs when the engine enters or leaves the debug mode.
        /// </summary>
        public event Action<Boolean> DebugChanged = d => { };

        /// <summary>
        /// Occurs when the current render device has changed.
        /// </summary>
        public event Action DeviceChanged = () => { };

        /// <summary>
        /// Occurs when the size of the current render device has changed.
        /// </summary>
        public event Action<Vector2f> DeviceResized = v => { };

        /// <summary>
        /// Occurs when the current render device has lost its focus.
        /// </summary>
        public event Action FocusLost = () => { };

        /// <summary>
        /// Occurs when the current render device (re)gained its focus.
        /// </summary>
        public event Action FocusGained = () => { };


        // Variables #######################################################################
        private Stopwatch _Timer;
        private Input _Input;
        private Boolean _Debug;
        private Boolean _Fullscreen;
        private CollisionSystem _CollisionSystem;
        internal Tools.Console _Console;


        // Properties ######################################################################
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Core"/> is currently in debug mode.
        /// </summary>
        public Boolean Debug
        {
            get { return _Debug; }
            set
            {
                if (_Debug == value) return;
                _Debug = value;
                if (_Debug)
                {
                    _Input.KeyPressed += QuitOnEsc;
                    Log.Info("Debug enabled");
                }
                else
                {
                    _Input.KeyPressed -= QuitOnEsc;
                    Log.Info("Debug disabled");
                }
                DebugChanged.Invoke(_Debug);
            }
        }

        /// <summary>
        /// Random Number Generator with float and integer support.
        /// </summary>
        public RandomHelper Random { get; private set; }

        /// <summary>
        /// Scene Manager. Manages the current Scene as well as Scene transitions.
        /// </summary>
        public SceneManager SceneManager { get; private set; }

        /// <summary>
        /// Animation Manager and Factory. Used primarily to make stuff move.
        /// </summary>
        public AnimationManager AnimationManager { get; private set; }

        /// <summary>
        /// Offers methods to calculate collisions between geometric primitives. Must never be null.
        /// </summary>
        public CollisionSystem CollisionSystem
        {
            get => _CollisionSystem;
            set => _CollisionSystem = value ?? throw new Exception($"{nameof(CollisionSystem)} must never be null");
        }

        /// <summary>
        /// Color used to clear the screen of the contents from the last rendered frame.
        /// </summary>
        public Color ClearColor { get; set; }

        /// <summary>
        /// Determines if the current render device has focus.
        /// </summary>
        public Boolean HasFocus { get; private set; }

        /// <summary>
        /// Determines if the update cycle should be paused when the render device looses focus.
        /// </summary>
        public Boolean PauseUpdateOnFocusLoss { get; set; }

        /// <summary>
        /// Determines if the Engine Core has been disposed.
        /// </summary>
        public Boolean Disposed { get; private set; }

        /// <summary>
        /// Default font of the Engine. The <see cref="TextItem"/> class needs it to display text when no font is loaded.
        /// </summary>
        public Font DefaultFont { get; private set; }

        /// <summary>
        /// Default Render View - uses full with and height of the rendering device.
        /// </summary>
        internal View DefaultView { get; private set; }

        /// <summary>
        /// Current Rendering Device
        /// </summary>
        internal Device Device { get; private set; }
        internal Device OldDevice { get; private set; }

        /// <summary>
        /// Size of the current Render Device
        /// </summary>
        public Vector2f DeviceSize => Device.Size.ToVector2f();

        /// <summary>
        /// Gets or sets a value indicating whether the current <see cref="BlackCoat.Device"/> is displayed full-screen.
        /// </summary>
        public bool Fullscreen { get; set; }


        // CTOR ############################################################################
        /// <summary>
        /// Creates a new Instance of the BlackCoat Core class
        /// </summary>
        /// <param name="device">Render Device used by the Core - use of the static creation methods is recommended</param>
        /// <param name="fullscreenDevice">Optional full screen device. If none is provided a desktop device will be created automatically.</param>
        /// <exception cref="ArgumentNullException">device</exception>
        public Core(Device device, Device fullscreenDevice = null)
        {
            Log.Info("Initializing Black Coat Engine...");

            // Init Backend
            Device = device ?? throw new ArgumentNullException(nameof(device));
            OldDevice = fullscreenDevice;
            _Timer = new Stopwatch();

            // Init Defaults
            ClearColor = Color.Black;
            HasFocus = true;
            PauseUpdateOnFocusLoss = true;
            Disposed = false;
            DefaultView = Device.DefaultView;
            DefaultFont = new Font(Resources.Squares_Bold_Free);
            for (uint i = 4; i <= 42; i += 2) InitializeFontHack(DefaultFont, i); // Unfortunate necessity to prevent SFML from disposing parts of a font.

            // Attach Core-relevant Device Events
            AttachToDevice(Device);

            // Init Subsystems
            Random = new RandomHelper();
            SceneManager = new SceneManager(this);
            AnimationManager = new AnimationManager();
            CollisionSystem = new CollisionSystem();

            // Init Input
            _Input = new Input(this, false, true, false);
            _Input.KeyPressed += k => { if (_Input.AltKeyPressed && k == Keyboard.Key.Enter) Fullscreen = !Fullscreen; };
            Log.Debug("Input ready");

            // Init Console
            _Console = new Tools.Console(this, _Input);
            _Console.Command += HandleConsoleCommand;

            Log.Info("Black Coat Engine Creation Completed. - Version", GetType().Assembly.GetName().Version);
        }

        ~Core()
        {
            if (!Disposed) Dispose();
        }


        // Methods #########################################################################
        /// <summary>
        /// Initializes a font. An unfortunate necessity to prevent SFML from disposing parts of a font.
        /// </summary>
        /// <param name="font">The font to initialize.</param>
        /// <param name="charSize">Size of the character.</param>
        /// <param name="bold">If set to <c>true</c> the font will be initialized with bold characters.</param>
        public void InitializeFontHack(Font font, uint charSize = 10, bool bold = false)
        {
            var text = new Text("0123456789abcdefghijklmnopqrstuvwxyzüöäABCDEFGHIJKLMNOPQRSTUVWXYZÜÖÄ.:-_,;#'+*~´`\\?ß=()&/$%\"§!^°|@", font, charSize);
            if (bold) text.Style = Text.Styles.Bold;
            text.Draw(Device, RenderStates.Default);
            text.Dispose();
        }

        /// <summary>
        /// Displays the Renderwindow to the User.
        /// </summary>
        public void ShowRenderWindow()
        {
            if (Disposed) throw new ObjectDisposedException(nameof(Core));
            Device.SetVisible(true);
        }

        /// <summary>
        /// Hides the Renderwindow to the User without closing it.
        /// Use ShowRenderWindow() to reveal it again.
        /// </summary>
        public void HideRenderWindow()
        {
            if (Disposed) throw new ObjectDisposedException(nameof(Core));
            Device.SetVisible(false);
        }

        /// <summary>
        /// Begins the Update / Rendering Loop.
        /// This method is blocking until Exit() is called.
        /// </summary>
        public void Run()
        {
            if (Disposed) throw new ObjectDisposedException(nameof(Core));
            Log.Info("Engine Started");
            ShowRenderWindow();
            while (Device.IsOpen)
            {
                if (Fullscreen != _Fullscreen) ChangeFullscreen();
                Device.DispatchEvents();
                if (HasFocus || !PauseUpdateOnFocusLoss) // run updates
                {
                    var deltaT = (float)(_Timer.Elapsed.TotalMilliseconds / 1000d);// fractal second
                    _Timer.Restart();
                    Update(deltaT);
                }
                else // pause updating & relieve host machine
                {
                    Thread.Sleep(100);
                }
                Draw();
            }
            _Timer.Stop();
            Log.Info("Engine stopped.");
        }

        /// <summary>
        /// Exits the Update / Rendering Loop (if present) and closes and disposes the Renderwindow
        /// </summary>
        /// <param name="reason">Optional reason for logging</param>
        public void Exit(String reason = null)
        {
            if (Disposed) throw new ObjectDisposedException(nameof(Core));
            Log.Info(reason ?? "Begin Engine shutdown");
            Device.Close();
        }

        /// <summary>
        /// Calls the Update methods of the SceneGraph Hierarchy
        /// </summary>
        /// <param name="deltaT">Frame time</param>
        private void Update(Single deltaT)
        {
            // Update Scene Graph
            SceneManager.Update(deltaT);

            // Update running Animations
            AnimationManager.Update(deltaT);

            // Raise Update event for external updates
            OnUpdate(deltaT);

            // Update Console too
            _Console.Update(deltaT);
        }

        /// <summary>
        /// Draws the SceneGraph to the Render Device
        /// </summary>
        private void Draw()
        {
            // Clear Background
            Device.Clear(ClearColor);

            // Draw Scene
            DRAW_CALLS = 0;
            SceneManager.Draw();
            _Console.Draw();

            // Present Backbuffer
            Device.Display();
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
                // Get the combined transformations of all parents and update the RenderState.
                // In the SFML draw method the transform of the render state is multiplied with
                // the entities own transformation creating the final global transformation.
                state.Transform = entity.Parent.GlobalTransform;
            }

            var renderTarget = entity.RenderTarget ?? Device;
            if (entity.View == null) // view inheritance is handled by the property
            {
                entity.Draw(renderTarget, state);
            }
            else
            {
                renderTarget.SetView(entity.View);
                entity.Draw(renderTarget, state);
                renderTarget.SetView(entity.RenderTarget?.DefaultView ?? DefaultView);
            }
            DRAW_CALLS++;
        }

        /// <summary>
        /// Handles Core Commands and broadcasts other Commands to the remaining application
        /// </summary>
        /// <param name="cmd">Console input</param>
        private void HandleConsoleCommand(String cmd)
        {
            // Check for Core commands first
            switch (cmd.ToLower())
            {
                case "device":
                    Log.Debug(Device);
                    return;
                case "exit":
                case "quit":
                    Exit("Engine shutdown requested by console");
                    return;
                case "togglefullscreen":
                case "tfs":
                    Fullscreen = !Fullscreen;
                    return;
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
        private void QuitOnEsc(Keyboard.Key key)
        {
            if (key == Keyboard.Key.Escape) Exit();
        }
        #endregion

        #region Device Event handlers
        private void AttachToDevice(Device device)
        {
            Device.Resized += HandleDeviceResized;
            Device.Closed += HandleWindowClose;
            Device.LostFocus += HandleLostFocus;
            Device.GainedFocus += HandleGainedFocus;
        }
        private void DetachFromDevice(Device device)
        {
            Device.Resized -= HandleDeviceResized;
            Device.Closed -= HandleWindowClose;
            Device.LostFocus -= HandleLostFocus;
            Device.GainedFocus -= HandleGainedFocus;
        }

        private void HandleDeviceResized(object sender, SizeEventArgs e)
        {
            DefaultView.Size = DeviceSize;
            DefaultView.Center = DeviceSize / 2;
            Device.SetView(DefaultView);
            DeviceResized.Invoke(DeviceSize);
        }

        private void HandleWindowClose(object sender, EventArgs e)
        {
            // Needed for when the close button is clicked
            Exit();
        }

        private void HandleLostFocus(object sender, EventArgs e)
        {
            _Timer.Stop();
            HasFocus = false;
            FocusLost.Invoke();
        }

        private void HandleGainedFocus(object sender, EventArgs e)
        {
            HasFocus = true;
            FocusGained.Invoke();
            _Timer.Start();
        }

        private void ChangeFullscreen()
        {
            _Fullscreen = Fullscreen;
            if (OldDevice == null)
            {
                OldDevice = Device;
                Device = Fullscreen ? Device.Fullscreen : Device.Demo;
            }
            else
            {
                var temp = OldDevice;
                OldDevice = Device;
                Device = temp;
            }

            DetachFromDevice(OldDevice);
            OldDevice.SetVisible(false);
            Device.SetVisible(true);
            Device.DispatchEvents(); // flush out some redundant events before attaching
            AttachToDevice(Device);

            DefaultView = Device.DefaultView;

            DeviceChanged.Invoke();
            DeviceResized.Invoke(DeviceSize);
        }
        #endregion

        /// <summary>
        /// Releases all unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;

            SceneManager.Destroy();

            DisposeDevice(Device);
            DisposeDevice(OldDevice);

            DefaultFont.Dispose();
            DefaultFont = null;

            Log.Info("Engine Destroyed");
        }

        private void DisposeDevice(Device device)
        {
            if (device == null) return;
            if (device.CPointer != IntPtr.Zero)
            {
                if (device.IsOpen) device.Close();
                device.Dispose();
            }
            device = null;
        }
    }
}