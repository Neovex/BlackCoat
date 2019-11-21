using System;
using System.Collections.Generic;
using System.Linq;
using SFML.System;

namespace BlackCoat.UI
{
    /// <summary>
    /// Basic UI container. Its size is defined by the size, margin and position of its child components.
    /// </summary>
    /// <seealso cref="BlackCoat.UI.UIComponent" />
    public class UIContainer : UIComponent
    {
        public event Action<UIComponent> ComponentAdded = c => { };
        public event Action<UIComponent> ComponentRemoved = c => { };


        private bool _UpdateLock;


        public override UIInput Input
        {
            get => base.Input;
            set
            {
                base.Input = value;
                foreach (var component in Components) component.Input = value;
            }
        }

        public override Vector2f InnerSize
        {
            get
            {
                var components = Components.Select(c => c.RelativeSize).ToArray();
                if (components.Length == 0) return default(Vector2f);
                return new Vector2f(components.Max(v => v.X), components.Max(v => v.Y));
            }
        }

        public IEnumerable<UIComponent> Init
        {
            set
            {
                foreach (var component in Components) Remove(component);
                if (value == null) return;
                foreach (var component in value) Add(component);
            }
        }

        public IEnumerable<UIComponent> Components => _Entities?.OfType<UIComponent>() ?? Enumerable.Empty<UIComponent>();
        public IEnumerable<UIComponent> ComponentsFlattened => Components.SelectMany(c => new[] { c }.Concat(c is UIContainer cont ? cont.ComponentsFlattened : Enumerable.Empty<UIComponent>()));


        public UIContainer(Core core, params UIComponent[] components) : base(core)
        {
            _UpdateLock = true;
            foreach (var component in components) Add(component);
            _UpdateLock = false;
        }


        public virtual void Add(UIComponent component)
        {
            if (component.Container != null) component.Container.Remove(component);
            Add(component as IEntity);
            component.Container = this;
            component.PositionChanged += HandleChildComponentModified;
            component.OriginChanged += HandleChildComponentModified;
            component.SizeChanged += HandleChildComponentModified;
            component.MarginChanged += HandleChildComponentModified;
            InvokeComponentAdded(component);
            InvokeSizeChanged();
        }

        public virtual void Remove(UIComponent component)
        {
            component.Container = null;
            component.PositionChanged -= HandleChildComponentModified;
            component.OriginChanged -= HandleChildComponentModified;
            component.SizeChanged -= HandleChildComponentModified;
            component.MarginChanged -= HandleChildComponentModified;
            Remove(component as IEntity);
            InvokeComponentRemoved(component);
            InvokeSizeChanged();
        }

        public override bool GiveFocus() => base.GiveFocus() || Components.Any(c => c.GiveFocus());

        protected virtual void HandleChildComponentModified(UIComponent c) => InvokeSizeChanged();

        protected override void InvokeSizeChanged()
        {
            if (_UpdateLock) return;
            _UpdateLock = true;
            UpdateDockedComponents();
            _UpdateLock = false;
            base.InvokeSizeChanged();
        }

        protected virtual void UpdateDockedComponents()
        {
            foreach (var c in Components)
            {
                UpdateDockedComponent(c);
            }
        }

        protected virtual void UpdateDockedComponent(UIComponent c)
        {
            if (c is IDockable dockee && (dockee.DockX || dockee.DockY))
            {
                // Reset
                c.Rotation = 0;
                c.Origin = default(Vector2f);
                c.Scale = 1.ToVector2f();

                // Dock Position
                c.Position = new Vector2f(dockee.DockX ? c.Margin.Left : c.Position.X,
                                          dockee.DockY ? c.Margin.Top : c.Position.Y);
                // Dock Size
                var components = Components.Select(co => co.RelativeSize).ToArray();

                var size = components.Length == 0 ? default(Vector2f) : 
                           new Vector2f(components.Max(v => v.X), components.Max(v => v.Y));

                dockee.Resize(new Vector2f(dockee.DockX ? size.X - (c.Margin.Left + c.Margin.Width) : c.InnerSize.X,
                                           dockee.DockY ? size.Y - (c.Margin.Top + c.Margin.Height) : c.InnerSize.Y));
            }
        }

        protected virtual void InvokeComponentAdded(UIComponent component) => ComponentAdded.Invoke(component);
        protected virtual void InvokeComponentRemoved(UIComponent component) => ComponentRemoved.Invoke(component);
    }
}