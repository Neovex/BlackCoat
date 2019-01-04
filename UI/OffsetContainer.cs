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

        public override bool DockX { get => base.DockX && !Horizontal; set => base.DockX = value && !Horizontal; }
        public override bool DockY { get => base.DockY && Horizontal; set => base.DockY = value && Horizontal; }


        public OffsetContainer(Core core, bool horizontal = true) : base(core, horizontal)
        {
        }

        protected override void UpdateDockedComponent(UIComponent c)
        {
            if (c is IDockable dockee && (dockee.DockX || dockee.DockY))
            {
                // Dock Position
                c.Position = new Vector2f(dockee.DockX ? c.Margin.Left : c.Position.X,
                                          dockee.DockY ? c.Margin.Top : c.Position.Y);
                // Dock Size
                dockee.Resize(new Vector2f(dockee.DockX && !Horizontal ? InnerSize.X - (c.Margin.Left + c.Margin.Width) : c.InnerSize.X,
                                           dockee.DockY && Horizontal  ? InnerSize.Y - (c.Margin.Top + c.Margin.Height) : c.InnerSize.Y));
            }

            base.UpdateDockedComponent(c);
        }
    }
}