using System;
using System.Collections.Generic;
using System.Linq;
using SFML.System;

namespace BlackCoat.UI
{
    /// <summary>
    /// Places all child components evenly spaced along one axis inside itself based on a fixed offset.
    /// </summary>
    /// <seealso cref="BlackCoat.UI.UIContainer" />
    public class OffsetContainer : AutomatedCanvas
    {
        public float Offset { get => _Offset; set { _Offset = value; InvokeSizeChanged(); } }

        public override bool DockX { get => base.DockX && Orientation == Orientation.Vertical;   set => base.DockX = value && Orientation == Orientation.Vertical; }
        public override bool DockY { get => base.DockY && Orientation == Orientation.Horizontal; set => base.DockY = value && Orientation == Orientation.Horizontal; }


        public OffsetContainer(Core core, Orientation orientation, float offset = 0, params UIComponent[] components) : this(core, orientation, offset, components as IEnumerable<UIComponent>)
        { }
        public OffsetContainer(Core core, Orientation orientation, float offset = 0, IEnumerable<UIComponent> components = null) : base(core, orientation, null, components)
        {
            Offset = offset;
        }

        protected override void UpdateDockedComponent(UIComponent c)
        {
            if (c is IDockable dockee && (dockee.DockX || dockee.DockY))
            {
                // Dock Position
                c.Position = new Vector2f(dockee.DockX ? c.Margin.Left : c.Position.X,
                                          dockee.DockY ? c.Margin.Top : c.Position.Y);
                // Dock Size
                dockee.Resize(new Vector2f(dockee.DockX && Orientation == Orientation.Vertical   ? InnerSize.X - (c.Margin.Left + c.Margin.Width) : c.InnerSize.X,
                                           dockee.DockY && Orientation == Orientation.Horizontal ? InnerSize.Y - (c.Margin.Top + c.Margin.Height) : c.InnerSize.Y));
            }

            base.UpdateDockedComponent(c);
        }
    }
}