using System.Collections.Generic;
using SFML.System;

namespace BlackCoat.UI
{
    /// <summary>
    /// Abstract base class for UI components with automated child distribution.
    /// </summary>
    /// <seealso cref="BlackCoat.UI.Canvas" />
    public abstract class AutomatedCanvas : Canvas
    {
        // Variables #######################################################################
        private Orientation _Orientation;
        protected float _Offset;
        private float _CurrentOffset;


        // Properties ######################################################################
        /// <summary>
        /// Gets a value indicating whether this <see cref="AutomatedCanvas"/> is currently updating its child components positions.
        /// </summary>
        protected bool Updating { get; private set; }

        /// <summary>
        /// Gets or sets the orientation for the automatic child component positioning.
        /// </summary>
        public Orientation Orientation
        {
            get => _Orientation;
            set
            {
                _Orientation = value;
                InvokeSizeChanged();
            }
        }


        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="AutomatedCanvas"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="orientation">The initial orientation.</param>
        /// <param name="size">Optional initial size of the <see cref="AutomatedCanvas"/>.</param>
        /// <param name="components">Optional enumeration of <see cref="UIComponent"/>s for functional construction.</param>
        public AutomatedCanvas(Core core, Orientation orientation, Vector2f? size = null, IEnumerable<UIComponent> components = null) :
                          base(core, size, components)
        {
            Orientation = orientation;
        }


        // Methods #########################################################################
        /// <summary>
        /// Invokes the size changed event.
        /// </summary>
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

        /// <summary>
        /// Updates the position and size of all docked components.
        /// </summary>
        protected override void UpdateDockedComponents()
        {
            base.UpdateDockedComponents();
            ResizeToFitContent();
        }

        /// <summary>
        /// Updates the position and size of a docked component.
        /// </summary>
        /// <param name="c">The component to update.</param>
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

            // base intentionally not called - automated canvas does not support docking
        }
    }
}