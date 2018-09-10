using System;
using System.Collections.Generic;
using System.Linq;
using SFML.System;

namespace BlackCoat.UI
{
    /// <summary>
    /// Basic UI container. Its size is defined by the size, padding and position of its child components.
    /// </summary>
    /// <seealso cref="BlackCoat.UI.UIComponent" />
    public class UIContainer : UIComponent
    {
        public event Action<UIComponent> ComponentAdded = c => { };
        public event Action<UIComponent> ComponentRemoved = c => { };


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
                var components = Components.ToArray();
                if (components.Length == 0) return default(Vector2f);
                return new Vector2f(components.Max(c => c.Position.X - c.Origin.X - c.Padding.Left + c.OuterSize.X),
                                    components.Max(c => c.Position.Y - c.Origin.Y - c.Padding.Top + c.OuterSize.Y));
            }
        }

        public IEnumerable<UIComponent> Components => _Entities.OfType<UIComponent>();
        public IEnumerable<UIComponent> ComponentsFlattened => Components.SelectMany(c => new[] { c }.Concat(c is UIContainer cont ? cont.ComponentsFlattened : Enumerable.Empty<UIComponent>()));

        public IEnumerable<UIComponent> Init
        {
            set
            {
                foreach (var component in Components) Remove(component);
                if (value == null) return;
                foreach (var component in value) Add(component);
            }
        }


        public UIContainer(Core core) : base(core)
        {
        }


        public virtual void Add(UIComponent component)
        {
            if (component.Container != null) component.Container.Remove(component);
            Add(component as IEntity);
            component.Container = this;
            component.PositionChanged += HandleChildComponentModified;
            component.OriginChanged += HandleChildComponentModified;
            component.SizeChanged += HandleChildComponentModified;
            component.PaddingChanged += HandleChildComponentModified;
            InvokeComponentAdded(component);
            InvokeSizeChanged();
        }

        public virtual void Remove(UIComponent component)
        {
            component.Container = null;
            component.PositionChanged -= HandleChildComponentModified;
            component.OriginChanged -= HandleChildComponentModified;
            component.SizeChanged -= HandleChildComponentModified;
            component.PaddingChanged -= HandleChildComponentModified;
            Remove(component as IEntity);
            InvokeComponentRemoved(component);
            InvokeSizeChanged();
        }

        public override bool GiveFocus() => Components.Any(c => c.GiveFocus());

        protected virtual void HandleChildComponentModified(UIComponent c) => InvokeSizeChanged();

        protected virtual void InvokeComponentAdded(UIComponent component) => ComponentAdded.Invoke(component);
        protected virtual void InvokeComponentRemoved(UIComponent component) => ComponentRemoved.Invoke(component);
    }
}