using System;

namespace BlackCoat.Animation
{
    /// <summary>
    /// A <see cref="TimeAnimation"/> is an abstraction of a time based constant value change.
    /// A new value is calculated on each update cycle based on the provided interpolation method.
    /// </summary>
    public class TimeAnimation : Animation
    {
        // Variables #######################################################################
        private Func<float, float, float, float, float> _Interpolation;
        private float _Distance;
        private float _ElapsedTime;


        // Properties ######################################################################
        /// <summary>
        /// Value the Animation started from
        /// </summary>
        public float StartValue { get; private set; }
        /// <summary>
        /// Duration of the Animation
        /// </summary>
        public float Duration { get; private set; }


        // CTOR ############################################################################
        /// <summary>
        /// Creates a new Instance of the <see cref="TimeAnimation"/> class.
        /// </summary>
        /// <param name="startValue">Value to start the Animation from</param>
        /// <param name="targetValue">Value to Animation to</param>
        /// <param name="duration">Animation duration in fractal seconds</param>
        /// <param name="setter">Delegate to assign the calculated value to a target</param>
        /// <param name="interpolation">Delegate to interpolate the Animation</param>
        public TimeAnimation(float startValue, float targetValue, float duration, Func<float, float, float, float, float> interpolation)
        {
            StartValue = startValue;
            TargetValue = targetValue;
            Duration = duration;
            _Interpolation = interpolation;

            // Calculate Distance
            _Distance = TargetValue - StartValue;
        }


        // Methods #########################################################################
        /// <summary>
        /// Updates the current Value of this <see cref="Animation"/> based on the current frame time.
        /// </summary>
        /// <param name="deltaT">Frame Time</param>
        override public void UpdateAnimation(float deltaT)
        {
            if (Paused) return;
            _ElapsedTime += deltaT;
            CurrentValue = _Interpolation.Invoke(StartValue, _Distance, _ElapsedTime, Duration);
            OnUpdate(CurrentValue);
            if (CurrentValue == TargetValue) Cancel();
        }
    }
}