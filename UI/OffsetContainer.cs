using System;
using System.Collections.Generic;
using System.Linq;
using SFML.System;

namespace BlackCoat.UI
{
    /// <summary>
    /// Places all child components evenly spaced inside itself base on a fixed offset.
    /// </summary>
    /// <seealso cref="BlackCoat.UI.UIContainer" />
    public class OffsetContainer : DistributionContainer
    {
        public float Offset { get => _Offset; set { _Offset = value; InvokeSizeChanged(); } }

        public override bool DockX { get => base.DockX && !Horizontal; set => base.DockX = value && !Horizontal; }
        public override bool DockY { get => base.DockY && Horizontal; set => base.DockY = value && Horizontal; }


        public OffsetContainer(Core core, bool horizontal = true) : base(core, horizontal)
        {
        }

        protected override void InvokeSizeChanged()
        {
            base.InvokeSizeChanged();
            var components = Components.Select(c => c.RelativeSize);
            if (!Components.Any()) return;
            Resize(new Vector2f(DockX ? InnerSize.X : components.Max(c => c.X),
                                DockY ? InnerSize.Y : components.Max(c => c.Y)));
        }

        protected override void CalculateOffset() { } // Intentionally empty
    }
}