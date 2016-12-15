using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;

namespace BlackCoat.InputMapping
{
    /// <summary>
    /// Represents a mapping between input events and a generic operation identifier.
    /// </summary>
    /// <typeparam name="TMappedOperation">The type of the mapped operation.</typeparam>
    /// <seealso cref="System.IDisposable" />
    public class InputMap<TMappedOperation> : IDisposable // TODO: check inheritance & Action structure
    {
        private List<InputAction<Keyboard.Key, TMappedOperation>> _KeyboardActions;
        private List<InputAction<Mouse.Button, TMappedOperation>> _MouseActions;
        private List<InputAction<float, TMappedOperation>> _ScrollActions;

        /// <summary>
        /// Gets a value indicating whether this <see cref="InputMap{TMappedOperation}"/> is enabled.
        /// </summary>
        public Boolean Enabled { get; private set; }

        /// <summary>
        /// Occurs when a mapped operation is invoked.
        /// </summary>
        public event Action<TMappedOperation> MappedOperationInvoked = a => { };


        //CTOR
        /// <summary>
        /// Initializes a new instance of the <see cref="InputMap{TMappedOperation}"/> class.
        /// </summary>
        public InputMap()
        {
            Log.Debug("created");
            _KeyboardActions = new List<InputAction<Keyboard.Key, TMappedOperation>>();
            _MouseActions = new List<InputAction<Mouse.Button, TMappedOperation>>();
            _ScrollActions = new List<InputAction<float, TMappedOperation>>();
            Enable();
        }
        ~InputMap()
        {
            Dispose();
        }

        /// <summary>
        /// Enables this instance.
        /// </summary>
        public void Enable()
        {
            Input.MouseButtonPressed += HandleMouseButtonPressed;
            Input.MouseWheelScrolled += HandleMouseWheelScrolled;
            Input.KeyPressed += HandleKeyPressed;
            Enabled = true;
        }

        /// <summary>
        /// Disables this instance.
        /// </summary>
        public void Disable()
        {
            Input.MouseButtonPressed += HandleMouseButtonPressed;
            Input.MouseWheelScrolled += HandleMouseWheelScrolled;
            Input.KeyPressed += HandleKeyPressed;
            Enabled = false;
        }

        /// <summary>
        /// Adds a keyboard mapping.
        /// </summary>
        /// <param name="key">The Keyboard.Key.</param>
        /// <param name="action">The mapped value.</param>
        /// <returns>The created InputAction</returns>
        public InputAction<Keyboard.Key, TMappedOperation> AddKeyboardMapping(Keyboard.Key key, TMappedOperation action)
        {
            var a = new InputAction<Keyboard.Key, TMappedOperation>(key, action, Input.IsKeyDown);
            a.Invoked += RaiseMappedOperationInvoked;
            _KeyboardActions.Add(a);
            return a;
        }

        /// <summary>
        /// Adds a mouse mapping.
        /// </summary>
        /// <param name="button">The Mouse.Button.</param>
        /// <param name="action">The mapped value.</param>
        /// <returns>The created InputAction</returns>
        public InputAction<Mouse.Button, TMappedOperation> AddMouseMapping(Mouse.Button button, TMappedOperation action)
        {
            var a = new InputAction<Mouse.Button, TMappedOperation>(button, action, Input.IsMButtonDown);
            a.Invoked += RaiseMappedOperationInvoked;
            _MouseActions.Add(a);
            return a;
        }

        /// <summary>
        /// Adds a scroll mapping.
        /// </summary>
        /// <param name="delta">The delta condition.</param>
        /// <param name="action">The mapped value.</param>
        /// <returns>The created InputAction</returns>
        public InputAction<float, TMappedOperation> AddScrollMapping(float delta, TMappedOperation action)
        {
            var a = new InputAction<float, TMappedOperation>(delta, action, ValidateDelta);
            a.Invoked += RaiseMappedOperationInvoked;
            _ScrollActions.Add(a);
            return a;
        }

        private bool ValidateDelta(float d)
        {
            return Math.Sign(d) == Math.Sign(Input.MouseWheelDelta);
        }

        private void HandleKeyPressed(Keyboard.Key key)
        {
            foreach (var action in _KeyboardActions) action.Invoke();
        }

        private void HandleMouseButtonPressed(Mouse.Button button)
        {
            foreach (var action in _MouseActions) action.Invoke();
        }

        private void HandleMouseWheelScrolled(float delta)
        {
            foreach (var action in _ScrollActions) action.Invoke();
        }


        private void RaiseMappedOperationInvoked(TMappedOperation operation)
        {
            MappedOperationInvoked.Invoke(operation);
        }


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Log.Debug("dispose");
            if (Enabled) Disable();
        }
    }
}