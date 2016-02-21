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
        // TODO : add remaining events (i.e. Device resized, mousewheel)    
        public static event EventHandler<MouseMoveEventArgs> MouseMoved
        {
            add { _Device.MouseMoved += value; }
            remove { _Device.MouseMoved -= value; }
        }

        public static event EventHandler<MouseButtonEventArgs> MouseButtonPressed
        {
            add { _Device.MouseButtonPressed += value; }
            remove { _Device.MouseButtonPressed -= value; }
        }

        public static event EventHandler<KeyEventArgs> KeyPressed
        {
            add { _Device.KeyPressed += value; }
            remove { _Device.KeyPressed -= value; }
        }


        // Methods #########################################################################
        // TODO : comment
        // Mouse
        internal static void HandleMouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            if (!_MouseButtons.Contains(e.Button)) _MouseButtons.Add(e.Button);
        }

        internal static void HandleMouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            if (_MouseButtons.Contains(e.Button)) _MouseButtons.Remove(e.Button);
        }

        internal static void HandleMouseMoved(object sender, MouseMoveEventArgs e)
        {
            MousePosition = new Vector2i(e.X, e.Y);
        }

        internal static void HandleMouseWheelMoved(object sender, MouseWheelScrollEventArgs e)
        {
            MouseWheelDelta = e.Delta;
        }

        public static Boolean IsMButtonDown(Mouse.Button button) { return _MouseButtons.Contains(button); }

        public static Boolean IsLMButtonDown() { return _MouseButtons.Contains(Mouse.Button.Left); }
        public static Boolean IsRMButtonDown() { return _MouseButtons.Contains(Mouse.Button.Right); }


        // Keyboard
        internal static void HandleKeyPressed(object sender, KeyEventArgs e)
        {
            if (!_KeyboardKeys.Contains(e.Code)) _KeyboardKeys.Add(e.Code);
        }

        internal static void HandleKeyReleased(object sender, KeyEventArgs e)
        {
            if (_KeyboardKeys.Contains(e.Code)) _KeyboardKeys.Remove(e.Code);
        }

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
            _Device.MouseWheelScrolled += Input.HandleMouseWheelMoved;
            _Device.KeyPressed += Input.HandleKeyPressed;
            _Device.KeyReleased += Input.HandleKeyReleased;
        }
    }
}