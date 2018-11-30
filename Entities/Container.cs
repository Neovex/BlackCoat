using System;
using System.Linq;
using System.Collections.Generic;
using SFML.Graphics;

namespace BlackCoat.Entities
{
    /// <summary>
    /// Hierarchical render element. The scene graph is constructed primarily out of instances of this class.
    /// It can be used like a <see cref="Graphic"/> but additionally draws each subsidiary entity within its own coordinate system.
    /// </summary>
    public class Container : Graphic
    {
        // Variables #######################################################################
        protected internal readonly List<IEntity> _Entities;


        // Properties ######################################################################
        /// <summary>
        /// Transformation Matrix defining Position, Scale and Rotation of the Entity within the scene graph
        /// </summary>
        public virtual Transform GlobalTransform => Parent == null ? Transform : Transform * Parent.GlobalTransform;

        /// <summary>
        /// Retrieves the entity at the given index or null when the index is invalid
        /// </summary>
        public virtual IEntity this[int i] => i > -1 && i < _Entities.Count ? _Entities[i] : null;

        /// <summary>
        /// Alpha Visibility - 0-1f
        /// </summary>
        public override float Alpha
        {
            get => base.Alpha;
            set
            {
                base.Alpha = value;
                // Update all subsidiary entities by reassigning each its respective original alpha value
                foreach (var entity in _Entities) entity.Alpha = entity.Alpha;
            }
        }


        // CTOR ############################################################################
        public Container(Core core) : base(core)
        {
            _Entities = new List<IEntity>();
        }


        // Methods #########################################################################
        /// <summary>
        /// Adds an Entity to this Container
        /// </summary>
        /// <param name="entity">The Entity to add</param>
        /// <returns>True if the Entity could be added</returns>
        public virtual Boolean Add(IEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (entity.Parent != null) entity.Parent.Remove(entity);
            entity.Parent = this;
            _Entities.Add(entity);
            return true;
        }

        /// <summary>
        /// Removes the provided Entity from the Container
        /// </summary>
        /// <param name="entity">The Entity to remove</param>
        public virtual void Remove(IEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _Entities.Remove(entity);
            entity.Parent = null;
        }

        /// <summary>
        /// Gets the index of a Child Entity
        /// </summary>
        /// <param name="entity">The target entity</param>
        /// <returns>The index of the entity or -1 if it is not contained</returns>
        public virtual Int32 IndexOf(IEntity entity) => _Entities.IndexOf(entity);

        /// <summary>
        /// Determines if the given entity is inside of this container
        /// </summary>
        /// <param name="entity">Entity to check</param>
        /// <returns>True if the entity is inside of this container otherwise false</returns>
        public virtual Boolean Contains(IEntity entity) => _Entities.Contains(entity);

        /// <summary>
        /// Gets the first entity of the provided type inside this <see cref="Container"/>
        /// </summary>
        /// <typeparam name="T">Type of the entity</typeparam>
        /// <returns>The first entity of the given type or null when there is none</returns>
        public T GetFirst<T>() where T : IEntity => _Entities.OfType<T>().FirstOrDefault();

        /// <summary>
        /// Gets all entities of the provided type.
        /// </summary>
        /// <typeparam name="T">Type of the entities</typeparam>
        /// <returns>An array containing all entities of the given type</returns>
        public T[] GetAll<T>() where T : IEntity => _Entities.OfType<T>().ToArray();

        /// <summary>
        /// Clears all entities from this container.
        /// </summary>
        public virtual void Clear()
        {
            foreach (var entity in _Entities) entity.Parent = null;
            _Entities.Clear();
        }

        /// <summary>
        /// Updates the container and all subsequent entities along the scene graph using their respective roles
        /// </summary>
        /// <param name="deltaT">Last frame duration</param>
        override public void Update(Single deltaT)
        {
            base.Update(deltaT);
            for (int i = _Entities.Count - 1; i > -1; i--) _Entities[i].Update(deltaT);
        }

        /// <summary>
        /// Draws the Graphic of the container and all subsequent entities along the scene graph
        /// </summary>
        override public void Draw()
        {
            if (!Visible) return;
            if (Texture != null) base.Draw();
            foreach (var entity in _Entities) entity.Draw();
        }

        /// <summary>
        /// Handles the destruction of the object
        /// </summary>
        /// <param name="disposing">Determines if the GC is disposing the object (true), or it's an explicit call (false).</param>
        protected override void Destroy(bool disposing)
        {
            foreach (var entity in _Entities)
            {
                entity.Parent = null;
                entity.Dispose();
            }
            _Entities.Clear();
            base.Destroy(disposing);
        }
    }
}