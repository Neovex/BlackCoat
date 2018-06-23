using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;
using SFML.System;

namespace BlackCoat
{
    /// <summary>
    /// Provides all available input data along with events for custom input handlers.
    /// </summary>
    public class Input
    {
        public static Input DEFAULT { get; private set; }

        // Variables #######################################################################
        private RenderWindow _Device;
        private Vector2f _MousePosition;
        private Boolean _MouseVisible;
        private List<Mouse.Button> _MouseButtons;
        private List<Keyboard.Key> _KeyboardKeys;
        
        private Boolean _MousePositionEnabled;
        private Boolean _MouseEnabled;
        private Boolean _KeyboardEnabled;


        // Properties ######################################################################
        public Boolean Shift => IsKeyDown(Keyboard.Key.LShift) || IsKeyDown(Keyboard.Key.RShift);
        public Boolean Control => IsKeyDown(Keyboard.Key.LControl) || IsKeyDown(Keyboard.Key.RControl);
        public Boolean Alt => IsKeyDown(Keyboard.Key.LAlt) || IsKeyDown(Keyboard.Key.RAlt);

        public Vector2f MousePosition => _MousePosition;
        public Single MouseWheelDelta { get; private set; }

        public Boolean MouseVisible
        {
            get => _MouseVisible;
            set => _Device.SetMouseCursorVisible(_MouseVisible = value);
        }
        
        public Boolean Enabled
        {
            get => MousePositionEnabled && MouseEnabled && KeyboardEnabled;
            set => MousePositionEnabled = MouseEnabled = KeyboardEnabled = value;
        }

        public Boolean MousePositionEnabled
        {
            get => _MousePositionEnabled;
            set
            {
                if (_MousePositionEnabled == value) return;
                if (_MousePositionEnabled = value)
                {
                    _Device.MouseMoved += HandleMouseMoved;
                }
                else
                {
                    _Device.MouseMoved -= HandleMouseMoved;
                }
            }
        }

        public Boolean MouseEnabled
        {
            get => _MouseEnabled;
            set
            {
                if (_MouseEnabled == value) return;
                if (_MouseEnabled = value)
                {
                    ResetMouse();
                    _Device.MouseButtonPressed += HandleMouseButtonPressed;
                    _Device.MouseButtonReleased += HandleMouseButtonReleased;
                    _Device.MouseWheelScrolled += HandleMouseWheelScrolled;
                }
                else
                {
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
                    ResetKeyboard();
                    _Device.KeyPressed += HandleKeyPressed;
                    _Device.KeyReleased += HandleKeyReleased;
                    _Device.TextEntered += HandleTextEntered;
                }
                else
                {
                    _Device.KeyPressed -= HandleKeyPressed;
                    _Device.KeyReleased -= HandleKeyReleased;
                    _Device.TextEntered -= HandleTextEntered;
                }
            }
        }


        // Events ##########################################################################
        public event Action<Vector2f> MouseMoved = p => { };
        public event Action<Mouse.Button> MouseButtonPressed = b => { };
        public event Action<Mouse.Button> MouseButtonReleased = b => { };
        public event Action<Single> MouseWheelScrolled = d => { };
        public event Action<Keyboard.Key> KeyPressed = k => { };
        public event Action<Keyboard.Key> KeyReleased = k => { };
        public event Action<TextEnteredEventArgs> TextEntered = t => { };


        /// <summary>
        /// Initializes a new instance of the <see cref="Input"/> class.
        /// </summary>
        /// <param name="device">Renderwindow as event provider</param>
        public Input(Core core)
        {
            if (DEFAULT == null) DEFAULT = this;

            // Init Class
            _Device = core.Device;
            _MouseVisible = true;
            _MouseButtons = new List<Mouse.Button>();
            _KeyboardKeys = new List<Keyboard.Key>();
            core.FocusLost += HandleCoreFocusLost;

            // Subscribe to input events
            Enabled = true;

            // TODO Joysticks & Game-pads:
            //_Device.JoystickButtonPressed...
        }


        // Methods #########################################################################
        /// <summary>
        /// Resets all cached input states.
        /// </summary>
        private void HandleCoreFocusLost()
        {
            ResetMouse();
            ResetKeyboard();
        }

        private void ResetMouse()
        {
            MouseWheelDelta = 0;
            _MouseButtons.ForEach(b => HandleMouseButtonReleased(this, new MouseButtonEventArgs(new MouseButtonEvent() { Button = b })));
            _MouseButtons.Clear();
        }

        private void ResetKeyboard()
        {
            _KeyboardKeys.ForEach(c => HandleKeyReleased(this, new KeyEventArgs(new KeyEvent() { Code = c })));
            _KeyboardKeys.Clear();
        }

        // Mouse
        public Boolean IsMButtonDown(Mouse.Button button) { return _MouseButtons.Contains(button); }
        public Boolean IsLMButtonDown() { return _MouseButtons.Contains(Mouse.Button.Left); }
        public Boolean IsRMButtonDown() { return _MouseButtons.Contains(Mouse.Button.Right); }

        private void HandleMouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            if (!_MouseButtons.Contains(e.Button)) _MouseButtons.Add(e.Button);
            MouseButtonPressed(e.Button);
        }

        private void HandleMouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            if (_MouseButtons.Contains(e.Button)) _MouseButtons.Remove(e.Button);
            MouseButtonReleased(e.Button);
        }

        private void HandleMouseMoved(object sender, MouseMoveEventArgs e)
        {
            _MousePosition.X = e.X;
            _MousePosition.Y = e.Y;
            MouseMoved(_MousePosition);
        }

        private void HandleMouseWheelScrolled(object sender, MouseWheelScrollEventArgs e)
        {
            MouseWheelDelta = e.Delta;
            MouseWheelScrolled(MouseWheelDelta);
        }


        // Keyboard
        public Boolean IsKeyDown(Keyboard.Key key) { return _KeyboardKeys.Contains(key); }

        private void HandleKeyPressed(object sender, KeyEventArgs e)
        {
            // Handle special keys for text input
            if (e.Code == Keyboard.Key.BackSpace) TextEntered(new TextEnteredEventArgs(true, false));
            else if (e.Code == Keyboard.Key.Delete) TextEntered(new TextEnteredEventArgs(false, true));

            if (IsKeyDown(e.Code)) return;
            _KeyboardKeys.Add(e.Code);
            KeyPressed(e.Code);
        }

        private void HandleKeyReleased(object sender, KeyEventArgs e)
        {
            if (!IsKeyDown(e.Code)) return;
            _KeyboardKeys.Remove(e.Code);
            KeyReleased(e.Code);
        }

        private void HandleTextEntered(object sender, TextEventArgs e)
        {
            if (e.Unicode.All(c => !Char.IsControl(c))) TextEntered(new TextEnteredEventArgs(e.Unicode));
        }
    }
}