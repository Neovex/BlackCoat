using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.System;

namespace BlackCoat
{
    public static class VectorExtensions
    {
        public static Vector2i ToVector2i(this Vector2f v)
        {
            return new Vector2i((int)v.X, (int)v.Y);
        }

        public static Vector2f ToVector2f(this Vector2i v)
        {
            return new Vector2f(v.X, v.Y);
        }
    }
}
