using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlackCoat.Collision;
using SFML.System;

namespace BlackCoat.UI
{
    public abstract class Button : UICanvas
    {
        public event Action<Button> Pressed = b => { };
        public event Action<Button> Released = b => { };


        public Button(Core core, Vector2f? size = null) : base(core, size)
        {
        }
        

        protected override void HandleInputBeforeConfirm() { if (HasFocus) InvokePressed(); }
        protected override void HandleInputConfirm() { if (HasFocus) InvokeReleased(); }

        protected virtual void InvokePressed() => Pressed.Invoke(this);
        protected virtual void InvokeReleased() => Released.Invoke(this);
    }
}