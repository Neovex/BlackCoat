using System;
using System.Linq;
using System.Collections.Generic;

using SFML.System;
using SFML.Window;
using SFML.Graphics;

namespace BlackCoat
{
    /// <summary>
    /// Provides all available input data along with events for custom input handlers.
    /// </summary>
    public class Input : BlackCoatBase, IDisposable
    {
        internal static Input MASTER_OVERRIDE;

        // Events ##########################################################################
        public event Action<Vector2f> MouseMoved = p => { };
        public event Action<Mouse.Button> MouseButtonPressed = b => { };
        public event Action<Mouse.Button> MouseButtonReleased = b => { };
        public event Action<float> MouseWheelScrolled = d => { };
        public event Action<Keyboard.Key> KeyPressed = k => { };
        public event Action<Keyboard.Key> KeyReleased = k => { };
        public event Action<TextEnteredEventArgs> TextEntered = t => { };

        public event Action<uint> JoystickConnected = i => { };
        public event Action<uint> JoystickDisconnected = i => { };
        public event Action<uint, Joystick.Axis, float> JoystickMoved = (i, a, v) => { };
        public event Action<uint, uint> JoystickButtonPressed = (i, b) => { };
        public event Action<uint, uint> JoystickButtonReleased = (i, b) => { };


        // Variables #######################################################################
        private RenderWindow _Device;
        private Vector2f _MousePosition;
        private static Boolean _MouseVisible = true;
        private HashSet<Mouse.Button> _MouseButtons;
        private HashSet<Keyboard.Key> _KeyboardKeys;
        private HashSet<uint> _ConnectedJoysticks;
        private HashSet<(uint, uint)> _JoystickButtons;
        private Dictionary<(uint, Joystick.Axis), float> _JoystickPositions;

        private Boolean _MouseEnabled;
        private Boolean _KeyboardEnabled;
        private Boolean _JoysticksEnabled;


        // Properties ######################################################################
        public Boolean Disposed { get; private set; }
        public InputSource CurrentEventSource { get; private set; }
        public Boolean ShiftKeyPressed => IsKeyDown(Keyboard.Key.LShift) || IsKeyDown(Keyboard.Key.RShift);
        public Boolean ControlKeyPressed => IsKeyDown(Keyboard.Key.LControl) || IsKeyDown(Keyboard.Key.RControl);
        public Boolean AltKeyPressed => IsKeyDown(Keyboard.Key.LAlt) || IsKeyDown(Keyboard.Key.RAlt);

        public Vector2f MousePosition => _MousePosition;
        public Boolean LeftMouseButtonPressed => _MouseButtons.Contains(Mouse.Button.Left);
        public Boolean RightMouseButtonPressed => _MouseButtons.Contains(Mouse.Button.Right);
        public Single MouseWheelDelta { get; private set; }

        public Boolean MouseVisible
        {
            get => _MouseVisible;
            set => _Device.SetMouseCursorVisible(_MouseVisible = value);
        }

        public Boolean MouseEnabled
        {
            get => _MouseEnabled;
            set
            {
                if (_MouseEnabled == value) return;
                if (_MouseEnabled = value)
                {
                    _Device.MouseMoved += HandleMouseMoved;
                    _Device.MouseButtonPressed += HandleMouseButtonPressed;
                    _Device.MouseButtonReleased += HandleMouseButtonReleased;
                    _Device.MouseWheelScrolled += HandleMouseWheelScrolled;
                }
                else
                {
                    ResetMouse();
                    _Device.MouseMoved -= HandleMouseMoved;
                    _MousePosition = new Vector2f(-1, -1);
                    _Device.MouseButtonPressed -= HandleMouseButtonPressed;
                    _Device.MouseButtonReleased -= HandleMouseButtonReleased;
                    _Device.MouseWheelScrolled -= HandleMouseWheelScrolled;
                }
            }
        }

        public Boolean KeyboardEnabled
        {
            get => _KeyboardEnabled;
            set
            {
                if (_KeyboardEnabled == value) return;
                if (_KeyboardEnabled = value)
                {
                    _Device.KeyPressed += HandleKeyPressed;
                    _Device.KeyReleased += HandleKeyReleased;
                    _Device.TextEntered += HandleTextEntered;
                }
                else
                {
                    ResetKeyboard();
                    _Device.KeyPressed -= HandleKeyPressed;
                    _Device.KeyReleased -= HandleKeyReleased;
                    _Device.TextEntered -= HandleTextEntered;
                }
            }
        }

        public bool JoysticksEnabled
        {
            get => _JoysticksEnabled;
            set
            {
                if (_JoysticksEnabled == value) return;
                if (_JoysticksEnabled = value)
                {
                    _Device.JoystickConnected += HandleJoystickConnected;
                    _Device.JoystickDisconnected += HandleJoystickDisconnected;
                    _Device.JoystickMoved += HandleJoystickMoved;
                    _Device.JoystickButtonPressed += HandleJoystickButtonPressed;
                    _Device.JoystickButtonReleased += HandleJoystickButtonReleased;
                }
                else
                {
                    ResetJoysticks();
                    _Device.JoystickConnected -= HandleJoystickConnected;
                    _Device.JoystickDisconnected -= HandleJoystickDisconnected;
                    _Device.JoystickMoved -= HandleJoystickMoved;
                    _Device.JoystickButtonPressed -= HandleJoystickButtonPressed;
                    _Device.JoystickButtonReleased -= HandleJoystickButtonReleased;
                }
            }
        }


