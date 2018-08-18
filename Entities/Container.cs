using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackCoat.Entities
{
    /// <summary>
    /// Hierarchical render element. The scene graph is constructed primarily out of instances of this class.
    /// It can be used like a <see cref="Graphic"/> but additionally supports having child elements which will be rendered as well.
    /// </summary>
    public class Container : Graphic
    {
        // Variables #######################################################################
        protected internal List<IEntity> _Childs = new List<IEntity>();


        // Properties ######################################################################
        /// <summary>
        /// Transform Matrix defining Position, Scale and Rotation of the Entity
        /// </summary>
        public new Transform Transform { get { return Parent == null ? base.Transform : base.Transform * Parent.Transform; } }


        // CTOR ############################################################################
        public Container(Core core) : base(core)
        { }


        // Methods #########################################################################
        /// <summary>
        /// Adds an Entity to this Container
        /// </summary>
        /// <param name="e">The Entity to add</param>
        /// <returns>True if the Entity could be added</returns>
        public virtual Boolean AddChild(IEntity e)
        {
            if (e.Parent != null) e.Parent.RemoveChild(e);
            e.Parent = this;
            _Childs.Add(e);
            return true;
        }

        /// <summary>
        /// Removes the provided Entity from the Container
        /// </summary>
        /// <param name="e">The Entity to remove</param>
        public virtual void RemoveChild(IEntity e)
        {
            _Childs.Remove(e);
            e.Parent = null;
        }

        /// <summary>
        /// Retrieves the entity at the given index
        /// </summary>
        /// <param name="i">Entity index</param>
        /// <returns>The requested Entity if found otherwise null</returns>
        public virtual IEntity GetChildFromIndex(Int32 i)
        {
            return i > -1 && i < _Childs.Count ? _Childs[i] : null;
        }

        /// <summary>
        /// Gets the index of a Child Entity
        /// </summary>
        /// <param name="child">The request child entity</param>
        /// <returns>The index of the child or -1 if it is not contained</returns>
        public virtual Int32 GetChildIndex(IEntity child)
        {
            return _Childs.IndexOf(child);
        }

        /// <summary>
        /// Determines if the given entity is child of this container
        /// </summary>
        /// <param name="child">Entity to check</param>
        /// <returns>True if the entity is child of this container otherwise false</returns>
        public virtual Boolean HasChild(IEntity child)
        {
            return _Childs.Contains(child);
        }

        /// <summary>
        /// Clears all entities from this container.
        /// </summary>
        public virtual void Clear()
        {
            foreach (var child in _Childs) child.Parent = null;
            _Childs.Clear();
        }

        /// <summary>
        /// Updates the container and all subsequent entities along the scene graph using their respective roles
        /// </summary>
        /// <param name="deltaT">Last frame duration</param>
        override public void Update(Single deltaT)
        {
            base.Update(deltaT);
            for (int i = _Childs.Count - 1; i > -1; i--) _Childs[i].Update(deltaT);
        }

        /// <summary>
        /// Draws the Graphic of the container and all subsequent entities along the scene graph
        /// </summary>
        override public void Draw()
        {
            if (!Visible) return;
            if (Texture != null) base.Draw();
            foreach (IEntity e in _Childs) e.Draw();
        }
    }
}