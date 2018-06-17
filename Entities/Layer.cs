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
        public new float Rotation    { set { throw new Exception("Invalid Operation"); } }
        public new Vector2f Scale    { set { throw new Exception("Invalid Operation"); } }
        public new Texture Texture   { set { throw new Exception("Invalid Operation"); } }


        // CTOR ############################################################################
        internal Layer(Core core) : base(core)
        {
            TimeMultiplier = 1;
        }


        // Methods #########################################################################
        public override void Update(Single deltaT)
        {
            deltaT *= TimeMultiplier;
            for (int i = _Childs.Count - 1; i > -1; i--) _Childs[i].Update(deltaT);
        }
    }
}