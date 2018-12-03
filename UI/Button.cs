using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlackCoat.Collision;
using SFML.System;

namespace BlackCoat.UI
{
    public class Button : UICanvas
    {
        public event Action<Button> Pressed = b => { };
        public Action<Button> InitPressed { set => Pressed += value; }
        protected virtual void InvokePressed() => Pressed.Invoke(this);

        public event Action<Button> Released = b => { };
        public Action<Button> InitReleased { set => Released += value; }
        protected virtual void InvokeReleased() => Released.Invoke(this);


        public Button(Core core, Vector2f? size = null) : base(core, size)
        {
            CanFocus = true;
        }


        public override bool GiveFocus() => HasFocus = true;

        protected override void HandleInputBeforeConfirm() { if (HasFocus) InvokePressed(); }
        protected override void HandleInputConfirm() { if (HasFocus) InvokeReleased(); }
    }
}