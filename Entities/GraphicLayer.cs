using System;
using SFML.Graphics;

namespace BlackCoat
{
    public class GraphicLayer : Container
    {
        public override Vector2 Position { set { } } // disabled

        public GraphicLayer(Core core) : base(core)
        {
        }

        public override void Update(Single deltaT)
        {
            for (int i = _Childs.Count - 1; i > -1; i--) _Childs[i].Update(deltaT);
        }
    }
}