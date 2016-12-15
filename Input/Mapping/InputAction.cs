using System;

namespace BlackCoat.InputMapping
{
    /// <summary>
    /// Represents a single mapped operation
    /// </summary>
    /// <typeparam name="TCondition">The type of the condition.</typeparam>
    /// <typeparam name="TOperation">The type of the operation.</typeparam>
    public class InputAction<TCondition, TOperation> // consider class rename to operation something
    {
        private TCondition _Condition;
        private TOperation _Operation;
        private Func<TCondition, Boolean> _Validator;

        /// <summary>
        /// Occurs when the operation condition is met.
        /// </summary>
        public event Action<TOperation> Invoked = x => { };

        /// <summary>
        /// Initializes a new instance of the <see cref="InputAction{TCondition, TOperation}"/> class.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="validator">The validator.</param>
        public InputAction(TCondition condition, TOperation operation, Func<TCondition, Boolean> validator)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (operation == null) throw new ArgumentNullException(nameof(operation));
            if (validator == null) throw new ArgumentNullException(nameof(validator));

            _Condition = condition;
            _Operation = operation;
            _Validator = validator;
        }

        /// <summary>
        /// Invokes the operation when the condition is met
        /// </summary>
        internal void Invoke()
        {
            if (_Validator.Invoke(_Condition)) Invoked.Invoke(_Operation);
        }
    }
}