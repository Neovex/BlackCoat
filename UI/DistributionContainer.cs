using System;
using System.Collections.Generic;
using System.Linq;
using SFML.System;

namespace BlackCoat.UI
{
    /// <summary>
    /// Distributes all child components evenly spaced along one axis inside itself.
    /// </summary>
    /// <seealso cref="BlackCoat.UI.Canvas" />
    public class DistributionContainer : AutomatedCanvas
    {
        private Vector2f _DockingSize;

        public override bool DockX { get => base.DockX || Horizontal; set => base.DockX = value || Horizontal; }
        public override bool DockY { get => base.DockY || !Horizontal; set => base.DockY = value || !Horizontal; }

            
        public DistributionContainer(Core core, bool horizontal = true, Vector2f? size = null, params UIComponent[] components) : base(core, horizontal, size, components)
        {
        }

        protected override void InvokeSizeChanged()
        {
            if (Updating) return;

            // Calculate Offset for docking
            _Offset = CalculateOffset();

            // Calculate Size for docking
            _DockingSize = CalculateDockingSize();

            // Base calls UpdateDockedComponent on all components
            base.InvokeSizeChanged();
        }

        protected override void UpdateDockedComponent(UIComponent c)
        {
            if (c is IDockable dockee && (dockee.DockX || dockee.DockY))
            {
                // Dock Position
                c.Position = new Vector2f(dockee.DockX && !Horizontal ? c.Margin.Left : c.Position.X,
                                          dockee.DockY &&  Horizontal ? c.Margin.Top : c.Position.Y);

                // Dock Size
                Vector2f size;
                if (Horizontal)
                {
                    size.X = dockee.DockX ? dockee.MinSize.X + _DockingSize.X : c.InnerSize.X;
                    size.Y = dockee.DockY ? InnerSize.Y - (c.Margin.Top + c.Margin.Height) : c.InnerSize.Y;
                }
                else
                {
                    size.X = dockee.DockX ? InnerSize.X - (c.Margin.Left + c.Margin.Width) : c.InnerSize.X;
                    size.Y = dockee.DockY ? dockee.MinSize.Y + _DockingSize.Y : c.InnerSize.Y;
                }
                dockee.Resize(size);
            }

            base.UpdateDockedComponent(c);
        }

        protected virtual float CalculateOffset()
        {
            var components = Components.ToArray();
            if (components.Length < 2 || components.OfType<IDockable>().Any(d => (d.DockX && Horizontal) || (d.DockY && !Horizontal)))
            {
                return 0;
            }

            var componentSize = new Vector2f(components.Sum(c => c.OuterSize.X), components.Sum(c => c.OuterSize.Y));
            var r = (InnerSize - componentSize) / (components.Length - 1);
            return Horizontal ? r.X : r.Y;
        }

        protected virtual Vector2f CalculateDockingSize()
        {
            var dockeeCount = 0;
            var componentSizes = Components.Select(c =>
            {
                if (c is IDockable dockee)
                {
                    if ((Horizontal && dockee.DockX) || (!Horizontal && dockee.DockY)) dockeeCount++;
                    return new Vector2f(Horizontal && dockee.DockX ? dockee.OuterMinSize.X : c.OuterSize.X,
                                       !Horizontal && dockee.DockY ? dockee.OuterMinSize.Y : c.OuterSize.Y);
                }
                return c.OuterSize;
            }).ToArray();

            if (componentSizes.Length == 0 || dockeeCount == 0) return default(Vector2f);
            
            return (InnerSize - new Vector2f(componentSizes.Sum(v => v.X), componentSizes.Sum(v => v.Y))) / dockeeCount;
        }
    }
}