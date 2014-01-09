using System;

namespace BlackCoat.Tools
{
    public class RandomHelper : Random
    {
        /// <summary>
        /// Creates a new Instance of the RandomHelper class.
        /// Creates a Random with a default seed.
        /// </summary>
        public RandomHelper():base()
        { }


        /// <summary>
        /// Generates a random float number
        /// </summary>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <returns>A random number between the bounds</returns>
        public Single NextFloat(float min, float max)
        {
            // Prechecks
            if (min > max) throw new ArgumentException();
            if (min == max) return max;

            // Negative case
            if (min < 0) return NextFloat(0, max - min) + min;

            // Random
            float ret;
            do ret = (float)NextDouble() * max;
            while (ret < min);
            return ret;
        }
    }
}