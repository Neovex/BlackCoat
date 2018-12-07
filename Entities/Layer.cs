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
        /// Gets or sets the time multiplier. Default Value 1. Changing this value will slow down or speed up all time based entities on this <see cref="Layer"/>
        /// </summary>
        public float TimeMultiplier { get; set; }

        // Disabled Properties
        public new Vector2f Position { set { throw new Exception("Invalid Operation"); } }
        public new float Rotation { set { throw new Exception("Invalid Operation"); } }
        public new Vector2f Scale { set { throw new Exception("Invalid Operation"); } }
        public new Texture Texture { set { throw new Exception("Invalid Operation"); } }


        // CTOR ############################################################################
        internal Layer(Core core) : base(core)
        {
            TimeMultiplier = 1;
        }


        // Methods #########################################################################
        public override void Update(Single deltaT)
        {
            deltaT *= TimeMultiplier;
            for (int i = _Entities.Count - 1; i > -1; i--) _Entities[i].Update(deltaT);
        }

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