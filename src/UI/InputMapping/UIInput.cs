using System;
using SFML.Window;

namespace BlackCoat.UI
{
    /// <summary>
    /// Base class for mapping Input Events to UI Events
    /// </summary>
    public class UIInput
    {
        // Events ##########################################################################
        /// <summary>
        /// Occurs when the focus tries to move.
        /// </summary>
        public event Action<float> Move = d => { };

        /// <summary>
        /// Occurs before a confirm event.
        /// </summary>
        public event Action BeforeConfirm = () => { };

        /// <summary>
        /// Occurs when the user confirms an operation. I.e.: Clicks on a button.
        /// </summary>
        public event Action Confirm = () => { };

        /// <summary>
        /// Occurs when the user wants to chancel the current operation or dialog.
        /// </summary>
        public event Action Cancel = () => { };

        /// <summary>
        /// Occurs when the user desires to enter an edit state. I.e.: Focusing a text field.
        /// </summary>
        public event Action Edit = () => { };

        /// <summary>
        /// Occurs when the user enters text.
        /// </summary>
        public event Action<TextEnteredEventArgs> TextEntered = t => { };

        /// <summary>
        /// Occurs when the user tries to scroll through content.
        /// </summary>
        public event Action<float> Scroll = d => { };


        // Properties ######################################################################
        /// <summary>
        /// Current Input event source.
        /// </summary>
        public Input Input { get; }

        /// <summary>
        /// Gets a value indicating whether mouse events are available on the current input source.
        /// </summary>
        public bool MouseEventActive => Input.CurrentEventSource == InputSource.Mouse;


        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="UIInput"/> class.
        /// </summary>
        /// <param name="input">The input source.</param>
        /// <param name="setupDefaultKeyboardMouseMapping">If set to <c>true</c> a simple default keyboard/mouse mapping will be created.</param>
        /// <exception cref="ArgumentNullException">input</exception>
        public UIInput(Input input, bool setupDefaultKeyboardMouseMapping = false)
        {
            Input = input ?? throw new ArgumentNullException(nameof(input));
            if (setupDefaultKeyboardMouseMapping) SetupDefaultKeyboardMouseMapping();
        }


        // Methods #########################################################################        
        /// <summary>
        /// Raises the text entered event.
        /// </summary>
        /// <param name="tArgs">The <see cref="TextEnteredEventArgs"/> instance containing the event data.</param>
        public void RaiseTextEnteredEvent(TextEnteredEventArgs tArgs) => TextEntered.Invoke(tArgs);

        /// <summary>
        /// Raises the move event.
        /// </summary>
        /// <param name="direction">The movement direction.</param>
        protected void RaiseMoveEvent(float direction) => Move.Invoke(direction);

        /// <summary>
        /// Raises the before confirm event.
        /// </summary>
        protected void RaiseBeforeConfirmEvent() => BeforeConfirm.Invoke();

        /// <summary>
        /// Raises the confirm event.
        /// </summary>
        protected void RaiseConfirmEvent() => Confirm.Invoke();

        /// <summary>
        /// Raises the cancel event.
        /// </summary>
        protected void RaiseCancelEvent() => Cancel.Invoke();

        /// <summary>
        /// Raises the edit event.
        /// </summary>
        protected void RaiseEditEvent() => Edit.Invoke();

        /// <summary>
        /// Raises the scroll event.
        /// </summary>
        /// <param name="direction">The scroll direction.</param>
        protected void RaiseScrollEvent(float direction) => Scroll.Invoke(direction);

        #region Default Mapping
        private void SetupDefaultKeyboardMouseMapping()
        {
            Input.MouseButtonPressed += HandleMouseDown;
            Input.MouseButtonReleased += HandleMouseUp;
            Input.MouseWheelScrolled += HandleMouseScroll;

            Input.KeyPressed += HandleKeyboardDown;
            Input.KeyReleased += HandleKeyboardUp;

            Input.TextEntered += RaiseTextEnteredEvent;
        }

        private void HandleMouseDown(Mouse.Button btn)
        {
            if (btn == Mouse.Button.Left) RaiseBeforeConfirmEvent();
        }

        private void HandleMouseUp(Mouse.Button btn)
        {
            switch (btn)
            {
                case Mouse.Button.Left:
                    RaiseConfirmEvent();
                    break;
                case Mouse.Button.Right:
                    RaiseCancelEvent();
                    break;
            }
        }

        private void HandleMouseScroll(float mouseWheelDelta)
        {
            RaiseScrollEvent(mouseWheelDelta > 0 ? Direction.UP : Direction.DOWN);
        }

        private void HandleKeyboardDown(Keyboard.Key key)
        {
            if (key == Keyboard.Key.Enter) RaiseBeforeConfirmEvent();
        }

        private void HandleKeyboardUp(Keyboard.Key key)
        {
            switch (key)
            {
                case Keyboard.Key.Escape:
                    RaiseCancelEvent();
                    break;
                case Keyboard.Key.Enter:
                    RaiseConfirmEvent();
                    break;
                case Keyboard.Key.A:
                case Keyboard.Key.Left:
                    RaiseMoveEvent(Direction.LEFT);
                    break;
                case Keyboard.Key.D:
                case Keyboard.Key.Right:
                    RaiseMoveEvent(Direction.RIGHT);
                    break;
                case Keyboard.Key.W:
                case Keyboard.Key.Up:
                    RaiseMoveEvent(Direction.UP);
                    break;
                case Keyboard.Key.S:
                case Keyboard.Key.Down:
                    RaiseMoveEvent(Direction.DOWN);
                    break;
                case Keyboard.Key.F2:
                    RaiseEditEvent();
                    break;
            }
        }
        #endregion
    }
}