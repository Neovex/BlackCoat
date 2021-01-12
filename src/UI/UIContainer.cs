using System;
using System.Collections.Generic;
using System.Linq;
using SFML.System;
using BlackCoat.Entities;

namespace BlackCoat.UI
{
    /// <summary>
    /// Basic UI container. Its size is defined by the size, margins and positions of its child components.
    /// </summary>
    /// <seealso cref="BlackCoat.UI.UIComponent" />
    public class UIContainer : UIComponent
    {
        // Events ##########################################################################
        /// <summary>
        /// Occurs when a <see cref="UIComponent"/> is added to this <see cref="UIContainer"/>.
        /// </summary>
        public event Action<UIComponent> ComponentAdded = c => { };

        /// <summary>
        /// Occurs when a <see cref="UIComponent"/> is removed from this <see cref="UIContainer"/>.
        /// </summary>
        public event Action<UIComponent> ComponentRemoved = c => { };


        // Variables #######################################################################
        private bool _UpdateLock;


        // Properties ######################################################################
        /// <summary>
        /// Current Input event source.
        /// </summary>
        public override UIInput Input
        {
            get => base.Input;
            set
            {
                base.Input = value;
                foreach (var component in Components) component.Input = value;
            }
        }

        /// <summary>
        /// Gets the inner size of this <see cref="UIComponent" />.
        /// </summary>
        public override Vector2f InnerSize
        {
            get
            {
                var components = Components.Select(c => c.RelativeSize).ToArray();
                if (components.Length == 0) return default(Vector2f);
                return new Vector2f(components.Max(v => v.X), components.Max(v => v.Y));
            }
        }

        /// <summary>
        /// Gets an enumeration of all <see cref="UIComponent"/>s that are directly contained in this <see cref="UIContainer"/>.
        /// </summary>
        public IEnumerable<UIComponent> Components => _Entities?.OfType<UIComponent>() ?? Enumerable.Empty<UIComponent>();

        /// <summary>
        /// Gets an enumeration of all <see cref="UIComponent"/>s in the Scene Graph beneath this <see cref="UIContainer"/>.
        /// </summary>
        public IEnumerable<UIComponent> ComponentsFlattened => 
            Components.SelectMany(c => new[] { c }.Concat(c is UIContainer cont ? cont.ComponentsFlattened : Enumerable.Empty<UIComponent>()));

        /// <summary>
        /// Helper property to simplify adding of <see cref="UIComponent"/>s during initialization.
        /// </summary>
        public IEnumerable<UIComponent> Init { set => Add(value); }


        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="UIContainer"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="components">Optional <see cref="UIComponent"/>s for functional construction.</param>
        public UIContainer(Core core, params UIComponent[] components) :
                      this(core, components as IEnumerable<UIComponent>)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UIContainer"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="components">An enumeration of <see cref="UIComponent"/>s for functional construction.</param>
        public UIContainer(Core core, IEnumerable<UIComponent> components) : base(core)
        {
            if(components != null) Add(components);
        }


        // Methods #########################################################################
        /// <summary>
        /// Adds the specified <see cref="UIComponent"/>s to this <see cref="UIContainer"/>.
        /// </summary>
        /// <param name="components">The components to add.</param>
        public void Add(params UIComponent[] components) => Add(components as IEnumerable<UIComponent>);

        /// <summary>
        /// Adds the specified <see cref="UIComponent" />s to this <see cref="UIContainer" />.
        /// </summary>
        /// <param name="components">Enumeration of components to add.</param>
        /// <exception cref="ArgumentNullException">components</exception>
        public void Add(IEnumerable<UIComponent> components)
        {
            if (components == null) throw new ArgumentNullException(nameof(components));
            _UpdateLock = true;
            foreach (var component in components) Add(component);
            _UpdateLock = false;
            InvokeSizeChanged();
        }

