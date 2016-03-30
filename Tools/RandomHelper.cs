using System;

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
        /// <returns>A random number between the bounds</returns>
        public Single NextFloat(float min, float max)
        {
            // Prechecks
            if (min > max) throw new ArgumentException();
            if (min == max) return max;

            // Negative cases
            if (max < 0) return NextFloat(max * -1, min * -1) * -1;
            if (min < 0) return NextFloat(0, max - min) + min;

            // Random
            float ret;
            do ret = (float)NextDouble() * max;
            while (ret < min);
            return ret;
        }
    }
}