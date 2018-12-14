using System;
using System.Collections.Generic;
using System.Linq;
using SFML.System;

namespace BlackCoat.UI
{
    /// <summary>
    /// Distributes all child components evenly spaced inside itself.
    /// </summary>
    /// <seealso cref="BlackCoat.UI.Canvas" />
    public class DistributionContainer : Canvas
    {
        private bool _Horizontal;
        protected float _Offset;
        private float _CurrentOffset;
        private Vector2f _DockingSize;
        private bool _UpdateLock;

        public Boolean Horizontal { get => _Horizontal; set { _Horizontal = value; InvokeSizeChanged(); } }

        public override bool DockX { get => base.DockX || Horizontal; set => base.DockX = value || Horizontal; }
        public override bool DockY { get => base.DockY || !Horizontal; set => base.DockY = value || !Horizontal; }



            
        public DistributionContainer(Core core, bool horizontal = true, Vector2f? size = null) : base(core, size)
        {
            Horizontal = horizontal;
        }

        protected override void InvokeSizeChanged()
        {
            if (_UpdateLock) return;
            _UpdateLock = true;

            // Reset Offset
            _CurrentOffset = 0;
            // Calculate Offset for docking
            CalculateOffset();

            // Calculate Size for docking
            var componentSizes = Components.Where(co => !(co is IDockable)).Select(co => co.RelativeSize).
                          Concat(Components.OfType<IDockable>().Select(dock => dock.MinRelativeSize)).ToArray();
            _DockingSize = componentSizes.Length == 0 ? default(Vector2f) :
                           new Vector2f(componentSizes.Max(v => v.X), componentSizes.Max(v => v.Y));

            // Base calls UpdateDockedComponent on all components
            base.InvokeSizeChanged();
            _UpdateLock = false;
        }

        protected override void UpdateDockedComponent(UIComponent c)
        {
            // Reset
            c.Rotation = 0;
            c.Origin = default(Vector2f);
            c.Scale = Create.Vector2f(1);

            if (Horizontal)
            {
                _CurrentOffset += c.Padding.Left;
                c.Position = new Vector2f(_CurrentOffset, c.Padding.Top);
                _CurrentOffset += c.InnerSize.X;
                _CurrentOffset += c.Padding.Width;
            }
            else
            {
                _CurrentOffset += c.Padding.Top;
                c.Position = new Vector2f(c.Padding.Left, _CurrentOffset);
                _CurrentOffset += c.InnerSize.Y;
                _CurrentOffset += c.Padding.Height;
            }
            _CurrentOffset += _Offset;

            base.UpdateDockedComponent(c);
        }

        protected virtual void CalculateOffset()
        {
            var components = Components.ToArray();
            if (components.Length < 2)
            {
                _Offset = 0;
            }
            else
            {
                var componentSize = new Vector2f(components.Sum(c => c.OuterSize.X), components.Sum(c => c.OuterSize.Y));
                if (Horizontal)
                    _Offset = (InnerSize.X - componentSize.X) / (components.Length - 1);
                else
                    _Offset = (InnerSize.Y - componentSize.Y) / (components.Length - 1);
            }
        }
    }
}