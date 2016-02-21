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
        public new Vector2f Position { set { } } // disabled
        public new float Rotation { set { } } // disabled
        public new Vector2f Scale { set { } } // disabled


        // CTOR ############################################################################
        internal Layer(Core core) : base(core)
        { }


        // Methods #########################################################################
        public override void Update(Single deltaT)
        {
            for (int i = _Childs.Count - 1; i > -1; i--) _Childs[i].Update(deltaT);
        }
    }
}