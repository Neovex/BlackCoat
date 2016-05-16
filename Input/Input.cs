﻿using System;
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
        private static List<Mouse.Button> _MouseButtons = new List<Mouse.Button>();
        private static List<Keyboard.Key> _KeyboardKeys = new List<Keyboard.Key>();


        // Properties ######################################################################
        public static Boolean Shift { get { return IsKeyDown(Keyboard.Key.LShift) || IsKeyDown(Keyboard.Key.RShift); } }
        public static Boolean Control { get { return IsKeyDown(Keyboard.Key.LControl) || IsKeyDown(Keyboard.Key.RControl); } }
        public static Boolean Alt { get { return IsKeyDown(Keyboard.Key.LAlt) || IsKeyDown(Keyboard.Key.RAlt); } }

        public static Vector2f MousePosition { get; private set; }
        public static Single MouseWheelDelta { get; private set; }



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
            MouseButtonPressed(e.Button);
        }

        internal static void HandleMouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            if (_MouseButtons.Contains(e.Button)) _MouseButtons.Remove(e.Button);
            MouseButtonReleased(e.Button);
        }

        internal static void HandleMouseMoved(object sender, MouseMoveEventArgs e)
        {
            MousePosition = new Vector2f(e.X, e.Y);
            MouseMoved(MousePosition);
        }

        internal static void HandleMouseWheelScrolled(object sender, MouseWheelScrollEventArgs e)
        {
            MouseWheelDelta = e.Delta;
            MouseWheelScrolled(MouseWheelDelta);
        }

        public static Boolean IsMButtonDown(Mouse.Button button) { return _MouseButtons.Contains(button); }
        public static Boolean IsLMButtonDown() { return _MouseButtons.Contains(Mouse.Button.Left); }
        public static Boolean IsRMButtonDown() { return _MouseButtons.Contains(Mouse.Button.Right); }


        // Keyboard
        internal static void HandleKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.BackSpace) TextEntered(new TextEnteredEventArgs(true));
            if (IsKeyDown(e.Code)) return;
            _KeyboardKeys.Add(e.Code);
            KeyPressed(e.Code);
        }

        internal static void HandleKeyReleased(object sender, KeyEventArgs e)
        {
            if (!IsKeyDown(e.Code)) return;
            _KeyboardKeys.Remove(e.Code);
            KeyReleased(e.Code);
        }

        internal static void HandleTextEntered(object sender, TextEventArgs e)
        {
            if (e.Unicode.All(c => !Char.IsControl(c))) TextEntered(new TextEnteredEventArgs(e.Unicode));
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