using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackCoat.Collision;
using SFML.System;

namespace BlackCoat.UI
{
    /// <summary>
    /// UI Container with an adjustable size which is independent from its child components.
    /// </summary>
    /// <seealso cref="BlackCoat.UI.UIContainer" />
    public class UICanvas : UIContainer
    {
        protected Vector2f _Size;
        private bool _DockX;
        private bool _DockY;

        public override Vector2f InnerSize
        {
            get
            {
                var realInnerSize = base.InnerSize;
                return new Vector2f(Math.Max(realInnerSize.X, _Size.X), Math.Max(realInnerSize.Y, _Size.Y));
            }
        }
        public override Vector2f OuterSize
        {
            get
            {
                var realInnerSize = base.InnerSize;
                return new Vector2f(DockX ? realInnerSize.X : InnerSize.X, DockY ? realInnerSize.Y : InnerSize.Y) 
                     + new Vector2f(Padding.Left + Padding.Width, Padding.Top + Padding.Height);
            }
        }


        public bool DockX
        {
            get => _DockX;
            set { _DockX = value; UpdateDocking(); }
        }
        public bool DockY
        {
            get => _DockY;
            set { _DockY = value; UpdateDocking(); }
        }

        public override UIContainer Container
        {
            get => base.Container;
            set
            {
                if (base.Container != null) base.Container.SizeChanged -= UpdateDocking;
                base.Container = value;
                if (base.Container != null) base.Container.SizeChanged += UpdateDocking;
            }
        }


        public UICanvas(Core core, Vector2f? size = null) : base(core)
        {
            if (size.HasValue) SetSize(size.Value);
        }


        public void SetSize(Vector2f size)
        {
            if (_Size == size) return;
            _Size = size;
            InvokeSizeChanged();
            _Background.Size = _Size;
        }

        protected override void HandleChildComponentModified(UIComponent c)
        {
            var cPos = c.Position;
            var cSize = c.OuterSize;
            if ((c.Position.X - c.Origin.X) - c.Padding.Left < 0) cPos.X = c.Origin.X + c.Padding.Left;
            if ((c.Position.Y - c.Origin.Y) - c.Padding.Top < 0) cPos.Y = c.Origin.Y + c.Padding.Top;
            if ((c.Position.X - c.Origin.X) + cSize.X - c.Padding.Left > InnerSize.X) cPos.X = InnerSize.X - cSize.X + c.Origin.X + c.Padding.Left;
            if ((c.Position.Y - c.Origin.Y) + cSize.Y - c.Padding.Top > InnerSize.Y) cPos.Y = InnerSize.Y - cSize.Y + c.Origin.Y + c.Padding.Top;
            c.Position = cPos;
            base.HandleChildComponentModified(c);
        }

        private void UpdateDocking(UIComponent c = null)
        {
            if (Container == null) return;
            Position = new Vector2f(DockX ? Padding.Left : Position.X,
                                    DockY ? Padding.Top : Position.Y);

            if (_Size.X == 0) _Size.X = base.InnerSize.X;
            if (_Size.Y == 0) _Size.Y = base.InnerSize.Y;

            var containerSize = Container.InnerSize;
            SetSize(new Vector2f(DockX ? containerSize.X - (Padding.Left + Padding.Width) : InnerSize.X,
                                 DockY ? containerSize.Y - (Padding.Top + Padding.Height) : InnerSize.Y));
        }

    }
}