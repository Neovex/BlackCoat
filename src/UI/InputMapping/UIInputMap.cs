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

        private Dictionary<TMapped, (UiOperation Activation, UiOperation Deactivation, float Direction)> _Map;


        public UIInputMap(SimpleInputMap<TMapped> map, Dictionary<TMapped, (UiOperation activation, UiOperation deactivation, float direction)> mapping) : this(map.Input, mapping)
        {
            map.MappedOperationInvoked += HandleMappedOperationInvoked;
        }

        public UIInputMap(ComplexInputMap<TMapped> map, Dictionary<TMapped, (UiOperation activation, UiOperation deactivation, float direction)> mapping) : this(map.Input, mapping)
        {
            map.MappedOperationInvoked += a => HandleMappedOperationInvoked(a, true);
        }

        private UIInputMap(Input input, Dictionary<TMapped, (UiOperation activation, UiOperation deactivation, float direction)> mapping) : base(input)
        {
            _Map = mapping ?? throw new ArgumentNullException(nameof(mapping));
        }


        protected void HandleMappedOperationInvoked(TMapped action, bool activate)
        {
            if (_Map.TryGetValue(action, out var op))
            {
                switch (activate ? op.Activation : op.Deactivation)
                {
                    case UiOperation.Move:
                        RaiseMoveEvent(op.Direction);
                        break;
                    case UiOperation.BeforeConfirm:
                        RaiseBeforeConfirmEvent();
                        break;
                    case UiOperation.Confirm:
                        RaiseConfirmEvent();
                        break;
                    case UiOperation.Cancel:
                        RaiseCancelEvent();
                        break;
                    case UiOperation.Edit:
                        RaiseEditEvent();
                        break;
                    case UiOperation.Scroll:
                        RaiseScrollEvent(op.Direction);
                        break;
                }
            }
        }
    }
}