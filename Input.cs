using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackCoat
{
    /// <summary>
    /// Handles input device information
    /// </summary>
    public static class Input
    {
        internal static RenderWindow Device;

        public static Vector2i MousePosition { get; private set; }
        public static Int32 MouseWheelDelta { get; private set; }

        private static List<Mouse.Button> _MouseButtons = new List<Mouse.Button>();
        private static List<Keyboard.Key> _KeyboardKeys = new List<Keyboard.Key>();

        // TODO : add remaining events
        public static event EventHandler<MouseMoveEventArgs> MouseMoved
        {
            add { Device.MouseMoved += value; }
            remove { Device.MouseMoved -= value; }
        }

        public static event EventHandler<MouseButtonEventArgs> MouseButtonPressed
        {
            add { Device.MouseButtonPressed += value; }
            remove { Device.MouseButtonPressed -= value; }
        }

        public static event EventHandler<KeyEventArgs> KeyPressed
        {
            add { Device.KeyPressed += value; }
            remove { Device.KeyPressed -= value; }
        }


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

        internal static void HandleMouseWheelMoved(object sender, MouseWheelEventArgs e)
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

        internal static void InitializeInternal(Core core) // TODO : cleanup
        {
            Device = core.Device;
            Device.MouseMoved += Input.HandleMouseMoved;
            Device.MouseButtonPressed += Input.HandleMouseButtonPressed;
            Device.MouseButtonReleased += Input.HandleMouseButtonReleased;
            Device.MouseWheelMoved += Input.HandleMouseWheelMoved;
            Device.KeyPressed += Input.HandleKeyPressed;
            Device.KeyReleased += Input.HandleKeyReleased;
        }
    }
}