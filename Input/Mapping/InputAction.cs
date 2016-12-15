using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackCoat.InputMapping
{
    public class InputAction<TInput, TOutput>
    {
        private TInput[] _Conditions;
        private TOutput _Output;
        private Func<TInput, Boolean> _Validator;

        public event Action<TOutput> Invoked = x => { };

        public InputAction(IEnumerable<TInput> conditions, TOutput output, Func<TInput, Boolean> validator)
        {
            if (conditions == null) throw new ArgumentNullException(nameof(conditions));
            if (conditions.Count() == 0) throw new ArgumentException("no conditions");
            if (output == null) throw new ArgumentNullException(nameof(output));
            if (validator == null) throw new ArgumentNullException(nameof(validator));

            _Conditions = conditions.ToArray();
            _Output = output;
            _Validator = validator;
        }

        public void Invoke()
        {
            foreach (var condition in _Conditions)
            {
                if (!_Validator.Invoke(condition)) return;
            }
            Invoked.Invoke(_Output);
        }
    }
}
