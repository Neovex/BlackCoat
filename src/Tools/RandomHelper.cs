using System;
using SFML.System;
using SFML.Graphics;

namespace BlackCoat.Tools
{
    /// <summary>
    /// Helperclass that extends the default C# Pseudo Random Number Generator with float numbers
    /// </summary>
    public class RandomHelper : Random
    {
        // CTOR ############################################################################
        /// <summary>
        /// Creates a Random with a default seed.
        /// </summary>
        internal RandomHelper():base() { }


        // Methods #########################################################################
        /// <summary>
        /// Generates a random float number
        /// </summary>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <returns>A random number within the bounds</returns>
        public Single NextFloat(float min, float max)
        {
            // Prechecks
            if (min == max) return max;
            if (min > max) throw new ArgumentException("Min must not be larger than max");

            // Negative cases
            if (max < 0) return NextFloat(max * -1, min * -1) * -1;
            if (min < 0) return NextFloat(0, max - min) + min;

            // Random
            float ret;
            do ret = (float)NextDouble() * max;
            while (ret < min);
            return ret;
        }

        /// <summary>
        /// Generates a random Vector
        /// </summary>
        /// <param name="xmin">Minimum value for x</param>
        /// <param name="xmax">Maximum value for x</param>
        /// <param name="ymin">Minimum value for y</param>
        /// <param name="ymax">Maximum value for y</param>
        /// <returns>A Vector within the bounds</returns>
        public Vector2f NextVector(float xmin, float xmax, float ymin, float ymax)
        {
            return new Vector2f(NextFloat(xmin, xmax), NextFloat(ymin, ymax));
        }

        /// <summary>
        /// Generates a random Vector
        /// </summary>
        /// <param name="min">Minimum value for x and y</param>
        /// <param name="max">Maximum value for x and y</param>
        /// <returns>A Vector within the bounds</returns>
        public Vector2f NextVector(float min, float max)
        {
            return NextVector(min, max, min, max);
        }

        /// <summary>
        /// Generates a random Vector within a rectangular space
        /// </summary>
        /// <param name="min">Top left corner of the allowed space</param>
        /// <param name="max">Bottom right corner of the allowed space</param>
        /// <returns>A Vector within the bounds</returns>
        public Vector2f NextVector(Vector2f min, Vector2f max)
        {
            return NextVector(min.X, max.X, min.Y, max.Y);
        }

        /// <summary>
        /// Generates a random Vector within a rectangular space
        /// </summary>
        /// <param name="area">Rectangle defining the allowed space</param>
        /// <returns>A Vector within the bounds</returns>
        public Vector2f NextVector(FloatRect area)
        {
            return NextVector(area.Left, area.Top, area.Left + area.Width, area.Top + area.Height);
        }
    }
}