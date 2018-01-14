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
    /// Collects available input data and provides useful events for custom input handlers
    /// </summary>
    public static class Input
    {
        // Variables #######################################################################
        private static RenderWindow _Device;
        private static Vector2f _MousePosition;
        private static Boolean _MouseVisible;
        private static List<Mouse.Button> _MouseButtons = new List<Mouse.Button>();
        private static List<Keyboard.Key> _KeyboardKeys = new List<Keyboard.Key>();


        // Properties ######################################################################
        public static Boolean Shift { get { return IsKeyDown(Keyboard.Key.LShift) || IsKeyDown(Keyboard.Key.RShift); } }
        public static Boolean Control { get { return IsKeyDown(Keyboard.Key.LControl) || IsKeyDown(Keyboard.Key.RControl); } }
        public static Boolean Alt { get { return IsKeyDown(Keyboard.Key.LAlt) || IsKeyDown(Keyboard.Key.RAlt); } }

        public static Vector2f MousePosition { get { return _MousePosition; } }
        public static Single MouseWheelDelta { get; private set; }

        public static Boolean MouseVisible
        {
            get { return _MouseVisible; }
            set { _Device.SetMouseCursorVisible(_MouseVisible = value); }
        }


        // Events ##########################################################################
        public static event Action<Vector2u> DeviceResized = v => { };
        public static event Action<Vector2f> MouseMoved = p => { };
        public static event Action<Mouse.Button> MouseButtonPressed = b => { };
        public static event Action<Mouse.Button> MouseButtonReleased = b => { };
        public static event Action<Single> MouseWheelScrolled = d => { };
        public static event Action<Keyboard.Key> KeyPressed = k => { };
        public static event Action<Keyboard.Key> KeyReleased = k => { };
        public static event Action<TextEnteredEventArgs> TextEntered = t => { };


        // Methods #########################################################################
        
        // Internal Management

        /// <summary>
        /// Initializes the Input class.
        /// Starts listening to all available input events.
        /// </summary>
        /// <param name="device">Renderwindow as event provider</param>
        internal static void Initialize(RenderWindow device)
        {
            _Device = device;
            _Device.Resized += HandleDeviceResized;
            _Device.MouseMoved += HandleMouseMoved;
            _Device.MouseButtonPressed += HandleMouseButtonPressed;
            _Device.MouseButtonReleased += HandleMouseButtonReleased;
            _Device.MouseWheelScrolled += HandleMouseWheelScrolled;
            _Device.KeyPressed += HandleKeyPressed;
            _Device.KeyReleased += HandleKeyReleased;
            _Device.TextEntered += HandleTextEntered;
            
            Log.Debug(nameof(Input), "management initialized");
        }

        /// <summary>
        /// Resets all cached input states.
        /// </summary>
        internal static void Reset()
        {
            _MouseButtons.Clear();
            _KeyboardKeys.Clear();// fixme up events
        }


        // Render Window
        private static void HandleDeviceResized(object sender, SizeEventArgs e)
        {
            DeviceResized(new Vector2u(e.Width, e.Height));
        }
        // TODO : FOCUS LOST

        // Mouse
        public static Boolean IsMButtonDown(Mouse.Button button) { return _MouseButtons.Contains(button); }
        public static Boolean IsLMButtonDown() { return _MouseButtons.Contains(Mouse.Button.Left); }
        public static Boolean IsRMButtonDown() { return _MouseButtons.Contains(Mouse.Button.Right); }

        private static void HandleMouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            if (!_MouseButtons.Contains(e.Button)) _MouseButtons.Add(e.Button);
            MouseButtonPressed(e.Button);
        }

        private static void HandleMouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            if (_MouseButtons.Contains(e.Button)) _MouseButtons.Remove(e.Button);
            MouseButtonReleased(e.Button);
        }

        private static void HandleMouseMoved(object sender, MouseMoveEventArgs e)
        {
            _MousePosition.X = e.X;
            _MousePosition.Y = e.Y;
            MouseMoved(_MousePosition);
        }

        private static void HandleMouseWheelScrolled(object sender, MouseWheelScrollEventArgs e)
        {
            MouseWheelDelta = e.Delta;
            MouseWheelScrolled(MouseWheelDelta);
        }


        // Keyboard
        public static Boolean IsKeyDown(Keyboard.Key key) { return _KeyboardKeys.Contains(key); }

        private static void HandleKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.BackSpace) TextEntered(new TextEnteredEventArgs(true));
            if (IsKeyDown(e.Code)) return;
            _KeyboardKeys.Add(e.Code);
            KeyPressed(e.Code);
        }

        private static void HandleKeyReleased(object sender, KeyEventArgs e)
        {
            if (!IsKeyDown(e.Code)) return;
            _KeyboardKeys.Remove(e.Code);
            KeyReleased(e.Code);
        }

        private static void HandleTextEntered(object sender, TextEventArgs e)
        {
            if (e.Unicode.All(c => !Char.IsControl(c))) TextEntered(new TextEnteredEventArgs(e.Unicode));
        }

        // TODO : Joystick/Gamepad
    }
}