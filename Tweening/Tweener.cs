using System;
using System.Collections.Generic;

namespace BlackCoat.Tweening
{
    /// <summary>
    /// Tween Factory and management class. Handles all running tweens. Makes stuff move.
    /// </summary>
    public sealed class Tweener
    {
        // Statics #########################################################################
        public static Int32 ACTIVE_TWEENS = 0;


        
        // Variables #######################################################################
        private List<Tween> _ActiveTweens = new List<Tween>();
        private List<Tween> _TweensToAdd = new List<Tween>();
        private List<Tween> _TweensToRemove = new List<Tween>();


        // Properties ######################################################################
        /// <summary>
        /// Retrieves the ammount of currently running tweens.
        /// </summary>
        public int ActiveTweens { get { return _ActiveTweens.Count; } }


        // CTOR ############################################################################
        internal Tweener()
        { }


        // Methods #########################################################################
        /// <summary>
        /// Creates a new unmanaged Tween.
        /// </summary>
        /// <param name="startValue">Value to start the Tween from</param>
        /// <param name="targetValue">Value to Tween to</param>
        /// <param name="duration">Tween duration in fractal seconds</param>
        /// <param name="setter">Delegate to assign the calculated value to a target</param>
        /// <param name="interpolation">Delegate to interpolate the Tween</param>
        /// <returns>Created Tween</returns>
        public Tween Create(float startValue, float targetValue, float duration, Action<float> setter, Func<float, float, float, float, float> interpolation)
        {
            return new Tween(startValue, targetValue, duration, setter, interpolation);
        }

        /// <summary>
        /// Adds a Tween to the Tweeneer so it can run.
        /// </summary>
        /// <param name="tween">Tween to add</param>
        public void Add(Tween tween)
        {
            if (_ActiveTweens.Contains(tween) || _TweensToAdd.Contains(tween)) throw new InvalidOperationException("tween cannot be added twice");
            if (tween.Finished) throw new InvalidOperationException("tweens cannot be reused");
            tween.Completed += HandleTweenCompleted;
            _TweensToAdd.Add(tween);

            ACTIVE_TWEENS++;
        }

        /// <summary>
        /// Creates a new Tween and adds it to the Tweeneer so it can run.
        /// </summary>
        /// <param name="startValue">Value to start the Tween from</param>
        /// <param name="targetValue">Value to Tween to</param>
        /// <param name="duration">Tween duration in fractal seconds</param>
        /// <param name="setter">Delegate to assign the calculated value to a target</param>
        /// <returns>Created Tween</returns>
        public Tween Add(float startValue, float targetValue, float duration, Action<float> setter)
        {
            return Add(startValue, targetValue, duration, setter, Interpolation.Linear);
        }

        /// <summary>
        /// Creates a new Tween and adds it to the Tweeneer so it can run.
        /// </summary>
        /// <param name="startValue">Value to start the Tween from</param>
        /// <param name="targetValue">Value to Tween to</param>
        /// <param name="duration">Tween duration in fractal seconds</param>
        /// <param name="setter">Delegate to assign the calculated value to a target</param>
        /// <param name="interpolation">Delegate to interpolate the Tween</param>
        /// <returns>Created Tween</returns>
        public Tween Add(float startValue, float targetValue, float duration, Action<float> setter, Func<float, float, float, float, float> interpolation)
        {
            var tween = Create(startValue, targetValue, duration, setter, interpolation);
            Add(tween);
            return tween;
        }

        /// <summary>
        /// Updates all Running tweens and removes completed ones.
        /// </summary>
        /// <param name="deltaT">Frame Time</param>
        internal void Update(float deltaT)
        {
            // Add
            _ActiveTweens.AddRange(_TweensToAdd);
            _TweensToAdd.Clear();

            // Update
            foreach (var tween in _ActiveTweens)
            {
                tween.Update(deltaT);
            }

            // Delete
            foreach (var tween in _TweensToRemove)
            {
                _ActiveTweens.Remove(tween);
            }
            _TweensToRemove.Clear();
        }

        /// <summary>
        /// Handler for completed Tweens
        /// </summary>
        /// <param name="tween">Tween that is finished or canceled</param>
        private void HandleTweenCompleted(Tween tween)
        {
            tween.Completed -= HandleTweenCompleted;
            _TweensToRemove.Add(tween);

            ACTIVE_TWEENS--;
        }
    }
}