        /// <summary>
        /// Adds the specified <see cref="UIComponent" /> to this <see cref="UIContainer" />.
        /// </summary>
        /// <param name="component">The component to add.</param>
        /// <exception cref="ArgumentNullException">component</exception>
        public virtual void Add(UIComponent component)
        {
            if (component == null) throw new ArgumentNullException(nameof(component));
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

        /// <summary>
        /// Removes the specified <see cref="UIComponent" /> from this <see cref="UIContainer" />.
        /// </summary>
        /// <param name="component">The component to remove.</param>
        /// <exception cref="ArgumentNullException">component</exception>
        /// <exception cref="ArgumentException">Component must be a child</exception>
        public virtual void Remove(UIComponent component)
        {
            if (component == null) throw new ArgumentNullException(nameof(component));
            if (component.Container != this) throw new ArgumentException("Component must be a child");
            component.Container = null;
            component.PositionChanged -= HandleChildComponentModified;
            component.OriginChanged -= HandleChildComponentModified;
            component.SizeChanged -= HandleChildComponentModified;
            component.MarginChanged -= HandleChildComponentModified;
            Remove(component as IEntity);
            InvokeComponentRemoved(component);
            InvokeSizeChanged();
        }

        /// <summary>
        /// Clears all <see cref="UIComponent"/>s from this <see cref="UIContainer" />.
        /// </summary>
        public override void Clear()
        {
            _UpdateLock = true;
            foreach (var component in Components.ToArray()) Remove(component);
            _UpdateLock = false;
            InvokeSizeChanged();
        }

        /// <summary>
        /// Gives the focus to this <see cref="UIContainer" />.
        /// </summary>
        /// <returns>Whether the <see cref="UIContainer" /> got the focus within the same frame.</returns>
        public override bool GiveFocus() => base.GiveFocus() || Components.Any(c => c.GiveFocus());

        /// <summary>
        /// Handles the child component modified event to update the size values.
        /// </summary>
        /// <param name="c">The source <see cref="UIComponent"/>.</param>
        protected virtual void HandleChildComponentModified(UIComponent c) => InvokeSizeChanged();

        /// <summary>
        /// Invokes the size changed event.
        /// </summary>
        protected override void InvokeSizeChanged()
        {
            if (_UpdateLock) return;
            _UpdateLock = true;
            UpdateDockedComponents();
            _UpdateLock = false;
            base.InvokeSizeChanged();
        }

        /// <summary>
        /// Updates the position and size of all docked components.
        /// </summary>
        protected virtual void UpdateDockedComponents()
        {
            foreach (var c in Components)
            {
                UpdateDockedComponent(c);
            }
        }

        /// <summary>
        /// Updates the position and size of a docked component.
        /// </summary>
        /// <param name="c">The component to update.</param>
        protected virtual void UpdateDockedComponent(UIComponent c)
        {
            if (c is IDockable dockComponent && (dockComponent.DockX || dockComponent.DockY))
            {
                // Reset
                c.Rotation = 0;
                c.Origin = new Vector2f(0, 0);
                c.Scale  = new Vector2f(1, 1);

                // Dock Position
                c.Position = new Vector2f(dockComponent.DockX ? c.Margin.Left : c.Position.X,
                                          dockComponent.DockY ? c.Margin.Top  : c.Position.Y);
                // Dock Size
                var components = Components.Select(co => co.RelativeSize).ToArray();

                var size = components.Length == 0 ? default(Vector2f) : 
                           new Vector2f(components.Max(v => v.X), components.Max(v => v.Y));

                dockComponent.Resize(new Vector2f(dockComponent.DockX ? size.X - (c.Margin.Left + c.Margin.Width)  : c.InnerSize.X,
                                                  dockComponent.DockY ? size.Y - (c.Margin.Top  + c.Margin.Height) : c.InnerSize.Y));
            }
        }

        /// <summary>
        /// Invokes the component added event.
        /// </summary>
        /// <param name="component">The component that has been added.</param>
        protected virtual void InvokeComponentAdded(UIComponent component) => ComponentAdded.Invoke(component);

        /// <summary>
        /// Invokes the component removed event.
        /// </summary>
        /// <param name="component">The component that has been removed.</param>
        protected virtual void InvokeComponentRemoved(UIComponent component) => ComponentRemoved.Invoke(component);
    }
}