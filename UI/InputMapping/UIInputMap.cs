using System;
using System.Collections.Generic;
using System.Linq;

using BlackCoat;
using BlackCoat.InputMapping;

namespace BlackCoat.UI
{

    public class UIInputMap<TMapped> : UIInput
    {
        public enum UiOperation
        {
            None,
            Move,
            BeforeConfirm,
            Confirm,
            Cancel,
            Edit,
            Scroll
        }

        private Dictionary<TMapped, (UiOperation Operation, float Direction)> _Map;


        public UIInputMap(SimpleInputMap<TMapped> map, Dictionary<TMapped, (UiOperation operation, float direction)> mapping) : this(map.Input, mapping)
        {
            map.MappedOperationInvoked += HandleMappedOperationInvoked;
        }

        public UIInputMap(ComplexInputMap<TMapped> map, Dictionary<TMapped, (UiOperation operation, float direction)> mapping) : this(map.Input, mapping)
        {
            map.MappedOperationInvoked += a => HandleMappedOperationInvoked(a, true);
        }

        private UIInputMap(Input input, Dictionary<TMapped, (UiOperation operation, float direction)> mapping) : base(input)
        {
            _Map = mapping ?? throw new ArgumentNullException(nameof(mapping));
        }


        protected void HandleMappedOperationInvoked(TMapped action, bool activate)
        {
            if (_Map.TryGetValue(action, out var op))
            {
                switch (op.Operation)
                {
                    case UiOperation.Move:
                        if (activate) RaiseMoveEvent(op.Direction);
                        break;
                    case UiOperation.BeforeConfirm:
                        if (activate) RaiseBeforeConfirmEvent();
                        break;
                    case UiOperation.Confirm:
                        if (!activate) RaiseConfirmEvent();
                        break;
                    case UiOperation.Cancel:
                        if (activate) RaiseCancelEvent();
                        break;
                    case UiOperation.Edit:
                        if (activate) RaiseEditEvent();
                        break;
                    case UiOperation.Scroll:
                        if (activate) RaiseScrollEvent(op.Direction);
                        break;
                }
            }
        }
    }
}