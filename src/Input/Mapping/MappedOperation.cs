using System;
using System.Linq;

namespace BlackCoat.InputMapping
{
    /// <summary>
    /// Represents a single mapped operation
    /// </summary>
    /// <typeparam name="TCondition">The type of the conditions.</typeparam>
    /// <typeparam name="TOperation">The type of the operation.</typeparam>
    public class MappedOperation<TCondition, TOperation>
    {
        protected TCondition[] _Conditions;
        protected TOperation _Operation;
        protected Func<TCondition, Boolean> _Validator;

        /// <summary>
        /// Occurs when the operation condition is met.
        /// </summary>
        public event Action<TOperation> Invoked = o => { };

        /// <summary>
        /// Initializes a new instance of the <see cref="MappedOperation{TCondition, TOperation}"/> class with multiple conditions.
        /// </summary>
        /// <param name="conditions">The condition array.</param>
        /// <param name="operation">The operation value.</param>
        /// <param name="validator">The invocation validator.</param>
        public MappedOperation(TCondition[] conditions, TOperation operation, Func<TCondition, Boolean> validator)
        {
            if (conditions == null) throw new ArgumentNullException(nameof(conditions));
            if (conditions.Length == 0) throw new ArgumentException(nameof(conditions));
            if (operation == null) throw new ArgumentNullException(nameof(operation));
            if (validator == null) throw new ArgumentNullException(nameof(validator));

            _Conditions = conditions;
            _Operation = operation;
            _Validator = validator;
        }

        /// <summary>
        /// Invokes the operation when the condition is met
        /// </summary>
        public virtual void Invoke()
        {
            if(_Conditions.All(_Validator)) Invoked.Invoke(_Operation);
        }
    }
}