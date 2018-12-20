using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace BlackCoat
{
    public static class RectangleExtensions
    {
        public static Vector2f Position(this FloatRect src) => new Vector2f(src.Left, src.Top);
        public static Vector2f Size(this FloatRect src) => new Vector2f(src.Width, src.Height);

        public static Vector2i Position(this IntRect src) => new Vector2i(src.Left, src.Top);
        public static Vector2i Size(this IntRect src) => new Vector2i(src.Width, src.Height);
    }
}