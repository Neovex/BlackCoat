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
        public static Vector2i MousePosition { get; private set; }
        public static Single MouseWheelDelta { get; private set; }

        private static List<Mouse.Button> _MouseButtons = new List<Mouse.Button>();
        private static List<Keyboard.Key> _KeyboardKeys = new List<Keyboard.Key>();
        


        // Events ##########################################################################
        public static event Action<MouseMoveEventArgs> MouseMoved = (a) => { };
        public static event Action<MouseButtonEventArgs> MouseButtonPressed = (a) => { };
        public static event Action<MouseButtonEventArgs> MouseButtonReleased = (a) => { };
        public static event Action<MouseWheelScrollEventArgs> MouseWheelScrolled = (a) => { };
        public static event Action<KeyEventArgs> KeyPressed = (a) => { };
        public static event Action<KeyEventArgs> KeyReleased = (a) => { };



        // Methods #########################################################################
        // TODO : comment
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
            MousePosition = new Vector2i(e.X, e.Y);
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
            if (!_KeyboardKeys.Contains(e.Code)) _KeyboardKeys.Add(e.Code);
            KeyPressed(e);
        }

        internal static void HandleKeyReleased(object sender, KeyEventArgs e)
        {
            if (_KeyboardKeys.Contains(e.Code)) _KeyboardKeys.Remove(e.Code);
            KeyReleased(e);
        }

        // Other
        internal static void Reset()
        {
            _MouseButtons.Clear();
            _KeyboardKeys.Clear();
        }

        public static Boolean IsKeyDown(Keyboard.Key key)
        {
            //return _KEY1 == key || _KEY2 == key || _KEY3 == key || _KEY4 == key;
            //VS
            return _KeyboardKeys.Contains(key); // performance impact?!
        }

        /// <summary>
        /// Initializes the Input class.
        /// Starts listening to all available input events
        /// </summary>
        /// <param name="device">Renderwindow as event provider</param>
        internal static void Initialize(RenderWindow device)
        {
            _Device = device;
            _Device.MouseMoved += Input.HandleMouseMoved;
            _Device.MouseButtonPressed += Input.HandleMouseButtonPressed;
            _Device.MouseButtonReleased += Input.HandleMouseButtonReleased;
            _Device.MouseWheelScrolled += Input.HandleMouseWheelScrolled;
            _Device.KeyPressed += Input.HandleKeyPressed;
            _Device.KeyReleased += Input.HandleKeyReleased;
        }
    }
}