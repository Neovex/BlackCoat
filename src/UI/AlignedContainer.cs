using System.Collections.Generic;
using SFML.System;

namespace BlackCoat.UI
{
    public class AlignedContainer : UIContainer
    {
        private Alignment _Alignment;

        public Alignment Alignment { get => _Alignment; set { _Alignment = value; InvokeSizeChanged(); } }

        public override UIContainer Container
        {
            get => base.Container;
            set
            {
                if (Container != null)
                {
                    Container.SizeChanged -= ContainerSizeChanged;
                }
                base.Container = value;
                if (Container != null)
                {
                    Position = Align(value.InnerSize);
                    Container.SizeChanged += ContainerSizeChanged;
                }
            }
        }

        public AlignedContainer(Core core, Alignment alignment, params UIComponent[] components) : this(core, alignment, components as IEnumerable<UIComponent>)
        { }
        public AlignedContainer(Core core, Alignment alignment, IEnumerable<UIComponent> components) : base(core, components)
        {
            Alignment = alignment;
        }

        private void ContainerSizeChanged(UIComponent c)
        {
            if (Container != null) Position = Align(Container.InnerSize) + Margin.Position() - Margin.Size();
        }

        protected override void InvokeSizeChanged()
        {
            Origin = Align(InnerSize);
            base.InvokeSizeChanged();
        }
        protected override void InvokeOriginChanged()
        {
            if (Container != null) Position = Align(Container.InnerSize);
            base.InvokeOriginChanged();
        }

        private Vector2f Align(Vector2f size)
        {
            var ret = new Vector2f();
            switch (Alignment)
            {
                case Alignment.Center:
                case Alignment.CenterTop:
                case Alignment.CenterBottom:
                    ret.X = size.X / 2;
                    break;

                case Alignment.TopRight:
                case Alignment.CenterRight:
                case Alignment.BottomRight:
                    ret.X = size.X;
                    break;
            }
            switch (Alignment)
            {
                case Alignment.Center:
                case Alignment.CenterRight:
                case Alignment.CenterLeft:
                    ret.Y = size.Y / 2;
                    break;

                case Alignment.BottomRight:
                case Alignment.CenterBottom:
                case Alignment.BottomLeft:
                    ret.Y = size.Y;
                    break;
            }
            return ret;
        }

        protected override void Dispose(bool disposing)
        {
            if (Container != null)
            {
                Container.SizeChanged -= ContainerSizeChanged;
            }
            base.Dispose(disposing);
        }
    }
}