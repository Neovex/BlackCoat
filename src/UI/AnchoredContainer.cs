using System;
using System.Collections.Generic;
using SFML.System;

namespace BlackCoat.UI
{
    /// <summary>
    /// A <see cref="UIContainer"/> that automatically positions itself within its parent <see cref="UIContainer"/>.
    /// </summary>
    /// <seealso cref="BlackCoat.UI.UIContainer" />
    public class AnchoredContainer : UIContainer
    {
        // Variables #######################################################################
        private Anchor _Anchor;
        private bool _UpdateLock;
        private Vector2f _LastContainerSize;


        // Properties ######################################################################
        /// <summary>
        /// Gets or sets the anchor. This defines how the container will be positioned inside its parent.
        /// </summary>
        public Anchor Anchor
        {
            get => _Anchor;
            set
            {
                _Anchor = value;
                UpdatePosition();
            }
        }

        /// <summary>
        /// Gets or sets the parent <see cref="UIContainer" />.
        /// </summary>
        public override UIContainer Container
        {
            get => base.Container;
            set
            {
                if (base.Container != null) base.Container.SizeChanged -= UpdatePosition;
                base.Container = value;
                if (base.Container != null)
                {
                    base.Container.SizeChanged += UpdatePosition;
                    _LastContainerSize = base.Container.InnerSize;
                }
            }
        }


        /// <summary>
        /// Overridden, will always return default(Vector2f). Use <see cref="RealPosition"/> instead.
        /// </summary>
        public override Vector2f Position
        {
            get => default;
            set => base.Position = value;
        }

        /// <summary>
        /// Gets the current position of this <see cref="AnchoredContainer"/>.
        /// </summary>
        public Vector2f RealPosition => base.Position;


        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="AnchoredContainer"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="anchor">The desired anchor option.</param>
        /// <param name="components">Optional <see cref="UIComponent"/>s for functional construction.</param>
        public AnchoredContainer(Core core, Anchor anchor, params UIComponent[] components) :
                            this(core, anchor, components as IEnumerable<UIComponent>)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnchoredContainer"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="anchor">The desired anchor option.</param>
        /// <param name="components">An enumeration of <see cref="UIComponent"/>s for functional construction.</param>
        public AnchoredContainer(Core core, Anchor anchor, IEnumerable<UIComponent> components) : base(core, components)
        {
            Anchor = anchor;
        }


        // Methods #########################################################################
        /// <summary>
        /// Invokes the size changed event.
        /// </summary>
        protected override void InvokeSizeChanged()
        {
            _UpdateLock = true;
            base.InvokeSizeChanged();
            _UpdateLock = false;
        }

        /// <summary>
        /// Invokes the margin changed event.
        /// </summary>
        protected override void InvokeMarginChanged()
        {
            _UpdateLock = true;
            base.InvokeMarginChanged();
            _UpdateLock = false;
        }

        /// <summary>
        /// Invokes the position changed event.
        /// </summary>
        protected override void InvokePositionChanged()
        {
            _UpdateLock = true;
            base.InvokePositionChanged();
            _UpdateLock = false;
        }

        /// <summary>
        /// Invokes the origin changed event.
        /// </summary>
        protected override void InvokeOriginChanged()
        {
            _UpdateLock = true;
            base.InvokeOriginChanged();
            _UpdateLock = false;
        }

        /// <summary>
        /// Updates the position in relation to its parent.
        /// </summary>
        private void UpdatePosition(UIComponent c = null)
        {
            if (Container == null) return;
            // Calculate position offset
            var pos = new Vector2f();
            var containerSize = Container.InnerSize;
            switch (Anchor)
            {
                case Anchor.TopRight:
                    pos.X = containerSize.X - _LastContainerSize.X;
                    break;
                case Anchor.BottomLeft:
                    pos.Y = containerSize.Y - _LastContainerSize.Y;
                    break;
                case Anchor.BottomRight:
                    pos.X = containerSize.X - _LastContainerSize.X;
                    pos.Y = containerSize.Y - _LastContainerSize.Y;
                    break;
            }
            _LastContainerSize = containerSize;
            // Apply offset to stay anchored
            if (pos.X < 0 || pos.Y < 0 || !_UpdateLock)
            {
                Position = new Vector2f(Math.Max(0, RealPosition.X + pos.X), Math.Max(0, RealPosition.Y + pos.Y));
            }
        }
    }
}