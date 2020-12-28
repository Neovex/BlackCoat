using System.Collections.Generic;
using SFML.System;

namespace BlackCoat.UI
{
    /// <summary>
    /// A <see cref="UIContainer"/> that automatically aligns itself and its contents to its parent <see cref="UIContainer"/>.
    /// </summary>
    /// <seealso cref="BlackCoat.UI.UIContainer" />
    public class AlignedContainer : UIContainer
    {
        // Variables #######################################################################
        private Alignment _Alignment;


        // Properties ######################################################################
        /// <summary>
        /// Gets or sets the alignment of this <see cref="AlignedContainer"/>.
        /// </summary>
        public Alignment Alignment
        {
            get => _Alignment;
            set
            {
                _Alignment = value;
                InvokeSizeChanged();
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
                if (Container != null)
                {
                    Container.SizeChanged -= HandleParentContainerSizeChanged;
                }
                base.Container = value;
                if (Container != null)
                {
                    Position = Align(value.InnerSize);
                    Container.SizeChanged += HandleParentContainerSizeChanged;
                }
            }
        }


        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="AlignedContainer"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="alignment">The desired alignment.</param>
        /// <param name="components">Optional <see cref="UIComponent"/>s for functional construction.</param>
        public AlignedContainer(Core core, Alignment alignment, params UIComponent[] components) :
                           this(core, alignment, components as IEnumerable<UIComponent>)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlignedContainer"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="alignment">The desired alignment.</param>
        /// <param name="components">An enumeration of <see cref="UIComponent"/>s for functional construction.</param>
        public AlignedContainer(Core core, Alignment alignment, IEnumerable<UIComponent> components) : base(core, components)
        {
            Alignment = alignment;
        }


        // Methods #########################################################################
        /// <summary>
        /// Handles when the parent <see cref="UIContainer"/> size changes.
        /// </summary>
        private void HandleParentContainerSizeChanged(UIComponent c)
        {
            if (Container != null) Position = Align(Container.InnerSize) + Margin.Position() - Margin.Size();
        }

        /// <summary>
        /// Invokes the size changed event.
        /// </summary>
        protected override void InvokeSizeChanged()
        {
            Origin = Align(InnerSize);
            base.InvokeSizeChanged();
        }

        /// <summary>
        /// Invokes the origin changed event.
        /// </summary>
        protected override void InvokeOriginChanged()
        {
            if (Container != null) Position = Align(Container.InnerSize);
            base.InvokeOriginChanged();
        }

        /// <summary>
        /// Calculates an offset to create an alignment based on a given size.
        /// </summary>
        /// <param name="size">The size of the container.</param>
        /// <returns>The required offset for the alignment effect.</returns>
        private Vector2f Align(Vector2f size)
        {
            var ret = new Vector2f();
            switch (Alignment)
            {
                case Alignment.Center:
                case Alignment.CenterTop:
                case Alignment.CenterBottom:
                    ret.X = size.X / 2;
                    break;

                case Alignment.TopRight:
                case Alignment.CenterRight:
                case Alignment.BottomRight:
                    ret.X = size.X;
                    break;
            }
            switch (Alignment)
            {
                case Alignment.Center:
                case Alignment.CenterRight:
                case Alignment.CenterLeft:
                    ret.Y = size.Y / 2;
                    break;

                case Alignment.BottomRight:
                case Alignment.CenterBottom:
                case Alignment.BottomLeft:
                    ret.Y = size.Y;
                    break;
            }
            return ret;
        }

        /// <summary>
        /// Releases unmanaged and managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (Container != null)
            {
                Container.SizeChanged -= HandleParentContainerSizeChanged;
            }
            base.Dispose(disposing);
        }
    }
}