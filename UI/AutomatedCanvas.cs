﻿using System;
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
        private bool _Horizontal;
        protected float _Offset;
        private float _CurrentOffset;

        protected bool Updating { get; private set; }
        public Boolean Horizontal { get => _Horizontal; set { _Horizontal = value; InvokeSizeChanged(); } }


        public AutomatedCanvas(Core core, bool horizontal = true, Vector2f? size = null) : base(core, size)
        {
            Horizontal = horizontal;
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
            c.Origin = default(Vector2f);
            c.Scale = Create.Vector2f(1);

            if (Horizontal)
            {
                _CurrentOffset += c.Margin.Left;
                c.Position = new Vector2f(_CurrentOffset, c.Position.Y);
                _CurrentOffset += c.InnerSize.X;
                _CurrentOffset += c.Margin.Width;
            }
            else
            {
                _CurrentOffset += c.Margin.Top;
                c.Position = new Vector2f(c.Position.X, _CurrentOffset);
                _CurrentOffset += c.InnerSize.Y;
                _CurrentOffset += c.Margin.Height;
            }
            _CurrentOffset += _Offset;
        }
    }
}