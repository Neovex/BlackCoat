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
    /// Collects available input data and provides usefull events for custom input handlers
    /// </summary>
    public static class Input
    {
        // Variables #######################################################################
        private static RenderWindow _Device;


        // Properties ######################################################################
        public static Vector2f MousePosition { get; private set; }
        public static Single MouseWheelDelta { get; private set; }

        private static List<Mouse.Button> _MouseButtons = new List<Mouse.Button>();
        private static List<Keyboard.Key> _KeyboardKeys = new List<Keyboard.Key>();



        // Events ##########################################################################
        public static event Action<Vector2u> DeviceResized = (v) => { };
        public static event Action<MouseMoveEventArgs> MouseMoved = (a) => { };
        public static event Action<MouseButtonEventArgs> MouseButtonPressed = (a) => { };
        public static event Action<MouseButtonEventArgs> MouseButtonReleased = (a) => { };
        public static event Action<MouseWheelScrollEventArgs> MouseWheelScrolled = (a) => { };
        public static event Action<KeyEventArgs> KeyPressed = (a) => { };
        public static event Action<KeyEventArgs> KeyReleased = (a) => { };
        public static event Action<TextEventArgs> TextEntered = (a) => { };



        // Methods #########################################################################
        // TODO : comment

        // Render Window
        static void HandleDeviceResized(object sender, SizeEventArgs e)
        {
            DeviceResized(new Vector2u(e.Width, e.Height));
        }

        // Mouse
        internal static void HandleMouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            if (!_MouseButtons.Contains(e.Button)) _MouseButtons.Add(e.Button);
            MouseButtonPressed(e);
        }

        internal static void HandleMouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            if (_MouseButtons.Contains(e.Button)) _MouseButtons.Remove(e.Button);
            MouseButtonReleased(e);
        }

        internal static void HandleMouseMoved(object sender, MouseMoveEventArgs e)
        {
            MousePosition = new Vector2f(e.X, e.Y);
            MouseMoved(e);
        }

        internal static void HandleMouseWheelScrolled(object sender, MouseWheelScrollEventArgs e)
        {
            MouseWheelDelta = e.Delta;
            MouseWheelScrolled(e);
        }

        public static Boolean IsMButtonDown(Mouse.Button button) { return _MouseButtons.Contains(button); }
        public static Boolean IsLMButtonDown() { return _MouseButtons.Contains(Mouse.Button.Left); }
        public static Boolean IsRMButtonDown() { return _MouseButtons.Contains(Mouse.Button.Right); }


        // Keyboard
        internal static void HandleKeyPressed(object sender, KeyEventArgs e)
        {
            //if(e.Code == Keyboard.Key.BackSpace) TextEntered(new TODO: replace all event objects with corresponding structures also add backspace to text input
            if (IsKeyDown(e.Code)) return;
            _KeyboardKeys.Add(e.Code);
            KeyPressed(e);
        }

        internal static void HandleKeyReleased(object sender, KeyEventArgs e)
        {
            if (!IsKeyDown(e.Code)) return;
            _KeyboardKeys.Remove(e.Code);
            KeyReleased(e);
        }

        internal static void HandleTextEntered(object sender, TextEventArgs e)
        {
            TextEntered(e);
        }

        public static Boolean IsKeyDown(Keyboard.Key key) { return _KeyboardKeys.Contains(key); } // performance impact?!

        // Other
        internal static void Reset()
        {
            _MouseButtons.Clear();
            _KeyboardKeys.Clear();
        }

        /// <summary>
        /// Initializes the Input class.
        /// Starts listening to all available input events
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
        }
    }
}