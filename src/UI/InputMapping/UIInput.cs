using System;
using SFML.Window;

namespace BlackCoat.UI
{
    public class UIInput
    {
        public event Action<float> Move = d => { };
        public event Action BeforeConfirm = () => { };
        public event Action Confirm = () => { };
        public event Action Cancel = () => { };
        public event Action Edit = () => { };
        public event Action<TextEnteredEventArgs> TextEntered = t => { };
        public event Action<float> Scroll = d => { };


        public Input Input { get; }
        public bool MouseEventActive => Input.CurrentEventSource == InputSource.Mouse;


        public UIInput(Input input, bool setupDefaultKeyboardMouseMapping = false)
        {
            Input = input ?? throw new ArgumentNullException(nameof(input));
            if (setupDefaultKeyboardMouseMapping) SetupDefaultKeyboardMouseMapping();
        }

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

        protected void RaiseMoveEvent(float direction) => Move.Invoke(direction);
        protected void RaiseBeforeConfirmEvent() => BeforeConfirm.Invoke();
        protected void RaiseConfirmEvent() => Confirm.Invoke();
        protected void RaiseCancelEvent() => Cancel.Invoke();
        protected void RaiseEditEvent() => Edit.Invoke();
        protected void RaiseScrollEvent(float direction) => Scroll.Invoke(direction);

        public void RaiseTextEnteredEvent(TextEnteredEventArgs tArgs) => TextEntered.Invoke(tArgs);
    }
}