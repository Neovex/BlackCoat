using System;
using System.Collections.Generic;
using System.Linq;
using SFML.System;
using SFML.Window;
using SFML.Graphics;
using BlackCoat.Animation;
using BlackCoat.UI;

namespace BlackCoat.Tools
{
    /// <summary>
    /// The console is a simple direct User I/O interface for advanced control
    /// </summary>
    class Console : Canvas
    {
        // Constants #######################################################################
        private const int _FONT_SIZE = 10;
        private const int _MAX_MESSAGE_HISTORY = 30;


        // Events ##########################################################################
        internal event Action<String> Command;


        // Variables #######################################################################
        private Single _Height;
        private Boolean _AnimationRunning;
        private TextBox _InputBox;
        private Label _Output;
        private Queue<String> _Messages;


        // Properties ######################################################################
        internal bool IsOpen { get; private set; }


        // CTOR ############################################################################
        /// <summary>
        /// Creates a new instance of the <see cref="Console"/> class
        /// </summary>
        /// <param name="core">Engine Core</param>
        internal Console(Core core, Input input) : base(core)
        {
            // Init Self
            _Messages = new Queue<String>();
            Input = new UIInput(input, true);
            Visible = false;
            BackgroundColor = Color.Black;
            BackgroundAlpha = 0.6f;

            // Setup Controls
            Add(new OffsetContainer(_Core, Orientation.Vertical)
            {
                Init = new UIComponent[]
                {
                    _InputBox = new TextBox(_Core)
                    {
                        BackgroundAlpha = 0.3f,
                        TextColor = Color.Cyan,
                        EditingTextColor = Color.Cyan,
                        Padding = new FloatRect(3,3,3,3),
                        MinSize = new Vector2f(_Core.DeviceSize.X, 7),
                        CharacterSize = _FONT_SIZE
                    },
                    _Output = new Label(_Core)
                    {
                        Padding  = new FloatRect(3,0,3,0),
                        TextColor     = _InputBox.TextColor,
                        CharacterSize = _FONT_SIZE,
                    }
                }
            });

            // Events
            Log.OnLog += LogMessage;
            _Core.DeviceResized += UpdateDisplayProportions;
            Input.Input.KeyPressed += HandleKeyPressed;

            // Init
            UpdateDisplayProportions(_Core.DeviceSize);
            Log.Debug(nameof(Console), "ready");
        }


        // Methods #########################################################################
        private void UpdateDisplayProportions(Vector2f size)
        {
            _Height = size.Y / 3;
            Resize(new Vector2f(size.X, _Height));
            Position = new Vector2f(Position.X , size.Y - (IsOpen ? _Height : 0));
            _InputBox.MinSize = new Vector2f(size.X, _InputBox.MinSize.Y);
            UpdateOutputText();
        }

        private void HandleKeyPressed(Keyboard.Key key)
        {
            if (Input.Input.Control && Input.Input.Shift && key == Keyboard.Key.Num1)
            {
                if (IsOpen) Close();
                else Open();
            }
            else if (IsOpen && key == Keyboard.Key.Enter)
            {
                if (!String.IsNullOrWhiteSpace(_InputBox.Text))
                {
                    Command.Invoke(_InputBox.Text);
                }
                _InputBox.Text = String.Empty;
                UpdateOutputText();
            }
            else if (IsOpen && key == Keyboard.Key.Escape)
            {
                Close();
            }
        }

        private void LogMessage(String msg)
        {
            _Messages.Enqueue(msg.Replace(Environment.NewLine, Constants.NEW_LINE));
            if (_Messages.Count > _MAX_MESSAGE_HISTORY) _Messages.Dequeue();
            if(!_Core.Disposed) UpdateOutputText();
        }

        private void UpdateOutputText()
        {
            var availableLines = Convert.ToInt32(Math.Floor(_Height / _InputBox.InnerSize.Y));
            _Output.Text = String.Join(Constants.NEW_LINE, new[] { _InputBox.Text ?? String.Empty }.Concat(_Messages.Reverse().Take(availableLines)));
        }

        internal void Open()
        {
            if (_AnimationRunning) return;
            _AnimationRunning = true;

            Visible = true;
            _InputBox.GiveFocus();
            _InputBox.StartEdit();

            _Core.AnimationManager.RunAdvanced(Position.Y, Position.Y - _Height, 0.4f, v => Position = new Vector2f(Position.X, v), a =>
            {
                IsOpen = true;
                _AnimationRunning = false;
            }, InterpolationType.OutCubic);
        }
        internal void Close()
        {
            if (_AnimationRunning) return;
            _AnimationRunning = true;

            _Core.AnimationManager.RunAdvanced(Position.Y, Position.Y + _Height, 0.4f, v => Position = new Vector2f(Position.X, v), a =>
            {
                IsOpen = false;
                Visible = false;
                _AnimationRunning = false;
                _InputBox.Text = String.Empty;
            }, InterpolationType.OutCubic);
        }
    }
}