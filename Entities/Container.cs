using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackCoat.Entities
{
    public class Container : GraphicItem
    {
        // Variables #######################################################################
        protected List<IEntity> _Childs = new List<IEntity>();


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
        /// <returns>The requested Entity if found otherwhise null</returns>
        public virtual IEntity GetChildFromIndex(Int32 i)
        {
            if (i > -1 && i < _Childs.Count) return _Childs[i];
            return null;
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
        /// <returns>True if the entity is child of this container otherwhise false</returns>
        public virtual Boolean HasChild(IEntity child)
        {
            return _Childs.Contains(child);
        }

        /// <summary>
        /// Updates the Entity Space and all chiilds using their respecitve roles
        /// </summary>
        /// <param name="deltaT">Current gametime</param>
        override public void Update(Single deltaT)
        {
            base.Update(deltaT);
            for (int i = _Childs.Count - 1; i > -1; i--) _Childs[i].Update(deltaT);
        }

        /// <summary>
        /// Draws the Graphic of the Entity Space and its childs if it is visible
        /// </summary>
        override public void Draw()
        {
            if (!Visible) return;
            if (Texture != null) base.Draw();
            foreach (IEntity e in _Childs) e.Draw();
        }
    }
}