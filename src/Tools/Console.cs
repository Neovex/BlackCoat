using System;
using System.Linq;
using System.Collections.Generic;

using SFML.System;
using SFML.Window;
using SFML.Graphics;

using BlackCoat.UI;
using BlackCoat.Animation;


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
        /// <summary>
        /// Occurs whenever a command is issued.
        /// </summary>
        internal event Action<String> Command;


        // Variables #######################################################################
        private Single _Height;
        private Boolean _AnimationRunning;
        private TextBox _InputBox;
        private Label _Output;
        private Queue<String> _Messages;


        // Properties ######################################################################        
        /// <summary>
        /// Determines whether the <see cref="Console"/> is open.
        /// </summary>
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
            Add(new OffsetContainer(_Core, Orientation.Vertical, 0,
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
                    Padding = new FloatRect(3,0,3,0),
                    TextColor = _InputBox.TextColor,
                    CharacterSize = _FONT_SIZE,
                }
            ));

            // Events
            Log.OnLog += LogMessage;
            _Core.DeviceResized += UpdateDisplayProportions;
            Input.Input.KeyPressed += HandleKeyPressed;

            // Init
            UpdateDisplayProportions(_Core.DeviceSize);
            Log.Debug(nameof(Console), "ready");
        }


        // Methods #########################################################################        
        /// <summary>
        /// Keeps the display proportions aligned with the current render device.
        /// </summary>
        /// <param name="size">The size of the current render device.</param>
        private void UpdateDisplayProportions(Vector2f size)
        {
            _Height = size.Y / 3;
            Resize(new Vector2f(size.X, _Height));
            Position = new Vector2f(Position.X , size.Y - (IsOpen ? _Height : 0));
            _InputBox.MinSize = new Vector2f(size.X, _InputBox.MinSize.Y);
            UpdateOutputText();
        }

        /// <summary>
        /// Handles the key presses for opening and closing the <see cref="Console"/>.
        /// </summary>
        /// <param name="key">The key being pressed.</param>
        private void HandleKeyPressed(Keyboard.Key key)
        {
            if (Input.Input.ControlKeyPressed && Input.Input.ShiftKeyPressed && key == Keyboard.Key.Num1)
            {
                if (IsOpen) Close();
                else Open();
            }
            else if (IsOpen && key == Keyboard.Key.Enter)
            {
                if (!String.IsNullOrWhiteSpace(_InputBox.Text))
                {
                    Command.Invoke(_InputBox.Text.Trim());
                }
                _InputBox.Text = String.Empty;
                UpdateOutputText();
            }
            else if (IsOpen && key == Keyboard.Key.Escape)
            {
                Close();
            }
        }

        /// <summary>
        /// Logs a message to the <see cref="Console"/>.
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <param name="lvl">The <see cref="LogLevel"/> of the message.</param>
        private void LogMessage(String msg, LogLevel lvl)
        {
            _Messages.Enqueue(msg.Replace(Environment.NewLine, Constants.NEW_LINE));
            if (_Messages.Count > _MAX_MESSAGE_HISTORY) _Messages.Dequeue();
            if(!_Core.Disposed) UpdateOutputText();
        }

        /// <summary>
        /// Updates the <see cref="Console"/>s output history.
        /// </summary>
        private void UpdateOutputText()
        {
            var availableLines = Convert.ToInt32(Math.Floor(_Height / _InputBox.InnerSize.Y));
            _Output.Text = String.Join(Constants.NEW_LINE, new[] { _InputBox.Text ?? String.Empty }.Concat(_Messages.Reverse().Take(availableLines)));
        }

        /// <summary>
        /// Opens the <see cref="Console"/>.
        /// </summary>
        internal void Open()
        {
            if (_AnimationRunning) return;
            _AnimationRunning = true;

            Visible = true;
            BlackCoat.Input.MASTER_OVERRIDE = Input.Input;
            _InputBox.GiveFocus();
            _InputBox.StartEdit();

            _Core.AnimationManager.RunAdvanced(Position.Y, Position.Y - _Height, 0.4f, v => Position = new Vector2f(Position.X, v), a =>
            {
                IsOpen = true;
                _AnimationRunning = false;
            }, InterpolationType.OutCubic);
        }

        /// <summary>
        /// Closes the <see cref="Console"/>.
        /// </summary>
        internal void Close()
        {
            if (_AnimationRunning) return;
            _AnimationRunning = true;

            _Core.AnimationManager.RunAdvanced(Position.Y, Position.Y + _Height, 0.4f, v => Position = new Vector2f(Position.X, v), a =>
            {
                IsOpen = false;
                Visible = false;
                BlackCoat.Input.MASTER_OVERRIDE = null;
                _AnimationRunning = false;
                _InputBox.Text = String.Empty;
            }, InterpolationType.OutCubic);
        }
    }
}