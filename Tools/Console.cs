using System;
using System.Collections.Generic;
using System.Linq;
using SFML.System;
using SFML.Window;
using SFML.Graphics;
using BlackCoat.Animation;
using BlackCoat.Entities;
using BlackCoat.Entities.Shapes;

namespace BlackCoat.Tools
{
    /// <summary>
    /// The console is a simple direct User I/O interface for advanced control
    /// </summary>
    internal class Console : Container
    {
        // Constants #######################################################################
        private const int _FONT_SIZE = 10;
        private const int _KEEP_LINES = 25;


        // Events ##########################################################################
        internal event Action<String> Command;


        // Variables #######################################################################
        private Boolean _Open = false;
        private Boolean _AnimationRunning = false;
        private Rectangle _Background;
        private TextItem _Display;
        private Queue<String> _Messages = new Queue<String>();
        private String _CurrentInput;


        // CTOR ############################################################################
        /// <summary>
        /// Creates a new instance of the <see cref="Console"/> class
        /// </summary>
        /// <param name="core">Engine Core</param>
        /// <param name="device">Render Device</param>
        internal Console(Core core, RenderWindow device) : base(core)
        {
            _Background = new Rectangle(_Core);
            _Background.Color = Color.Black;
            _Background.Alpha = 0.6f;
            AddChild(_Background);

            _Display = new TextItem(_Core);
            _Display.Position = new Vector2f(3, 3);
            _Display.Color = SFML.Graphics.Color.Cyan;
            _Display.CharacterSize = _FONT_SIZE;
            AddChild(_Display);

            Visible = false;
            View = new View(_Core.DefaultView);
            UpdateDisplayProportions(View.Size.X, View.Size.Y);

            Log.OnLog += LogMessage;
            Input.KeyPressed += HandleKeyPressed;
            Input.TextEntered += HandleTextEntered;
            device.Resized += Device_Resized;
            
            Log.Debug("Engine", nameof(Console), "ready");
        }


        // Methods #########################################################################
        private void HandleKeyPressed(Keyboard.Key key)
        {
            if (Input.Control && Input.Shift && key == Keyboard.Key.Num1)
            {
                if (_Open) Close();
                else Open();
            }
            else if (_Open && key == Keyboard.Key.Return)
            {
                if (!String.IsNullOrWhiteSpace(_CurrentInput))
                {
                    Command.Invoke(_CurrentInput);
                }
                _CurrentInput = null;
                UpdateDisplayText();
            }
        }

        private void HandleTextEntered(TextEnteredEventArgs evnt)
        {
            if (!_Open) return;
            _CurrentInput = evnt.UpdateText(_CurrentInput);
            UpdateDisplayText();
        }

        private void LogMessage(String msg)
        {
            _Messages.Enqueue(msg.Replace(Environment.NewLine, Constants.NEW_LINE));
            if (_Messages.Count > _KEEP_LINES) _Messages.Dequeue();
            if(!_Core.Disposed) UpdateDisplayText();
        }

        private void UpdateDisplayText()
        {
            var availableLines = Convert.ToInt32(Math.Floor(_Background.Size.Y / _FONT_SIZE)) - 5;
            _Display.Text = String.Join(Constants.NEW_LINE, new[] { _CurrentInput ?? String.Empty }.Concat(_Messages.Reverse().Take(availableLines)));
        }

        private void Device_Resized(object sender, SizeEventArgs e)
        {
            UpdateDisplayProportions(e.Width, e.Height);
        }

        private void UpdateDisplayProportions(float width, float height)
        {
            View.Size = new Vector2f(width, height);
            View.Center = new Vector2f(width / 2, height / 2);

            _Background.Size = new Vector2f(width, height / 3);
            if (_Open)
            {
                Position = new Vector2f(0, height - height / 3);
            }
            else
            {
                Position = new Vector2f(0, height);
            }
        }

        private void Open()
        {
            if (_AnimationRunning) return;
            Visible = true;
            _AnimationRunning = true;
            _Core.AnimationManager.Run(Position.Y, Position.Y - _Background.Size.Y, 0.4f, v => Position = new Vector2f(Position.X, v), InterpolationType.OutCubic, a =>
            {
                _Open = true;
                _AnimationRunning = false;
            });
        }
        private void Close()
        {
            if (_AnimationRunning) return;
            _AnimationRunning = true;
            _Core.AnimationManager.Run(Position.Y, Position.Y + _Background.Size.Y, 0.4f, v => Position = new Vector2f(Position.X, v), InterpolationType.OutCubic, a =>
            {
                _Open = false;
                Visible = false;
                _AnimationRunning = false;
            });
        }
    }
}