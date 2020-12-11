using System;
using System.Collections.Generic;
using System.Linq;

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
        // Events ##########################################################################
        /// <summary>
        /// Occurs when a mapped operation is invoked.
        /// </summary>
        public event Action<TMappedOperation, Boolean> MappedOperationInvoked = (o, a) => { };


        // Variables #######################################################################
        private Dictionary<Keyboard.Key, TMappedOperation> _KeyboardActions;
        private Dictionary<Mouse.Button, TMappedOperation> _MouseActions;
        private Dictionary<uint, TMappedOperation> _JoystickButtonActions;
        private Dictionary<Joystick.Axis, (float, TMappedOperation)> _JoystickMovementActions;

        private uint _JoystickFilter;
        private Boolean _ScrollUpActionSet;
        private TMappedOperation _ScrollUpAction;
        private Boolean _ScrollDownActionSet;
        private TMappedOperation _ScrollDownAction;


        // Properties ######################################################################
        /// <summary>
        /// The input source for this map.
        /// </summary>
        public Input Input { get; }


        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleInputMap{TMappedOperation}" /> class.
        /// </summary>
        /// <param name="eventSource">The <see cref="Input"/> event source.</param>
        /// <param name="players">Optional filter for joystick use.</param>
        public SimpleInputMap(Input eventSource, uint joystickFilter = 0)
        {
            Input = eventSource ?? throw new ArgumentNullException(nameof(eventSource));

            _KeyboardActions = new Dictionary<Keyboard.Key, TMappedOperation>();
            _MouseActions = new Dictionary<Mouse.Button, TMappedOperation>();
            _JoystickButtonActions = new Dictionary<uint, TMappedOperation>();
            _JoystickMovementActions = new Dictionary<Joystick.Axis, (float, TMappedOperation)>();

            _JoystickFilter = joystickFilter;
            _ScrollUpActionSet = false;
            _ScrollDownActionSet = false;

            AttachEvents();
            Log.Debug(nameof(SimpleInputMap<TMappedOperation>), "created. Filter:", joystickFilter);
        }
        ~SimpleInputMap()
        {
            Dispose(false);
        }


        // Methods #########################################################################
        private void AttachEvents()
        {
            Input.MouseButtonPressed += HandleMouseButtonPressed;
            Input.MouseButtonReleased += HandleMouseButtonReleased;
            Input.MouseWheelScrolled += HandleMouseWheelScrolled;
            Input.KeyPressed += HandleKeyPressed;
            Input.KeyReleased += HandleKeyReleased;
            Input.JoystickMoved += HandleJoystickMoved;
            Input.JoystickButtonPressed += HandleJoystickButtonPressed;
            Input.JoystickButtonReleased += HandleJoystickButtonReleased;
        }

        private void DetachEvents()
        {
            Input.MouseButtonPressed -= HandleMouseButtonPressed;
            Input.MouseButtonReleased -= HandleMouseButtonReleased;
            Input.MouseWheelScrolled -= HandleMouseWheelScrolled;
            Input.KeyPressed -= HandleKeyPressed;
            Input.KeyReleased -= HandleKeyReleased;
            Input.JoystickMoved -= HandleJoystickMoved;
            Input.JoystickButtonPressed -= HandleJoystickButtonPressed;
            Input.JoystickButtonReleased -= HandleJoystickButtonReleased;
        }

        private void RaiseMappedOperationInvoked<TKey>(Dictionary<TKey, TMappedOperation> lookup, TKey key, Boolean activate)
        {
            if (lookup.TryGetValue(key, out TMappedOperation operation))
            {
                MappedOperationInvoked.Invoke(operation, activate);
            }
        }

        /// <summary>
        /// Adds a keyboard mapping.
        /// </summary>
        /// <param name="key">The Keyboard.Key.</param>
        /// <param name="action">The mapped action.</param>
        public void AddKeyboardMapping(Keyboard.Key key, TMappedOperation action)
        {
            _KeyboardActions[key] = action;
        }

        /// <summary>
        /// Adds a mouse mapping.
        /// </summary>
        /// <param name="button">The Mouse.Button.</param>
        /// <param name="action">The mapped action.</param>
        public void AddMouseMapping(Mouse.Button button, TMappedOperation action)
        {
            _MouseActions[button] = action;
        }

        /// <summary>
        /// Adds a mouse scroll mapping.
        /// </summary>
        /// <param name="direction">The scroll direction.</param>
        /// <param name="action">The mapped action.</param>
        public void AddScrollMapping(ScrollDirection direction, TMappedOperation action)
        {
            switch (direction)
            {
                case ScrollDirection.Up:
                    _ScrollUpAction = action;
                    _ScrollUpActionSet = true;
                break;
                case ScrollDirection.Down:
                    _ScrollDownAction = action;
                    _ScrollDownActionSet = true;
                break;
                default: throw new ArgumentException(nameof(direction));
            }
        }

        /// <summary>
        /// Adds a joystick button mapping.
        /// </summary>
        /// <param name="button">The joystick button id.</param>
        /// <param name="action">The mapped action.</param>
        public void AddJoystickButtonMapping(uint button, TMappedOperation action)
        {
            _JoystickButtonActions[button] = action;
        }

        /// <summary>
        /// Adds a joystick movement mapping.
        /// </summary>
        /// <param name="axis">The joy axis</param>
        /// <param name="limit">The limit when to trigger the action</param>
        /// <param name="action">The mapped action.</param>
        public void AddJoystickMovementMapping(Joystick.Axis axis, float limit, TMappedOperation action)
        {
            if (limit == 0) throw new Exception($"{nameof(limit)} must not be zero");
            _JoystickMovementActions[axis] = (limit, action);
        }


        private void HandleKeyPressed(Keyboard.Key key)
        {
            RaiseMappedOperationInvoked(_KeyboardActions, key, true);
        }
        private void HandleKeyReleased(Keyboard.Key key)
        {
            RaiseMappedOperationInvoked(_KeyboardActions, key, false);
        }

        private void HandleMouseButtonPressed(Mouse.Button button)
        {
            RaiseMappedOperationInvoked(_MouseActions, button, true);
        }
        private void HandleMouseButtonReleased(Mouse.Button button)
        {
            RaiseMappedOperationInvoked(_MouseActions, button, false);
        }

        private void HandleMouseWheelScrolled(float delta)
        {
            if (delta > 0)
            {
                if (_ScrollUpActionSet) MappedOperationInvoked.Invoke(_ScrollUpAction, true);
            }
            else
            {
                if (_ScrollDownActionSet) MappedOperationInvoked.Invoke(_ScrollDownAction, true);
            }
        }

        private void HandleJoystickMoved(uint joystickId, Joystick.Axis axis, float position)
        {
            if (joystickId == _JoystickFilter &&
               _JoystickMovementActions.TryGetValue(axis, out (float limit, TMappedOperation action) kvp))
            {
                var activate = (kvp.limit < 0 && position < kvp.limit) ||
                               (kvp.limit > 0 && position > kvp.limit);
                MappedOperationInvoked.Invoke(kvp.action, activate);
            }
        }

        private void HandleJoystickButtonPressed(uint joystickId, uint button)
        {
            if (joystickId == _JoystickFilter) RaiseMappedOperationInvoked(_JoystickButtonActions, button, true);
        }

        private void HandleJoystickButtonReleased(uint joystickId, uint button)
        {
            if (joystickId == _JoystickFilter) RaiseMappedOperationInvoked(_JoystickButtonActions, button, false);
        }

        protected virtual void Dispose(bool managed)
        {
            if (managed) DetachEvents();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}