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
    }
}