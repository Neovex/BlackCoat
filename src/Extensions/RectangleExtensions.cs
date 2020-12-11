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

        /* Unnecessary due to manual changes in the SFML wrapper
        public static IntRect ToIntRect(this (int, int, int, int) src) => new IntRect(src.Item1, src.Item2, src.Item3, src.Item4);
        public static FloatRect ToFloatRect(this (int, int, int, int) src) => new FloatRect(src.Item1, src.Item2, src.Item3, src.Item4);
        public static FloatRect ToFloatRect(this (float, float, float, float) src) => new FloatRect(src.Item1, src.Item2, src.Item3, src.Item4);
        public static FloatRect ToFloatRect(this (double, double, double, double) src) => new FloatRect((float)src.Item1, (float)src.Item2, (float)src.Item3, (float)src.Item4);
        */
    }
}