using System;
using System.Collections.Generic;

namespace BlackCoat.Animation
{
    /// <summary>
    /// Animation Factory and management class. Handles all running Animations. Makes stuff move.
    /// </summary>
    public sealed class AnimationManager
    {
        // Statics #########################################################################
        public static Int32 ACTIVE_ANIMATIONS = 0;

        
        // Variables #######################################################################
        private List<Animation> _ActiveAnimations = new List<Animation>();
        private List<Animation> _AnimationsToAdd = new List<Animation>();
        private List<Animation> _AnimationsToRemove = new List<Animation>();


        // Properties ######################################################################
        /// <summary>
        /// Retrieves the ammount of currently running Animations.
        /// </summary>
        public int ActiveAnimations { get { return _ActiveAnimations.Count; } }


        // CTOR ############################################################################
        internal AnimationManager()
        { }


        // Methods #########################################################################
        /// <summary>
        /// Runs the provided Animation
        /// </summary>
        /// <param name="Animation">Animation to start</param>
        public void Start(Animation Animation)
        {
            if (_ActiveAnimations.Contains(Animation) || _AnimationsToAdd.Contains(Animation)) throw new InvalidOperationException("Animation cannot be added twice");
            if (Animation.Finished) throw new InvalidOperationException("Animations cannot be reused");
            Animation.Complete += HandleAnimationCompleted;
            _AnimationsToAdd.Add(Animation);

            ACTIVE_ANIMATIONS++;
        }

        /// <summary>
        /// Creates a new timebased Animation and runs it.
        /// </summary>
        /// <param name="startValue">Value to start the Animation from</param>
        /// <param name="targetValue">Value to Animation to</param>
        /// <param name="duration">Animation duration in fractal seconds</param>
        /// <param name="onUpdate">Optional delegate providing the last interpolated value</param>
        /// <param name="interpolation">Optional delegate to interpolate the Animation</param>
        /// <param name="onComplete">Optional delegate called when the Animation has finished</param>
        /// <param name="tag">Optional Object that contains additional data</param>
        /// <returns>An instance of <see cref="TimeAnimation"/></returns>
        public TimeAnimation Run(float startValue, float targetValue, float duration, Action<float> onUpdate = null, InterpolationType interpolation = InterpolationType.Linear, Action<Animation> onComplete = null, object tag = null)
        {
            var animation = new TimeAnimation(startValue, targetValue, duration, Interpolation.Get(interpolation));
            if (onUpdate != null) animation.Update += onUpdate;
            if (onComplete != null) animation.Complete += onComplete;
            animation.Tag = tag;
            Start(animation);
            return animation;
        }

        /// <summary>
        /// Creates a new valuebased Animation and runs it.
        /// </summary>
        /// <param name="getter">Delegate to retrieve the current value</param>
        /// <param name="setter">Delegate providing the last interpolated value</param>
        /// <param name="targetValue">Value to Animation to</param>
        /// <param name="modifier">Optional animation speed multiplier</param>
        /// <param name="onComplete">Optional delegate called when the Animation has finished</param>
        /// <param name="tag">Optional Object that contains additional data</param>
        /// <returns>An instance of <see cref="ValueAnimation"/></returns>
        public ValueAnimation Run(Func<float> getter, Action<float> setter, float targetValue, float modifier = 100, Action<Animation> onComplete = null, object tag = null)
        {
            var animation = new ValueAnimation(getter, targetValue, modifier);
            animation.Update += setter;
            if (onComplete != null) animation.Complete += onComplete;
            animation.Tag = tag;
            Start(animation);
            return animation;
        }

        /// <summary>
        /// Creates a new Waiting Timer
        /// </summary>
        /// <param name="time">Time to wait in fractal seconds</param>
        /// <param name="onComplete">Delegate called when the timer has finished</param>
        /// <param name="onUpdate">Optional delegate providing the last interpolated value</param>
        /// <param name="tag">Optional Object that contains additional data</param>
        /// <returns>An instance of <see cref="Timer"/></returns>
        public Timer Wait(float time, Action<Animation> onComplete, Action<float> onUpdate = null, object tag = null)
        {
            var timer = new Timer(time);
            if (onUpdate != null) timer.Update += onUpdate;
            timer.Complete += onComplete;
            timer.Tag = tag;
            Start(timer);
            return timer;
        }

        /// <summary>
        /// Updates all Running Animations and removes completed ones.
        /// </summary>
        /// <param name="deltaT">Frame Time</param>
        internal void Update(float deltaT)
        {
            // Add
            _ActiveAnimations.AddRange(_AnimationsToAdd);
            _AnimationsToAdd.Clear();

            // Update
            foreach (var Animation in _ActiveAnimations)
            {
                Animation.UpdateAnimation(deltaT);
            }

            // Delete
            foreach (var Animation in _AnimationsToRemove)
            {
                _ActiveAnimations.Remove(Animation);
            }
            _AnimationsToRemove.Clear();
        }

        /// <summary>
        /// Handler for completed Animations
        /// </summary>
        /// <param name="Animation">Animation that is finished or canceled</param>
        private void HandleAnimationCompleted(Animation Animation)
        {
            Animation.Complete -= HandleAnimationCompleted;
            _AnimationsToRemove.Add(Animation);

            ACTIVE_ANIMATIONS--;
        }
    }
}