        // Constructor #####################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="Input" /> class.
        /// </summary>
        /// <param name="core">The <see cref="Core"/></param>
        /// <param name="mouse">If set to <c>true</c> mouse events will be available.</param>
        /// <param name="keyboard">If set to <c>true</c> keyboard events will be available.</param>
        /// <param name="joystick">If set to <c>true</c> game joystick will be available.</param>
        public Input(Core core, bool mouse = true, bool keyboard = true, bool joystick = false) : base(core)
        {
            // Init Class
            _Device = _Core.Device;
            _MousePosition = new Vector2f(-1, -1);
            _MouseButtons = new HashSet<Mouse.Button>();
            _KeyboardKeys = new HashSet<Keyboard.Key>();
            _ConnectedJoysticks = new HashSet<uint>();
            _JoystickButtons = new HashSet<(uint, uint)>();
            _JoystickPositions = new Dictionary<(uint, Joystick.Axis), float>();
            _Core.FocusLost += HandleCoreFocusLost;
            _Core.DeviceChanged += HandleCoreDeviceChanged;

            // Subscribe to input events
            MouseEnabled = mouse;
            KeyboardEnabled = keyboard;
            JoysticksEnabled = joystick;
        }

        ~Input()
        {
            Dispose(false);
        }


        // Methods #########################################################################        
        /// <summary>
        /// Removes all listeners from the old device and attaches to the new one.
        /// </summary>
        private void HandleCoreDeviceChanged()
        {
            // Save State
            var mv = MouseVisible;
            var m = MouseEnabled;
            var k = KeyboardEnabled;
            // Detach from old device
            DetachFromDevice();
            // Update Device
            _Device = _Core.Device;
            // Attach to new device
            MouseVisible = mv;
            MouseEnabled = m;
            KeyboardEnabled = k;
        }

        private void HandleCoreFocusLost()
        {
            ResetMouse();
            ResetKeyboard();
        }

        #region Mouse
        public Boolean IsMButtonDown(Mouse.Button button) => _MouseButtons.Contains(button);

        private void HandleMouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            if (_MouseButtons.Contains(e.Button)) return;
            _MouseButtons.Add(e.Button);
            CurrentEventSource = InputSource.Mouse;
            MouseButtonPressed(e.Button);
            CurrentEventSource = InputSource.None;
        }

