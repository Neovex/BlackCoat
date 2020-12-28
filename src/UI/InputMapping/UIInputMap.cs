using System;
using System.Collections.Generic;
using BlackCoat.InputMapping;

namespace BlackCoat.UI
{
    /// <summary>
    /// Specialist mapping class for mapping <see cref="MappedOperation{TCondition, TOperation}" /> to UI Events
    /// </summary>
    /// <typeparam name="TMapped">The type of the mapped.</typeparam>
    /// <seealso cref="BlackCoat.UI.UIInput" />
    public class UIInputMap<TMapped> : UIInput
    {
        // Nested ##########################################################################        
        /// <summary>
        /// List of all UI operations for input mapping.
        /// </summary>
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


        // Variables #######################################################################
        private Dictionary<TMapped, (UiOperation Activation, UiOperation Deactivation, float Direction)> _Map;


        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="UIInputMap{TMapped}"/> class.
        /// </summary>
        /// <param name="map">The input mapper to be used as input.</param>
        /// <param name="mapping">The mapping to be used to translate game actions into UI operations.</param>
        public UIInputMap(SimpleInputMap<TMapped> map, Dictionary<TMapped, (UiOperation activation, UiOperation deactivation, float direction)> mapping) : this(map.Input, mapping)
        {
            map.MappedOperationInvoked += HandleMappedOperationInvoked;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UIInputMap{TMapped}"/> class.
        /// </summary>
        /// <param name="map">The input mapper to be used as input.</param>
        /// <param name="mapping">The mapping to be used to translate game actions into UI operations.</param>
        public UIInputMap(ComplexInputMap<TMapped> map, Dictionary<TMapped, (UiOperation activation, UiOperation deactivation, float direction)> mapping) : this(map.Input, mapping)
        {
            map.MappedOperationInvoked += a => HandleMappedOperationInvoked(a, true);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UIInputMap{TMapped}"/> class.
        /// </summary>
        /// <param name="input">The raw input source.</param>
        /// <param name="mapping">The mapping to be used to translate game actions into UI operations.</param>
        /// <exception cref="ArgumentNullException">mapping</exception>
        private UIInputMap(Input input, Dictionary<TMapped, (UiOperation activation, UiOperation deactivation, float direction)> mapping) : base(input)
        {
            _Map = mapping ?? throw new ArgumentNullException(nameof(mapping));
        }


        // Methods #########################################################################
        /// <summary>
        /// Translates between game actions and UI operations.
        /// </summary>
        /// <param name="action">The mapped game action.</param>
        /// <param name="activate">Determines if this is an activation or not</param>
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