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
        private Device _Device;
        private Device _OldDevice;
        private Stopwatch _Timer;
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
                    Input.KeyPressed += QuitOnEsc;
                    Log.Info("Debug enabled");
                }
                else
                {
                    Input.KeyPressed -= QuitOnEsc;
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
            get => _CollisionSystem;
            set => _CollisionSystem = value ?? throw new Exception($"{nameof(CollisionSystem)} must never be null");
        }

        /// <summary>
        /// Provides all available input data along with events for custom input handlers.
        /// </summary>
        public Input Input { get; }

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
        public Boolean PauseUpdateOnFocusLoss { get; private set; }

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
        internal Device Device => _Device;
        internal Device OldDevice => _OldDevice;

        /// <summary>
        /// Size of the current Render Device
        /// </summary>
        public Vector2f DeviceSize => _Device.Size.ToVector2f();

        /// <summary>
        /// Gets or sets a value indicating whether the current <see cref="BlackCoat.Device"/> is displayed full-screen.
        /// </summary>
        public bool Fullscreen { get; set; }



        // CTOR ############################################################################
        /// <summary>
        /// Creates a new Instance of the BlackCoat Core class
        /// </summary>
        /// <param name="device">Render Device used by the Core - use of the static creation methods is recommended</param>
        /// <param name="fullscreenDevice">Optional alternate device. Should be the corresponding opposite of the first device.</param>
        /// <exception cref="ArgumentNullException">device</exception>
        public Core(Device device, Device fullscreenDevice = null)
        {
            Log.Info("Initializing Black Coat Engine...");

            // Init Backend
            _Device = device ?? throw new ArgumentNullException(nameof(device));
            _OldDevice = fullscreenDevice;
            _Timer = new Stopwatch();

            // Init Defaults
            ClearColor = Color.Black;
            HasFocus = true;
            PauseUpdateOnFocusLoss = true;
            Disposed = false;
            DefaultView = _Device.DefaultView;
            DefaultFont = new Font(Resources.Squares_Bold_Free);
            for (uint i = 4; i <= 42; i += 2) InitializeFontHack(DefaultFont, i); // Unfortunate necessity to prevent SFML from disposing parts of a font.

            // Attach Core-relevant Device Events
            AttachToDevice(_Device);

            // Init Subsystems
            Random = new RandomHelper();
            StateManager = new StateManager(this);
            AnimationManager = new AnimationManager();
            CollisionSystem = new CollisionSystem();

            // Init Input
            Input = new Input(this);
            Input.KeyPressed += k => { if (Input.Alt && k == Keyboard.Key.Return) Fullscreen = !Fullscreen; };
            Log.Debug("Default Input ready");

            // Init Console
            _Console = new Tools.Console(this);
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
            text.Draw(_Device, RenderStates.Default);
            text.Dispose();
        }

        /// <summary>
        /// Displays the Renderwindow to the User.
        /// </summary>
        public void ShowRenderWindow()
        {
            if (Disposed) throw new ObjectDisposedException(nameof(Core));
            _Device.SetVisible(true);
        }

        /// <summary>
        /// Hides the Renderwindow to the User without closing it.
        /// Use ShowRenderWindow() to reveal it again.
        /// </summary>
        public void HideRenderWindow()
        {
            if (Disposed) throw new ObjectDisposedException(nameof(Core));
            _Device.SetVisible(false);
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
            while (_Device.IsOpen)
            {
                if (Fullscreen != _Fullscreen) ChangeFullscreen();
                _Device.DispatchEvents();
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
        /// Performs a single update followed by a single render call. Resulting in a single Device refresh.
        /// </summary>
        /// <param name="deltaT">Optional time delta between frames in fractal seconds</param>
        public void ManualRefresh(float deltaT = 0)
        {
            if (Disposed) throw new ObjectDisposedException(nameof(Core));
            if (!_Device.IsOpen) throw new InvalidStateException("Device is not open");
            if (Fullscreen != _Fullscreen) ChangeFullscreen();
            _Device.DispatchEvents();
            Update(deltaT);
            Draw();
        }

        /// <summary>
        /// Exits the Update / Rendering Loop (if present) and closes and disposes the Renderwindow
        /// </summary>
        /// <param name="reason">Optional reason for logging</param>
        public void Exit(String reason = null)
        {
            if (Disposed) throw new ObjectDisposedException(nameof(Core));
            Log.Info(reason ?? "Begin Engine shutdown");
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

            // Update Console too
            _Console.Update(deltaT);
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
                // Get the combined transformations of all parents and update the renderstate.
                // In the SFML draw method the transform of the render state is multiplied
                // by the entities own transformation creating the final global transformation.
                state.Transform = entity.Parent.GlobalTransform;
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
            // Check for Core commands first
            switch (cmd.ToLower())
            {
                case "device":
                    Log.Debug(_Device);
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
            _Device.Resized += HandleDeviceResized;
            _Device.Closed += HandleWindowClose;
            _Device.LostFocus += HandleLostFocus;
            _Device.GainedFocus += HandleGainedFocus;
        }
        private void DetachFromDevice(Device device)
        {
            _Device.Resized -= HandleDeviceResized;
            _Device.Closed -= HandleWindowClose;
            _Device.LostFocus -= HandleLostFocus;
            _Device.GainedFocus -= HandleGainedFocus;
        }

        private void HandleDeviceResized(object sender, SizeEventArgs e)
        {
            DefaultView.Size = DeviceSize;
            DefaultView.Center = DeviceSize / 2;
            _Device.SetView(DefaultView);
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
            if (_OldDevice == null)
            {
                _OldDevice = _Device;
                _Device = Fullscreen ? Device.Fullscreen : Device.Demo;
            }
            else
            {
                var temp = _OldDevice;
                _OldDevice = _Device;
                _Device = temp;
            }

            DetachFromDevice(_OldDevice);
            _OldDevice.SetVisible(false);
            _Device.SetVisible(true);
            _Device.DispatchEvents(); // flush out some redundant events before attaching
            AttachToDevice(_Device);

            DefaultView = _Device.DefaultView;

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

            StateManager.Destroy();

            DisposeDevice(_Device);
            DisposeDevice(_OldDevice);

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