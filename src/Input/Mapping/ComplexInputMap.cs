using System;
using System.Collections.Generic;
using SFML.Window;

namespace BlackCoat.InputMapping
{
    /// <summary>
    /// Represents a mapping between multiple input events and a generic operation identifier.
    /// </summary>
    /// <typeparam name="TMappedOperation">The type of the mapped operation.</typeparam>
    /// <seealso cref="System.IDisposable" />
    public class ComplexInputMap<TMappedOperation>
    {
        // Variables #######################################################################
        private readonly List<MappedOperation<Keyboard.Key, TMappedOperation>> _KeyboardActions;
        private readonly List<MappedOperation<Mouse.Button, TMappedOperation>> _MouseActions;
        private readonly List<MappedOperation<ScrollDirection, TMappedOperation>> _ScrollActions;


        // Properties ######################################################################
        /// <summary>
        /// The input source for this map.
        /// </summary>
        public Input Input { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="InputMap{TMappedOperation}"/> is enabled.
        /// </summary>
        public Boolean Enabled { get; private set; }

        /// <summary>
        /// Occurs when a mapped operation is invoked.
        /// </summary>
        public event Action<TMappedOperation> MappedOperationInvoked = o => { };


        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="ComplexInputMap{TMappedOperation}"/> class.
        /// </summary>
        public ComplexInputMap(Input eventSource)
        {
            Input = eventSource ?? throw new ArgumentNullException(nameof(eventSource));
            _KeyboardActions = new List<MappedOperation<Keyboard.Key, TMappedOperation>>();
            _MouseActions = new List<MappedOperation<Mouse.Button, TMappedOperation>>();
            _ScrollActions = new List<MappedOperation<ScrollDirection, TMappedOperation>>();
            Enable();
            Log.Debug("created");
        }
        ~ComplexInputMap()
        {
            if (Enabled) Disable();
        }


        // Methods #########################################################################
        /// <summary>
        /// Enables this instance.
        /// </summary>
        public void Enable()
        {
            Input.MouseButtonPressed += HandleMouseButtonPressed;
            Input.KeyPressed += HandleKeyPressed;
            Enabled = true;
        }

        /// <summary>
        /// Disables this instance.
        /// </summary>
        public void Disable()
        {
            Input.MouseButtonPressed -= HandleMouseButtonPressed;
            Input.KeyPressed -= HandleKeyPressed;
            Enabled = false;
        }

        /// <summary>
        /// Adds a keyboard mapping.
        /// </summary>
        /// <param name="keys">The Keyboard.Keys.</param>
        /// <param name="action">The mapped value.</param>
        /// <returns>The created InputAction</returns>
        public MappedOperation<Keyboard.Key, TMappedOperation> AddKeyboardMapping(Keyboard.Key[] keys, TMappedOperation action)
        {
            var a = new MappedOperation<Keyboard.Key, TMappedOperation>(keys, action, Input.IsKeyDown);
            a.Invoked += RaiseMappedOperationInvoked;
            _KeyboardActions.Add(a);
            return a;
        }

        /// <summary>
        /// Adds a mouse mapping.
        /// </summary>
        /// <param name="buttons">The Mouse.Buttons.</param>
        /// <param name="action">The mapped value.</param>
        /// <returns>The created InputAction</returns>
        public MappedOperation<Mouse.Button, TMappedOperation> AddMouseMapping(Mouse.Button[] buttons, TMappedOperation action)
        {
            var a = new MappedOperation<Mouse.Button, TMappedOperation>(buttons, action, Input.IsMButtonDown);
            a.Invoked += RaiseMappedOperationInvoked;
            _MouseActions.Add(a);
            return a;
        }

        private void HandleKeyPressed(Keyboard.Key key)
        {
            foreach (var action in _KeyboardActions) action.Invoke();
        }

        private void HandleMouseButtonPressed(Mouse.Button button)
        {
            foreach (var action in _MouseActions) action.Invoke();
        }

        private void RaiseMappedOperationInvoked(TMappedOperation operation)
        {
            MappedOperationInvoked.Invoke(operation);
        }
    }
}