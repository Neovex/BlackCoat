using System;

namespace BlackCoat.Animation
{
    /// <summary>
    /// A <see cref="ValueAnimation"/> is an abstraction of a valuebased constant value change.
    /// A new value is calculated on each update based on the provided modifiers taking the current target value into account as well.
    /// </summary>
    public class ValueAnimation : Animation
    {
        // Variables #######################################################################
        private Func<float> _Getter;


        // Properties ######################################################################
        /// <summary>
        /// Modifier that will increase or decrease the speed of this <see cref="ValueAnimation"/>.
        /// </summary>
        public float Modifier { get; set; }


        // CTOR ############################################################################
        /// <summary>
        /// Creates a new instance of the <see cref="ValueAnimation"/> class.
        /// </summary>
        public ValueAnimation(Func<float> getter, float targetValue, float modifier = 100)
        {
            _Getter = getter;
            TargetValue = targetValue;
            Modifier = modifier;
            if (TargetValue < _Getter() && Modifier > 0) Modifier *= -1;
        }


        // Methods #########################################################################
        /// <summary>
        /// Updates the current Value of this <see cref="Animation"/> based on the current frame time.
        /// </summary>
        /// <param name="deltaT">Frame Time</param>
        override public void UpdateAnimation(float deltaT)
        {
            CurrentValue = _Getter() + Modifier * deltaT;
            if((Modifier > 0 && CurrentValue >= TargetValue) ||
               (Modifier < 0 && CurrentValue <= TargetValue))
            {
                OnUpdate(CurrentValue = TargetValue);
                Cancel();
            }
            else
            {
                OnUpdate(CurrentValue);
            }
        }
    }
}