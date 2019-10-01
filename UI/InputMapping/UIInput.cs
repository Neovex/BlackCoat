using System;
using SFML.Window;

namespace BlackCoat.UI
{
    public class UIInput
    {
        public event Action<float> Move = d => { };
        public event Action<bool> BeforeConfirm = m => { };
        public event Action<bool> Confirm = m => { };
        public event Action Cancel = () => { };
        public event Action Edit = () => { };
        public event Action<TextEnteredEventArgs> TextEntered = t => { };


        public Input Input { get; }

        public UIInput(Input input, bool setupDefaultKeyboardMouseMapping = false)
        {
            Input = input ?? throw new ArgumentNullException(nameof(input));
            if (setupDefaultKeyboardMouseMapping) SetupDefaultKeyboardMouseMapping();
        }

        private void SetupDefaultKeyboardMouseMapping()
        {
            Input.MouseButtonPressed += HandleMouseDown;
            Input.MouseButtonReleased += HandleMouseUp;
            Input.KeyPressed += HandleKeyboardDown;
            Input.KeyReleased += HandleKeyboardUp;
            Input.TextEntered += RaiseTextEnteredEvent;
        }

        private void HandleMouseDown(Mouse.Button btn)
        {
            if (btn == Mouse.Button.Left) RaiseBeforeConfirmEvent(true);
        }
        private void HandleMouseUp(Mouse.Button btn)
        {
            switch (btn)
            {
                case Mouse.Button.Left:
                    RaiseConfirmEvent(true);
                    break;
                case Mouse.Button.Right:
                    RaiseCancelEvent();
                    break;
            }
        }

        private void HandleKeyboardDown(Keyboard.Key key)
        {
            if (key == Keyboard.Key.Return) RaiseBeforeConfirmEvent(false);
        }
        private void HandleKeyboardUp(Keyboard.Key key)
        {
            switch (key)
            {
                case Keyboard.Key.Escape:
                    RaiseCancelEvent();
                    break;
                case Keyboard.Key.Return:
                    RaiseConfirmEvent(false);
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
        protected void RaiseBeforeConfirmEvent(bool fromMouse) => BeforeConfirm.Invoke(fromMouse);
        protected void RaiseConfirmEvent(bool fromMouse) => Confirm.Invoke(fromMouse);
        protected void RaiseCancelEvent() => Cancel.Invoke();
        protected void RaiseEditEvent() => Edit.Invoke();
        public void RaiseTextEnteredEvent(TextEnteredEventArgs tArgs) => TextEntered.Invoke(tArgs);
    }
}