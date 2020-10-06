using System;
using System.Collections.Generic;
using System.Linq;
using SFML.System;

namespace BlackCoat.UI
{
    /// <summary>
    /// Abstract base class for UI components with automated child distribution.
    /// </summary>
    /// <seealso cref="BlackCoat.UI.Canvas" />
    public abstract class AutomatedCanvas : Canvas
    {
        private Orientation _Orientation;
        protected float _Offset;
        private float _CurrentOffset;

        protected bool Updating { get; private set; }
        public Orientation Orientation { get => _Orientation; set { _Orientation = value; InvokeSizeChanged(); } }


        public AutomatedCanvas(Core core, Orientation orientation, Vector2f? size = null, IEnumerable<UIComponent> components = null) : base(core, size, components)
        {
            Orientation = orientation;
        }

        protected override void InvokeSizeChanged()
        {
            if (Updating) return;
            Updating = true;

            // Reset Offset
            _CurrentOffset = 0;

            // Calls UpdateDockedComponents
            base.InvokeSizeChanged();

            Updating = false;
        }

        protected override void UpdateDockedComponents()
        {
            base.UpdateDockedComponents();
            ResizeToFitContent();
        }

        protected override void UpdateDockedComponent(UIComponent c)
        {
            // Reset
            c.Rotation = 0;
            c.Origin = new Vector2f(0, 0);
            c.Scale = new Vector2f(1, 1);

            // Position
            if (Orientation == Orientation.Horizontal)
            {
                _CurrentOffset += c.Margin.Left;
                c.Position = new Vector2f(_CurrentOffset, c.Margin.Top);
                _CurrentOffset += c.InnerSize.X;
                _CurrentOffset += c.Margin.Width;
            }
            else
            {
                _CurrentOffset += c.Margin.Top;
                c.Position = new Vector2f(c.Margin.Left, _CurrentOffset);
                _CurrentOffset += c.InnerSize.Y;
                _CurrentOffset += c.Margin.Height;
            }
            _CurrentOffset += _Offset;
            // base intentionally not called - automated canvas does not support docking (yet)
        }
    }
}