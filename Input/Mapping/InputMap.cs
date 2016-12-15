using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;

namespace BlackCoat.InputMapping
{
    public class InputMap<TMappedAction> : IDisposable // TODO: check inheritance & Action structure
    {
        private List<InputAction<Keyboard.Key, TMappedAction>> _KeyboardActions;
        private List<InputAction<Mouse.Button, TMappedAction>> _MouseActions;
        private List<InputAction<float, TMappedAction>> _ScrollActions;

        public Boolean Enabled { get; private set; }

        public InputMap()
        {
            Enable();
        }
        ~InputMap()
        {
            Dispose();
        }

        public void Enable()
        {
            Input.MouseButtonPressed += HandleMouseButtonPressed;
            Input.MouseWheelScrolled += HandleMouseWheelScrolled;
            Input.KeyPressed += HandleKeyPressed;
            Enabled = true;
        }

        public void Disable()
        {
            Input.MouseButtonPressed += HandleMouseButtonPressed;
            Input.MouseWheelScrolled += HandleMouseWheelScrolled;
            Input.KeyPressed += HandleKeyPressed;
            Enabled = false;
        }

        public void AddKeyboardMapping(Keyboard.Key key, TMappedAction action)
        {
            _KeyboardActions.Add(new InputAction<Keyboard.Key, TMappedAction>(new[] { key }, action, Input.IsKeyDown));
        }

        public void AddMouseMapping(Mouse.Button button, TMappedAction action)
        {
            _MouseActions.Add(new InputAction<Mouse.Button, TMappedAction>(new[] { button }, action, Input.IsMButtonDown));
        }

        public void AddScrollMapping(float delta, TMappedAction action)
        {
            _ScrollActions.Add(new InputAction<float, TMappedAction>(new[] { delta }, action, d => Math.Sign(d) == Math.Sign(Input.MouseWheelDelta)));
        }


        private void HandleMouseButtonPressed(Mouse.Button button)
        {
            foreach (var action in _KeyboardActions) action.Invoke();
        }

        private void HandleKeyPressed(Keyboard.Key key)
        {

            foreach (var action in _MouseActions) action.Invoke();
        }

        private void HandleMouseWheelScrolled(float delta)
        {
            foreach (var action in _ScrollActions) action.Invoke();
        }


        public void Dispose()
        {
            if (Enabled) Disable();
        }
    }
}