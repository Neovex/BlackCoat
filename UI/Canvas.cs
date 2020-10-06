using System;
using System.Collections.Generic;
using System.Linq;
using SFML.System;
using SFML.Graphics;

namespace BlackCoat.UI
{
    /// <summary>
    /// UI Container with an adjustable size which is independent from its child components.
    /// </summary>
    /// <seealso cref="BlackCoat.UI.UIContainer" />
    public class Canvas : UIContainer, IDockable
    {
        private Vector2f _MinSize;
        private Vector2f _Size;
        private bool _DockX;
        private bool _DockY;


        public override Vector2f InnerSize => _Size;
        public override Vector2f RelativeSize => (DockedPosition - Origin) + (OuterMinSize - Margin.Position());

        public Vector2f MinSize { get => _MinSize; set { _MinSize = value; Resize(_Size); } }
        public Vector2f OuterMinSize => Margin.Position() + new Vector2f(DockX ? MinSize.X : InnerSize.X, DockY ? MinSize.Y : InnerSize.Y) + Margin.Size();
        public Vector2f DockedPosition => new Vector2f(DockX ? 0 : Position.X, DockY ? 0 : Position.Y);

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


        public Canvas(Core core, Vector2f? size = null, params UIComponent[] components) : this(core, size, components as IEnumerable<UIComponent>)
        { }
        public Canvas(Core core, Vector2f? size = null, IEnumerable<UIComponent> components = null) : base(core, components)
        {
            _MinSize = new Vector2f(10, 10);
            if (size.HasValue) Resize(size.Value);
        }


        public void Resize(Vector2f size)
        {
            size = new Vector2f(Math.Max(_MinSize.X, size.X), Math.Max(_MinSize.Y, size.Y));
            if (_Size == size) return;
            _Size = size;
            _Background.Size = _Size;
            TextureRect = new IntRect(new Vector2i(0, 0), _Size.ToVector2i());
            InvokeSizeChanged();
        }

        public void ResizeToFitContent()
        {
            var components = Components.Select(c => c.RelativeSize).ToArray();
            if (components.Length == 0) return;
            Resize(new Vector2f(DockX ? InnerSize.X : components.Max(c => c.X),
                                DockY ? InnerSize.Y : components.Max(c => c.Y)));
        }

        protected override void UpdateDockedComponent(UIComponent c)
        {
            if (c is IDockable dockee && (dockee.DockX || dockee.DockY))
            {
                // Reset
                c.Rotation = 0;
                c.Origin = new Vector2f(0, 0);
                c.Scale = new Vector2f(1, 1);

                // Dock Position
                c.Position = new Vector2f(dockee.DockX ? c.Margin.Left : c.Position.X,
                                          dockee.DockY ? c.Margin.Top : c.Position.Y);
                // Dock Size
                dockee.Resize(new Vector2f(dockee.DockX ? InnerSize.X - (c.Margin.Left + c.Margin.Width) : c.InnerSize.X,
                                           dockee.DockY ? InnerSize.Y - (c.Margin.Top + c.Margin.Height) : c.InnerSize.Y));
            }
        }
    }
}