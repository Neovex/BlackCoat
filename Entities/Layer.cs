using System;
using SFML.System;

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
        public new Vector2f Position { set { } } // disabled
        public new float Rotation { set { } } // disabled
        public new Vector2f Scale { set { } } // disabled


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