using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;

namespace BlackCoat.InputMapping
{
    /// <summary>
    /// Represents a mapping between single input events and a generic operation identifier.
    /// </summary>
    /// <typeparam name="TMappedOperation">The type of the mapped operation.</typeparam>
    /// <seealso cref="System.IDisposable" />
    public class SimpleInputMap<TMappedOperation> : IDisposable
    {
        private Dictionary<Keyboard.Key, TMappedOperation> _KeyboardActions;
        private Dictionary<Mouse.Button, TMappedOperation> _MouseActions;
        private Dictionary<float, TMappedOperation> _ScrollActions;

        /// <summary>
        /// Gets a value indicating whether this <see cref="InputMap{TMappedOperation}"/> is enabled.
        /// </summary>
        public Boolean Enabled { get; private set; }
        
        /// <summary>
        /// Gets the name of this instance.
        /// </summary>
        public String Name { get; private set; }

        /// <summary>
        /// Occurs when a mapped operation is invoked.
        /// </summary>
        public event Action<TMappedOperation> MappedOperationInvoked = a => { };


        //CTOR
        /// <summary>
        /// Initializes a new instance of the <see cref="InputMap{TMappedOperation}"/> class.
        /// </summary>
        public SimpleInputMap(string name)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentException(nameof(name));
            Name = name;
            Log.Debug(nameof(SimpleInputMap<TMappedOperation>), Name, "created");

            _KeyboardActions = new Dictionary<Keyboard.Key, TMappedOperation>();
            _MouseActions = new Dictionary<Mouse.Button, TMappedOperation>();
            _ScrollActions = new Dictionary<float, TMappedOperation>();

            Enable();
        }
        ~SimpleInputMap()
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
            Input.MouseButtonPressed -= HandleMouseButtonPressed;
            Input.MouseWheelScrolled -= HandleMouseWheelScrolled;
            Input.KeyPressed -= HandleKeyPressed;
            Enabled = false;
        }

        /// <summary>
        /// Adds a keyboard mapping.
        /// </summary>
        /// <param name="key">The Keyboard.Key.</param>
        /// <param name="action">The mapped value.</param>
        /// <returns>The created InputAction</returns>
        public void AddKeyboardMapping(Keyboard.Key key, TMappedOperation action)
        {
            _KeyboardActions[key] = action;
        }

        /// <summary>
        /// Adds a mouse mapping.
        /// </summary>
        /// <param name="button">The Mouse.Button.</param>
        /// <param name="action">The mapped value.</param>
        /// <returns>The created InputAction</returns>
        public void AddMouseMapping(Mouse.Button button, TMappedOperation action)
        {
            _MouseActions[button] = action;
        }

        /// <summary>
        /// Adds a scroll mapping.
        /// </summary>
        /// <param name="delta">The delta condition.</param>
        /// <param name="action">The mapped value.</param>
        /// <returns>The created InputAction</returns>
        public void AddScrollMapping(float delta, TMappedOperation action)
        {
            _ScrollActions[delta] = action;
        }

        private bool ValidateDelta(float d)
        {
            return Math.Sign(d) == Math.Sign(Input.MouseWheelDelta);
        }

        private void HandleKeyPressed(Keyboard.Key key)
        {
            TMappedOperation action;
            if (_KeyboardActions.TryGetValue(key, out action))
            {
                RaiseMappedOperationInvoked(action);
            }
        }

        private void HandleMouseButtonPressed(Mouse.Button button)
        {
            TMappedOperation action;
            if (_MouseActions.TryGetValue(button, out action))
            {
                RaiseMappedOperationInvoked(action);
            }
        }

        private void HandleMouseWheelScrolled(float delta)
        {
            if (delta != 0)
            {
                TMappedOperation action = _ScrollActions.First(kvp => ValidateDelta(kvp.Key)).Value;
                RaiseMappedOperationInvoked(action);
            }
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
            if (Enabled) Disable();
        }
    }
}