using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackCoat.Collision;
using SFML.System;

namespace BlackCoat.UI
{
    public class AnchoredContainer : UIContainer
    {
        private Anchor _Anchor;
        private bool _UpdateLock;
        private Vector2f _LastSize;
        private Vector2f _LastContainerSize;

        public Anchor Anchor { get => _Anchor; set { _Anchor = value; UpdatePosition(); } }

        public override UIContainer Container
        {
            get => base.Container;
            set
            {
                if (base.Container != null) base.Container.SizeChanged -= UpdatePosition;
                base.Container = value;
                if (base.Container != null)
                {
                    base.Container.SizeChanged += UpdatePosition;
                    _LastContainerSize = base.Container.InnerSize;
                }
            }
        }

        public override Vector2f Position { get => default(Vector2f); set => base.Position = value; }
        public Vector2f RealPosition => base.Position;


        public AnchoredContainer(Core core, Anchor anchor) : base(core)
        {
            Anchor = anchor;
        }

        protected override void InvokeSizeChanged()
        {
            _UpdateLock = true;
            base.InvokeSizeChanged();
            _UpdateLock = false;
        }
        protected override void InvokePaddingChanged()
        {
            _UpdateLock = true;
            base.InvokePaddingChanged();
            _UpdateLock = false;
        }
        protected override void InvokePositionChanged()
        {
            _UpdateLock = true;
            base.InvokePositionChanged();
            _UpdateLock = false;
        }
        protected override void InvokeOriginChanged()
        {
            _UpdateLock = true;
            base.InvokeOriginChanged();
            _UpdateLock = false;
        }

        private void UpdatePosition(UIComponent c = null)
        {
            if (Container == null) return;

            var pos = new Vector2f();
            var size = InnerSize;
            var containerSize = Container.InnerSize;
            switch (Anchor)
            {
                case Anchor.Center:
                    Origin = size / 2;
                    Position = containerSize / 2;
                    return;

                case Anchor.TopRight:
                    pos.X = (containerSize.X - _LastContainerSize.X) + (size.X - _LastSize.X);
                    break;
                case Anchor.BottomLeft:
                    pos.Y = (containerSize.Y - _LastContainerSize.Y) + (size.Y - _LastSize.Y);
                    break;
                case Anchor.BottomRight:
                    pos.X = (containerSize.X - _LastContainerSize.X) + (size.X - _LastSize.X);
                    pos.Y = (containerSize.Y - _LastContainerSize.Y) + (size.Y - _LastSize.Y);
                    break;
            }
            _LastSize = size;
            _LastContainerSize = containerSize;
            if (pos.X < 0 || pos.Y < 0 || !_UpdateLock)
            {
                Position = new Vector2f(Math.Max(0, RealPosition.X + pos.X), Math.Max(0, RealPosition.Y + pos.Y));
            }
        }
    }
}