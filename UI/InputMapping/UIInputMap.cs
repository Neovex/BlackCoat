using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackCoat;
using BlackCoat.InputMapping;

namespace BlackCoat.UI
{

    public class UIInputMap<TMapped> : UIInput
    {
        public enum Operation
        {
            Move,
            BeforeConfirm,
            Confirm,
            Cancel,
            Edit
        }

        private Dictionary<TMapped, (Operation Operation, float Direction)> _Map;


        public UIInputMap(SimpleInputMap<TMapped> map, Dictionary<TMapped, (Operation operation, float direction)> mapping) : this(map.Input, mapping)
        {
            map.MappedOperationInvoked += (a, b) => HandleMappedOperationInvoked(a);
        }

        public UIInputMap(ComplexInputMap<TMapped> map, Dictionary<TMapped, (Operation operation, float direction)> mapping) : this(map.Input, mapping)
        {
            map.MappedOperationInvoked += HandleMappedOperationInvoked;
        }

        protected UIInputMap(Input input, Dictionary<TMapped, (Operation operation, float direction)> mapping) : base(input)
        {
            _Map = mapping ?? throw new ArgumentNullException(nameof(mapping));
        }


        protected void HandleMappedOperationInvoked(TMapped action)
        {
            var op = _Map[action];
            switch (op.Operation)
            {
                case Operation.Move:
                    RaiseMoveEvent(op.Direction);
                    break;
                case Operation.BeforeConfirm:
                    RaiseBeforeConfirmEvent();
                    break;
                case Operation.Confirm:
                    RaiseConfirmEvent();
                    break;
                case Operation.Cancel:
                    RaiseCancelEvent();
                    break;
                case Operation.Edit:
                    RaiseEditEvent();
                    break;
            }
        }
    }
}