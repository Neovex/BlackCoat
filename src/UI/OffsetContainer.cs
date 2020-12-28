using System.Collections.Generic;
using SFML.System;

namespace BlackCoat.UI
{
    /// <summary>
    /// Places all child components evenly spaced along one axis inside itself based on a fixed offset.
    /// </summary>
    /// <seealso cref="BlackCoat.UI.UIContainer" />
    public class OffsetContainer : AutomatedCanvas
    {
        // Properties ######################################################################
        /// <summary>
        /// Gets or sets the offset in between each child <see cref="UIComponent"/>.
        /// </summary>
        public float Offset
        {
            get => _Offset;
            set
            {
                _Offset = value;
                InvokeSizeChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="OffsetContainer" /> should automatically span across its parent X axis.
        /// </summary>
        public override bool DockX
        {
            get => base.DockX && Orientation == Orientation.Vertical;
            set => base.DockX = value && Orientation == Orientation.Vertical;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="OffsetContainer" /> should automatically span across its parent Y axis.
        /// </summary>
        public override bool DockY
        {
            get => base.DockY && Orientation == Orientation.Horizontal;
            set => base.DockY = value && Orientation == Orientation.Horizontal;
        }


        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="OffsetContainer"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="orientation">The initial orientation.</param>
        /// <param name="offset">The initial offset in between each child <see cref="UIComponent"/>.</param>
        /// <param name="components">Optional <see cref="UIComponent"/>s for functional construction.</param>
        public OffsetContainer(Core core, Orientation orientation, float offset = 0, params UIComponent[] components) :
                          this(core, orientation, offset, components as IEnumerable<UIComponent>)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OffsetContainer"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="orientation">The initial orientation.</param>
        /// <param name="offset">The initial offset in between each child <see cref="UIComponent"/>.</param>
        /// <param name="components">Optional enumeration of <see cref="UIComponent"/>s for functional construction.</param>
        public OffsetContainer(Core core, Orientation orientation, float offset = 0, IEnumerable<UIComponent> components = null) :
                          base(core, orientation, null, components)
        {
            Offset = offset;
        }


        // Methods #########################################################################
        /// <summary>
        /// Updates the position and size of a docked component.
        /// </summary>
        /// <param name="c">The component to update.</param>
        protected override void UpdateDockedComponent(UIComponent c)
        {
            if (c is IDockable dockee && (dockee.DockX || dockee.DockY))
            {
                // Dock Position
                c.Position = new Vector2f(dockee.DockX ? c.Margin.Left : c.Position.X,
                                          dockee.DockY ? c.Margin.Top  : c.Position.Y);
                // Dock Size
                dockee.Resize(new Vector2f(dockee.DockX && Orientation == Orientation.Vertical   ? InnerSize.X - (c.Margin.Left + c.Margin.Width)  : c.InnerSize.X,
                                           dockee.DockY && Orientation == Orientation.Horizontal ? InnerSize.Y - (c.Margin.Top  + c.Margin.Height) : c.InnerSize.Y));
            }
            base.UpdateDockedComponent(c);
        }
    }
}