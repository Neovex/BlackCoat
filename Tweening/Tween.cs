using System;

namespace BlackCoat.Tweening
{
    /// <summary>
    /// A tween is an abstraction of a timebased constant value change.
    /// A new value is calculated on each update based on the provided interpolation delegate.
    /// </summary>
    public sealed class Tween
    {
        // Events ##########################################################################
        /// <summary>
        /// Occurs when the Tween reached its assigned target value and is therefore complete.
        /// </summary>
        public event Action<Tween> Completed = (t) => { };


        // Variables #######################################################################
        private Action<float> _Setter;
        private Func<float, float, float, float, float> _Interpolation;
        private float _Distance;
        private float _ElapsedTime;


        // Properties ######################################################################
        /// <summary>
        /// Value the Tween started from
        /// </summary>
        public float StartValue { get; private set; }
        /// <summary>
        /// Value the Tween is headed
        /// </summary>
        public float TargetValue { get; private set; }
        /// <summary>
        /// Last interpolated value
        /// </summary>
        public float CurrentValue { get; private set; }
        /// <summary>
        /// Duration of the Tween
        /// </summary>
        public float Duration { get; private set; }
        /// <summary>
        /// Determines if this tween has finished.
        /// </summary>
        public Boolean Finished { get; private set; }


        // CTOR ############################################################################
        /// <summary>
        /// Creates a new Instance of the <see cref="Tween"/> class.
        /// </summary>
        /// <param name="startValue">Value to start the Tween from</param>
        /// <param name="targetValue">Value to Tween to</param>
        /// <param name="duration">Tween duration in fractal seconds</param>
        /// <param name="setter">Delegate to assign the calculated value to a target</param>
        /// <param name="interpolation">Delegate to interpolate the Tween</param>
        internal Tween(float startValue, float targetValue, float duration, Action<float> setter, Func<float, float, float, float, float> interpolation)
        {
            StartValue = startValue;
            TargetValue = targetValue;
            Duration = duration;
            _Setter = setter;
            _Interpolation = interpolation;

            // Calculate Distance
            _Distance = TargetValue - StartValue;
        }

        /// <summary>
        /// Calculates the current Tween-Value based on the associated interpolation delegate
        /// </summary>
        /// <param name="deltaT">Frame Time</param>
        internal void Update(float deltaT)
        {
            _ElapsedTime += deltaT;
            CurrentValue = _Interpolation.Invoke(StartValue, _Distance, _ElapsedTime, Duration);
            _Setter(CurrentValue);
            if (CurrentValue == TargetValue) Cancel();
        }

        /// <summary>
        /// Immediately stops the Tween.
        /// </summary>
        public void Cancel()
        {
            if (Finished) return;
            Finished = true;
            Completed(this);
        }
    }
}