        private void HandleMouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            if (!_MouseButtons.Contains(e.Button)) return;
            _MouseButtons.Remove(e.Button);
            CurrentEventSource = InputSource.Mouse;
            MouseButtonReleased(e.Button);
            CurrentEventSource = InputSource.None;
        }

        private void HandleMouseMoved(object sender, MouseMoveEventArgs e)
        {
            _MousePosition.X = e.X;
            _MousePosition.Y = e.Y;
            CurrentEventSource = InputSource.Mouse;
            MouseMoved(_MousePosition);
            CurrentEventSource = InputSource.None;
        }

        private void HandleMouseWheelScrolled(object sender, MouseWheelScrollEventArgs e)
        {
            MouseWheelDelta = e.Delta;
            CurrentEventSource = InputSource.Mouse;
            MouseWheelScrolled(e.Delta);
            CurrentEventSource = InputSource.None;
        }

        private void ResetMouse()
        {
            MouseWheelDelta = 0;
            var events = _MouseButtons.Select(b => new MouseButtonEventArgs(new MouseButtonEvent() { Button = b })).ToArray();
            foreach (var e in events) HandleMouseButtonReleased(this, e);
        }
        #endregion

        #region Keyboard
        public Boolean IsKeyDown(Keyboard.Key key) { return _KeyboardKeys.Contains(key); }

        private void HandleKeyPressed(object sender, KeyEventArgs e)
        {
            // Handle special keys for text input
            switch (e.Code)
            {
                case Keyboard.Key.Backspace:
                    TextEntered(new TextEnteredEventArgs(TextEnteredEventArgs.Operation.Backspace));
                    break;
                case Keyboard.Key.End:
                    TextEntered(new TextEnteredEventArgs(TextEnteredEventArgs.Operation.End));
                    break;
                case Keyboard.Key.Home:
                    TextEntered(new TextEnteredEventArgs(TextEnteredEventArgs.Operation.Home));
                    break;
                case Keyboard.Key.Delete:
                    TextEntered(new TextEnteredEventArgs(TextEnteredEventArgs.Operation.Del));
                    break;
                case Keyboard.Key.Left:
                    TextEntered(new TextEnteredEventArgs(TextEnteredEventArgs.Operation.Left));
                    break;
                case Keyboard.Key.Right:
                    TextEntered(new TextEnteredEventArgs(TextEnteredEventArgs.Operation.Right));
                    break;
            }

            // Manage Key Presses
            if (IsKeyDown(e.Code)) return;
            _KeyboardKeys.Add(e.Code);
            CurrentEventSource = InputSource.Keyboard;
            if (MASTER_OVERRIDE == null || MASTER_OVERRIDE == this) KeyPressed(e.Code);
            CurrentEventSource = InputSource.None;
        }

        private void HandleKeyReleased(object sender, KeyEventArgs e)
        {
            if (!IsKeyDown(e.Code)) return;
            _KeyboardKeys.Remove(e.Code);
            CurrentEventSource = InputSource.Keyboard;
            if (MASTER_OVERRIDE == null || MASTER_OVERRIDE == this) KeyReleased(e.Code);
            CurrentEventSource = InputSource.None;
        }

        private void HandleTextEntered(object sender, TextEventArgs e)
        {
            if (e.Unicode.All(c => !Char.IsControl(c))) TextEntered(new TextEnteredEventArgs(e.Unicode));
        }

        private void ResetKeyboard()
        {
            var events = _KeyboardKeys.Select(k => new KeyEventArgs(new KeyEvent() { Code = k })).ToArray();
            foreach (var e in events) HandleKeyReleased(this, e);
        }
        #endregion

        #region Joystick
        private void HandleJoystickConnected(object sender, JoystickConnectEventArgs e)
        {
            HandleJoystickConnected(e.JoystickId);
        }
        private void HandleJoystickConnected(uint id)
        {
            if (_ConnectedJoysticks.Contains(id)) return;
            _ConnectedJoysticks.Add(id);
            JoystickDisconnected.Invoke(id);
        }

        private void HandleJoystickDisconnected(object sender, JoystickConnectEventArgs e)
        {
            if (!_ConnectedJoysticks.Contains(e.JoystickId)) return;
            ClearJoystickData(e.JoystickId);
            _ConnectedJoysticks.Remove(e.JoystickId);
            JoystickDisconnected.Invoke(e.JoystickId);
        }

        public Boolean IsJoystickButtonDown(uint joystick, uint button) => _JoystickButtons.Contains((joystick, button));
        private void HandleJoystickButtonPressed(object sender, JoystickButtonEventArgs e)
        {
            HandleJoystickConnected(e.JoystickId);
            if (IsJoystickButtonDown(e.JoystickId, e.Button)) return;
            _JoystickButtons.Add((e.JoystickId, e.Button));
            CurrentEventSource = InputSource.Joystick;
            JoystickButtonPressed.Invoke(e.JoystickId, e.Button);
            CurrentEventSource = InputSource.None;
        }

        private void HandleJoystickButtonReleased(object sender, JoystickButtonEventArgs e)
        {
            if (!IsJoystickButtonDown(e.JoystickId, e.Button)) return;
            _JoystickButtons.Remove((e.JoystickId, e.Button));
            CurrentEventSource = InputSource.Joystick;
            JoystickButtonReleased.Invoke(e.JoystickId, e.Button);
            CurrentEventSource = InputSource.None;
        }

        public float GetJoystickPositionFor(uint joystickId, Joystick.Axis axis) => _JoystickPositions.TryGetValue((joystickId, axis), out float v) ? v : 0;

        private void HandleJoystickMoved(object sender, JoystickMoveEventArgs e)
        {
            HandleJoystickConnected(e.JoystickId);
            _JoystickPositions[(e.JoystickId, e.Axis)] = e.Position;
        }

        private void ClearJoystickData(uint id)
        {
            var buttonEvents = _JoystickButtons.Where(e => e.Item1 == id).
                                                Select(e => new JoystickButtonEventArgs(
                                                            new JoystickButtonEvent() { JoystickId = id, Button = e.Item2 })).
                                                ToArray();
            foreach (var e in buttonEvents) HandleJoystickButtonReleased(this, e);

            var moveEvents = _JoystickPositions.Where(e => e.Key.Item1 == id).
                                                Select(e => new JoystickMoveEventArgs(
                                                            new JoystickMoveEvent() { JoystickId = e.Key.Item1, Axis = e.Key.Item2, Position = 0 })).
                                                ToArray();
            foreach (var e in moveEvents) HandleJoystickMoved(this, e);
        }

        private void ResetJoysticks()
        {
            foreach (var id in _ConnectedJoysticks) ClearJoystickData(id);
        }
        #endregion

        #region IDisposable Support
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool saveToDisposeManagedObjects)
        {
            if (Disposed) return;
            Disposed = true;

            if (saveToDisposeManagedObjects)
            {
                if (_Core != null)
                {
                    _Core.FocusLost -= HandleCoreFocusLost;
                    _Core.DeviceChanged -= HandleCoreDeviceChanged;
                }

                if (_Device != null && _Device.CPointer != IntPtr.Zero)
                {
                    DetachFromDevice();
                }
            }
            _Device = null;
            _MouseButtons = null;
            _KeyboardKeys = null;
            _ConnectedJoysticks = null;
        }

        private void DetachFromDevice()
        {
            MouseEnabled = false;
            KeyboardEnabled = false;
            JoysticksEnabled = false;
        }
        #endregion
    }
}