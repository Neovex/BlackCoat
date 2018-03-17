using System;
using SFML.System;

namespace BlackCoat.Tools
{
    /// <summary>
    /// Helperclass that extends the default C# Pseudo Random Number Generator with float numbers
    /// </summary>
    public class RandomHelper : Random
    {
        // CTOR ############################################################################
        /// <summary>
        /// Creates a new Instance of the RandomHelper class.
        /// Creates a Random with a default seed.
        /// </summary>
        internal RandomHelper():base()
        { }


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
            if (min > max) throw new ArgumentException();

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
        /// Generates a Vector with random coordinates
        /// </summary>
        /// <param name="min">Minimum value for x and y</param>
        /// <param name="max">Maximum value for x and y</param>
        /// <returns>A Vector within the bounds</returns>
        public Vector2f NextVector(float min, float max)
        {
            return NextVector(min, max, min, max);
        }
        /// <summary>
        /// Generates a Vector with random coordinates
        /// </summary>
        /// <param name="xmin">Minimum value for x</param>
        /// <param name="xmax">Maximum value for x</param>
        /// <param name="ymin">Minimum value for y</param>
        /// <param name="ymax">Maximum value for y</param>
        /// <returns>
        /// A Vector within the bounds
        /// </returns>
        public Vector2f NextVector(float xmin, float xmax, float ymin, float ymax)
        {
            return new Vector2f(NextFloat(xmin, xmax), NextFloat(ymin, ymax));
        }
    }
}