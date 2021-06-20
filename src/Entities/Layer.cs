using System;
using SFML.System;
using SFML.Graphics;

namespace BlackCoat.Entities
{
    /// <summary>
    /// Internal Render class.
    /// </summary>
    public class Layer : Container
    {
        // Properties ######################################################################
        /// <summary>
        /// Gets or sets the time multiplier. Default Value 1. 
        /// Changing this value will slow down or speed up all entities
        /// that base their actions on the frame time.
        /// </summary>
        public float TimeMultiplier { get; set; }

        // Disabled
        /// <summary>
        /// A <see cref="Layer"/>s position cannot be changed.
        /// </summary>
        public override Vector2f Position { get => base.Position; set => throw new Exception("Invalid Operation. Layers cannot be moved."); }

        /// <summary>
        /// A <see cref="Layer"/>s Rotation cannot be changed.
        /// </summary>
        public override float Rotation { get => base.Rotation; set => throw new Exception("Invalid Operation. Layers cannot be rotated."); }

        /// <summary>
        /// A <see cref="Layer"/>s Scale cannot be changed.
        /// </summary>
        public override Vector2f Scale { get => base.Scale; set => throw new Exception("Invalid Operation. Layers cannot be scaled."); }

        /// <summary>
        /// A <see cref="Layer"/>s texture must not be defined.
        /// </summary>
        public new Texture Texture { get => null; set => throw new Exception("Invalid Operation. Layers cannot be assigned a texture."); }


        // CTOR ############################################################################
        internal Layer(Core core) : base(core)
        {
            TimeMultiplier = 1;
        }


        // Methods #########################################################################
        /// <summary>
        /// Updates the <see cref="Layer"/> and all its entities along the scene graph.
        /// </summary>
        /// <param name="deltaT">Duration of the last frame</param>
        public override void Update(Single deltaT) => base.Update(deltaT * TimeMultiplier);

        /// <summary>
        /// Adds the specified entities.
        /// </summary>
        public void Add(params IEntity[] entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            foreach (var entity in entities) base.Add(entity);
        }
    }
}