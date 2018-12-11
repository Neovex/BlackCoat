using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlackCoat.Collision;
using SFML.System;

namespace BlackCoat.UI
{
    /// <summary>
    /// Places all child components evenly spaced inside itself base on a fixed offset.
    /// </summary>
    /// <seealso cref="BlackCoat.UI.UIContainer" />
    public class OffsetContainer : UIContainer
    {
        private bool _Horizontal;
        private float _Offset;

        public Boolean Horizontal { get => _Horizontal; set { _Horizontal = value; UpdatePositions(); } }
        public float Offset { get => _Offset; set { _Offset = value; UpdatePositions(); } }


        public OffsetContainer(Core core, bool horizontal = true) : base(core)
        {
            Horizontal = horizontal;
        }

        protected override void InvokeSizeChanged()
        {
            base.InvokeSizeChanged();
            UpdatePositions();
        }

        private void UpdatePositions()
        {
            float pos = 0;
            foreach (var component in Components)
            {
                if (_Horizontal)
                {
                    pos += component.Padding.Left;
                    component.Position = new Vector2f(pos, component.Padding.Top);
                    pos += component.InnerSize.X;
                    pos += component.Padding.Width;
                }
                else
                {
                    pos += component.Padding.Top;
                    component.Position = new Vector2f(component.Padding.Left, pos);
                    pos += component.InnerSize.Y;
                    pos += component.Padding.Height;
                }
                pos += _Offset;
            }
        }
    }
}