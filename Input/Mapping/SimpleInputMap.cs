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
        private Boolean _Enabled;

        private Dictionary<Keyboard.Key, TMappedOperation> _KeyboardActions;
        private Dictionary<Mouse.Button, TMappedOperation> _MouseActions;
        private Boolean _ScrollUpActionSet;
        private TMappedOperation _ScrollUpAction;
        private Boolean _ScrollDownActionSet;
        private TMappedOperation _ScrollDownAction;


        /// <summary>
        /// The input source for this map.
        /// </summary>
        public Input Input { get; }

        /// <summary>
        /// Gets the name of this instance.
        /// </summary>
        public String Name { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SimpleInputMap{TMappedOperation}"/> is enabled.
        /// </summary>
        public Boolean Enabled
        {
            get => _Enabled;
            set
            {
                if (value) Enable();
                else Disable();
            }
        }


        /// <summary>
        /// Occurs when a mapped operation is invoked.
        /// </summary>
        public event Action<TMappedOperation, Boolean, Boolean> MappedOperationInvoked = (a, b, m) => { };
        

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleInputMap{TMappedOperation}"/> class.
        /// </summary>
        public SimpleInputMap(Input eventSource, string name)
        {
            Input = eventSource ?? throw new ArgumentNullException(nameof(eventSource));
            if (String.IsNullOrEmpty(name)) throw new ArgumentException(nameof(name));
            Name = name;

            _KeyboardActions = new Dictionary<Keyboard.Key, TMappedOperation>();
            _MouseActions = new Dictionary<Mouse.Button, TMappedOperation>();
            _ScrollUpActionSet = false;
            _ScrollDownActionSet = false;

            Enable();
            Log.Debug(nameof(SimpleInputMap<TMappedOperation>), Name, "created");
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
            Input.MouseButtonReleased += HandleMouseButtonReleased;
            Input.MouseWheelScrolled += HandleMouseWheelScrolled;
            Input.KeyPressed += HandleKeyPressed;
            Input.KeyReleased += HandleKeyReleased;
            _Enabled = true;
        }

        /// <summary>
        /// Disables this instance.
        /// </summary>
        public void Disable()
        {
            Input.MouseButtonPressed -= HandleMouseButtonPressed;
            Input.MouseButtonReleased -= HandleMouseButtonReleased;
            Input.MouseWheelScrolled -= HandleMouseWheelScrolled;
            Input.KeyPressed -= HandleKeyPressed;
            Input.KeyReleased -= HandleKeyReleased;
            _Enabled = false;
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
        /// <param name="direction">The scroll direction.</param>
        /// <param name="action">The mapped value.</param>
        /// <returns>The created InputAction</returns>
        public void AddScrollMapping(ScrollDirection direction, TMappedOperation action)
        {
            if(direction == ScrollDirection.Up)
            {
                _ScrollUpAction = action;
                _ScrollUpActionSet = true;
            }
            else
            {
                _ScrollDownAction = action;
                _ScrollDownActionSet = true;
            }
        }


        private void HandleKeyPressed(Keyboard.Key key)
        {
            RaiseMappedOperationInvoked(_KeyboardActions, key, true, false);
        }
        private void HandleKeyReleased(Keyboard.Key key)
        {
            RaiseMappedOperationInvoked(_KeyboardActions, key, false, false);
        }

        private void HandleMouseButtonPressed(Mouse.Button button)
        {
            RaiseMappedOperationInvoked(_MouseActions, button, true, true);
        }
        private void HandleMouseButtonReleased(Mouse.Button button)
        {
            RaiseMappedOperationInvoked(_MouseActions, button, false, true);
        }

        private void HandleMouseWheelScrolled(float delta)
        {
            if (delta > 0)
            {
                if (_ScrollUpActionSet) MappedOperationInvoked.Invoke(_ScrollUpAction, true, true);
            }
            else
            {
                if (_ScrollDownActionSet) MappedOperationInvoked.Invoke(_ScrollDownAction, true, true);
            }
        }


        private void RaiseMappedOperationInvoked<TKey>(Dictionary<TKey, TMappedOperation> lookup, TKey key, Boolean activate, Boolean fromMouse)
        {
            if (lookup.TryGetValue(key, out TMappedOperation operation))
            {
                MappedOperationInvoked.Invoke(operation, activate, fromMouse);
            }
        }


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (Enabled) Disable();
        }
    }

    public enum ScrollDirection
    {
        Up,
        Down
    }
}