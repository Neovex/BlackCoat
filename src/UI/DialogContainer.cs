using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace BlackCoat.UI
{
    internal sealed class DialogContainer : Canvas
    {
        private UIComponent _CallingComponent;

        public override UIInput Input
        {
            get => base.Input;
            set
            {
                if (Input != null) Input.Confirm -= HandleInputConfirm;
                if (value != null) value.Confirm += HandleInputConfirm;
                base.Input = value;
            }
        }

        internal DialogContainer(Core core, UIComponent callingComponent, params UIComponent[] components) : base(core, core.DeviceSize, components)
        {
            _CallingComponent = callingComponent ?? throw new ArgumentNullException(nameof(callingComponent));
        }

        internal void Close()
        {
            if (Disposed) return;
            Parent.Remove(this);
            _CallingComponent.GiveFocus();
            _CallingComponent = null;
            DIALOG = null;
            Dispose();
        }

        private void HandleInputConfirm(bool fromMouse)
        {
            _Core.AnimationManager.Wait(0.01f, Close); // hacky I know - but works absolutely beautiful
        }

        protected override void HandleInputCancel()
        {
            base.HandleInputCancel();
            Close();
        }
        protected override void Dispose(bool disposing)
        {
            if (Input != null) Input.Confirm -= HandleInputConfirm;
            base.Dispose(disposing);
        }
    }
}
