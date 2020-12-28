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
        // Variables #######################################################################
        private Vector2f _DockingSize;


        // Properties ######################################################################
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="DistributionContainer" /> should automatically span across its parent X axis.
        /// </summary>
        public override bool DockX
        {
            get => base.DockX || Orientation == Orientation.Horizontal;
            set => base.DockX = value || Orientation == Orientation.Horizontal;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="DistributionContainer" /> should automatically span across its parent Y axis.
        /// </summary>
        public override bool DockY
        {
            get => base.DockY || Orientation == Orientation.Vertical;
            set => base.DockY = value || Orientation == Orientation.Vertical;
        }


        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="DistributionContainer"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="orientation">The initial orientation.</param>
        /// <param name="size">Optional initial size of the <see cref="DistributionContainer" />.</param>
        /// <param name="components">Optional <see cref="UIComponent"/>s for functional construction.</param>
        public DistributionContainer(Core core, Orientation orientation, Vector2f? size = null, params UIComponent[] components) :
                                base(core, orientation, size, components)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributionContainer"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="orientation">The initial orientation.</param>
        /// <param name="size">Optional initial size of the <see cref="DistributionContainer" />.</param>
        /// <param name="components">Optional enumeration of <see cref="UIComponent" />s for functional construction.</param>
        public DistributionContainer(Core core, Orientation orientation, Vector2f? size = null, IEnumerable<UIComponent> components = null) :
                                base(core, orientation, size, components)
        { }


        // Methods #########################################################################
        /// <summary>
        /// Invokes the size changed event.
        /// </summary>
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

        /// <summary>
        /// Updates the position and size of a docked component.
        /// </summary>
        /// <param name="c">The component to update.</param>
        protected override void UpdateDockedComponent(UIComponent c)
        {
            if (c is IDockable dockee && (dockee.DockX || dockee.DockY))
            {
                // Dock Position
                c.Position = new Vector2f(dockee.DockX && Orientation == Orientation.Vertical   ? c.Margin.Left : c.Position.X,
                                          dockee.DockY && Orientation == Orientation.Horizontal ? c.Margin.Top : c.Position.Y);

                // Dock Size
                Vector2f size;
                if (Orientation == Orientation.Horizontal)
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

        /// <summary>
        /// Calculates the offset automatically for when the <see cref="DistributionContainer"/> is docked.
        /// </summary>
        /// <returns>Optimal offset for space distribution</returns>
        protected virtual float CalculateOffset()
        {
            var components = Components.ToArray();
            if (components.Length < 2 || components.OfType<IDockable>().Any(d => (d.DockX && Orientation == Orientation.Horizontal) || 
                                                                                 (d.DockY && Orientation == Orientation.Vertical)))
            {
                return 0;
            }

            var componentSize = new Vector2f(components.Sum(c => c.OuterSize.X), components.Sum(c => c.OuterSize.Y));
            var r = (InnerSize - componentSize) / (components.Length - 1);
            return Orientation == Orientation.Horizontal ? r.X : r.Y;
        }

        /// <summary>
        /// Calculates the size of the <see cref="DistributionContainer"/> for when its docked.
        /// </summary>
        /// <returns>Required space</returns>
        protected virtual Vector2f CalculateDockingSize()
        {
            var dockeeCount = 0;
            var componentSizes = Components.Select(c =>
            {
                if (c is IDockable dockee)
                {
                    if ((Orientation == Orientation.Horizontal && dockee.DockX) || (Orientation == Orientation.Vertical && dockee.DockY)) dockeeCount++;
                    return new Vector2f(Orientation == Orientation.Horizontal && dockee.DockX ? dockee.OuterBounds.X : c.OuterSize.X,
                                        Orientation == Orientation.Vertical   && dockee.DockY ? dockee.OuterBounds.Y : c.OuterSize.Y);
                }
                return c.OuterSize;
            }).ToArray();

            if (componentSizes.Length == 0 || dockeeCount == 0) return default;
            
            return (InnerSize - new Vector2f(componentSizes.Sum(v => v.X), componentSizes.Sum(v => v.Y))) / dockeeCount;
        }
    }
}