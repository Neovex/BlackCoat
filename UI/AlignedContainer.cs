﻿using SFML.System;

namespace BlackCoat.UI
{
    public class AlignedContainer : UIContainer
    {
        private Alignment _Alignment;

        public Alignment Alignment { get => _Alignment; set { _Alignment = value; Origin = Align(InnerSize); } }

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

        public AlignedContainer(Core core, Alignment alignment) : base(core)
        {
            Alignment = alignment;
        }

        private void ContainerSizeChanged(UIComponent c)
        {
            if (Container != null) Position = Align(Container.InnerSize);
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

        protected override void Destroy(bool disposing)
        {
            if (Container != null)
            {
                Container.SizeChanged -= ContainerSizeChanged;
            }
            base.Destroy(disposing);
        }
    }
}