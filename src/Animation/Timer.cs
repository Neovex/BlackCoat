using System;

namespace BlackCoat.Animation
{
    /// <summary>
    /// A <see cref="Timer"/> is a simple timekeeper to manage animations.
    /// </summary>
    public class Timer : Animation
    {
        // CTOR ############################################################################
        /// <summary>
        /// Creates a <see cref="Timer"/> that represents a waiting time period
        /// </summary>
        /// <param name="duration">Length of this <see cref="Timer"/> in Fractal Seconds</param>
        public Timer(float duration)
        {
            if (duration <= 0) throw new ArgumentException("duration must be greater than 0");
            TargetValue = duration;
        }


        // Methods #########################################################################
        /// <summary>
        /// Updates the current Value of this <see cref="Animation"/> based on the current frame time.
        /// </summary>
        /// <param name="deltaT">Frame Time</param>
        override protected void UpdateInternal(float deltaT)
        {
            CurrentValue += deltaT;
            OnUpdate(CurrentValue);
            if (CurrentValue >= TargetValue) Stop();
        }
    }
}