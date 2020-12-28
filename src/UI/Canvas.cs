using System;
using System.Collections.Generic;
using System.Linq;
using SFML.System;
using SFML.Graphics;

namespace BlackCoat.UI
{
    /// <summary>
    /// UI Container with an adjustable size, independently from its child components.
    /// </summary>
    /// <seealso cref="BlackCoat.UI.UIContainer" />
    public class Canvas : UIContainer, IDockable
    {
        // Variables #######################################################################
        private Vector2f _MinSize;
        private Vector2f _Size;
        private bool _DockX;
        private bool _DockY;


        // Properties ######################################################################
        /// <summary>
        /// Gets the inner size of this <see cref="Canvas" />.
        /// </summary>
        public override Vector2f InnerSize => _Size;

        /// <summary>
        /// Gets the relative size of this <see cref="Canvas" /> including position, origin and margin values.
        /// </summary>
        public override Vector2f RelativeSize => (DockedPosition - Origin) + (OuterBounds - Margin.Position());

        /// <summary>
        /// Gets or sets the minimum size.
        /// </summary>
        public Vector2f MinSize
        {
            get => _MinSize;
            set
            {
                _MinSize = value;
                Resize(_Size);
            }
        }

        /// <summary>
        /// Gets the minimum outer bounds of this <see cref="Canvas"/>.
        /// </summary>
        public Vector2f OuterBounds => Margin.Position() + new Vector2f(DockX ? MinSize.X : InnerSize.X, DockY ? MinSize.Y : InnerSize.Y) + Margin.Size();

        /// <summary>
        /// Gets the docked position.
        /// </summary>
        public Vector2f DockedPosition => new Vector2f(DockX ? 0 : Position.X, DockY ? 0 : Position.Y);

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Canvas"/> should automatically span across its parent X axis.
        /// </summary>
        public virtual bool DockX
        {
            get => _DockX;
            set
            {
                var update = _DockX != value;
                _DockX = value;
                if (update) InvokeSizeChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Canvas"/> should automatically span across its parent Y axis.
        /// </summary>
        public virtual bool DockY
        {
            get => _DockY;
            set
            {
                var update = _DockY != value;
                _DockY = value;
                if (update) InvokeSizeChanged();
            }
        }


        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="Canvas"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="size">Optional initial size of the <see cref="Canvas"/>.</param>
        /// <param name="components">Optional <see cref="UIComponent"/>s for functional construction.</param>
        public Canvas(Core core, Vector2f? size = null, params UIComponent[] components) :
                 this(core, size, components as IEnumerable<UIComponent>)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Canvas"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="size">Optional initial size of the <see cref="Canvas"/>.</param>
        /// <param name="components">Optional enumeration of <see cref="UIComponent"/>s for functional construction.</param>
        public Canvas(Core core, Vector2f? size = null, IEnumerable<UIComponent> components = null) : base(core, components)
        {
            _MinSize = new Vector2f(10, 10);
            if (size.HasValue) Resize(size.Value);
            else ResizeToFitContent();
        }


        // Methods #########################################################################
        /// <summary>
        /// Resizes the <see cref="Canvas"/>.
        /// </summary>
        /// <param name="size">The new size.</param>
        public void Resize(Vector2f size)
        {
            size = new Vector2f(Math.Max(_MinSize.X, size.X), Math.Max(_MinSize.Y, size.Y));
            if (_Size == size) return;
            _Size = size;
            _Background.Size = _Size;
            TextureRect = new IntRect(default, _Size.ToVector2i());
            InvokeSizeChanged();
        }

        /// <summary>
        /// Resizes the <see cref="Canvas"/> of to fit its contents.
        /// </summary>
        public void ResizeToFitContent()
        {
            var components = Components.Select(c => c.RelativeSize).ToArray();
            if (components.Length == 0) return;
            Resize(new Vector2f(DockX ? InnerSize.X : components.Max(c => c.X),
                                DockY ? InnerSize.Y : components.Max(c => c.Y)));
        }

        /// <summary>
        /// Updates the position and size of a docked component.
        /// </summary>
        /// <param name="c">The component to update.</param>
        protected override void UpdateDockedComponent(UIComponent c)
        {
            if (c is IDockable dockee && (dockee.DockX || dockee.DockY))
            {
                // Reset
                c.Rotation = 0;
                c.Origin = new Vector2f(0, 0);
                c.Scale  = new Vector2f(1, 1);

                // Dock Position
                c.Position = new Vector2f(dockee.DockX ? c.Margin.Left : c.Position.X,
                                          dockee.DockY ? c.Margin.Top  : c.Position.Y);
                // Dock Size
                dockee.Resize(new Vector2f(dockee.DockX ? InnerSize.X - (c.Margin.Left + c.Margin.Width)  : c.InnerSize.X,
                                           dockee.DockY ? InnerSize.Y - (c.Margin.Top  + c.Margin.Height) : c.InnerSize.Y));
            }
        }
    }
}