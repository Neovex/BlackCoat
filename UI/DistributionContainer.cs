using System;
using System.Linq;
using SFML.System;
using BlackCoat.Collision;

namespace BlackCoat.UI
{
    /// <summary>
    /// Distributes all child components evenly spaced inside itself.
    /// </summary>
    /// <seealso cref="BlackCoat.UI.UICanvas" />
    public class DistributionContainer : UICanvas
    {
        private bool _Horizontal;
        private bool _UpdateLock;

        public Boolean Horizontal { get => _Horizontal; set { _Horizontal = value; UpdatePositions(); } }

        public override Vector2f InnerSize => _Size;
        public override Vector2f OuterSize
        {
            get
            {
                var realInnerSize = ComponentSize;
                return new Vector2f(DockX ? realInnerSize.X : InnerSize.X, DockY ? realInnerSize.Y : InnerSize.Y)
                     + new Vector2f(Padding.Left + Padding.Width, Padding.Top + Padding.Height);
            }
        }
        public Vector2f ComponentSize
        {
            get
            {
                var components = Components.ToArray();
                return new Vector2f(components.Sum(c => c.OuterSize.X), components.Sum(c => c.OuterSize.Y));
            }
        }
        public new Vector2f Origin
        {
            get => base.Origin;
            set { } // intended
        }



        public DistributionContainer(Core core, bool horizontal, Vector2f? size = null) : base(core, size)
        {
            Horizontal = horizontal;
        }

        protected override void InvokeSizeChanged()
        {
            UpdatePositions();
            base.InvokeSizeChanged();
        }

        protected override void HandleChildComponentModified(UIComponent c)
        {
            if (_UpdateLock) return;
            var pos = c.Position;
            _UpdateLock = true;
            base.HandleChildComponentModified(c);
            _UpdateLock = false;
            if (c.Position == pos) UpdatePositions();
        }

        private void UpdatePositions()
        {
            var components = Components.ToArray();
            if (components.Length < 2) return;

            var componentSize = ComponentSize;
            float offset;
            if (Horizontal)
                offset = (InnerSize.X - componentSize.X) / (components.Length - 1);
            else
                offset = (InnerSize.Y - componentSize.Y) / (components.Length - 1);

            float pos = 0;
            foreach (var component in components)
            {
                if (Horizontal)
                {
                    pos += component.Padding.Left;
                    component.Position = new Vector2f(pos, component.Position.Y);
                    pos += component.InnerSize.X;
                    pos += component.Padding.Width;
                }
                else
                {
                    pos += component.Padding.Top;
                    component.Position = new Vector2f(component.Position.X, pos);
                    pos += component.InnerSize.Y;
                    pos += component.Padding.Height;
                }
                pos += offset;
            }
        }

    }
}