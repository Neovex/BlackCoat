using System;
using SFML.Graphics;
using SFML.Window;

namespace BlackCoat.Entities
{
    public class GraphicLayer : Container
    {
        public new Vector2f Position { set { } } // disabled
        public new float Rotation { set { } } // disabled
        public new Vector2f Scale { set { } } // disabled

        public GraphicLayer(Core core) : base(core)
        { }

        public override void Update(Single deltaT)
        {
            for (int i = _Childs.Count - 1; i > -1; i--) _Childs[i].Update(deltaT);
        }
    }
}