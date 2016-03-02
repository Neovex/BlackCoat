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
        /// Creates a new Tween.
        /// </summary>
        /// <param name="startValue">Value to start the Tween from</param>
        /// <param name="targetValue">Value to Tween to</param>
        /// <param name="duration">Tween duration in fractal seconds</param>
        /// <param name="interpolation">Optional delegate to interpolate the Tween</param>
        /// <returns>Created Tween</returns>
        public Tween Create(float startValue, float targetValue, float duration, Func<float, float, float, float, float> interpolation = null)
        {
            return new Tween(startValue, targetValue, duration, interpolation ?? Interpolation.Linear);
        }

        /// <summary>
        /// Runs the provided Tween
        /// </summary>
        /// <param name="tween">Tween to add</param>
        public void Run(Tween tween)
        {
            if (_ActiveTweens.Contains(tween) || _TweensToAdd.Contains(tween)) throw new InvalidOperationException("tween cannot be added twice");
            if (tween.Finished) throw new InvalidOperationException("tweens cannot be reused");
            tween.OnComplete += HandleTweenCompleted;
            _TweensToAdd.Add(tween);

            ACTIVE_TWEENS++;
        }

        /// <summary>
        /// Creates a new Tween and runs it.
        /// </summary>
        /// <param name="startValue">Value to start the Tween from</param>
        /// <param name="targetValue">Value to Tween to</param>
        /// <param name="duration">Tween duration in fractal seconds</param>
        /// <param name="onUpdate">Optional delegate to assign the interpolated value to a target</param>
        /// <param name="interpolation">Optional delegate to interpolate the Tween</param>
        /// <param name="onComplete">Optional delegate called when the tween has finished</param>
        /// <returns>Created Tween</returns>
        public Tween Run(float startValue, float targetValue, float duration, Action<float> onUpdate = null, Func<float, float, float, float, float> interpolation = null, Action<Tween> onComplete = null)
        {
            var tween = Create(startValue, targetValue, duration, interpolation ?? Interpolation.Linear);
            if (onUpdate != null) tween.OnUpdate += onUpdate;
            if (onComplete != null) tween.OnComplete += onComplete;
            Run(tween);
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
            tween.OnComplete -= HandleTweenCompleted;
            _TweensToRemove.Add(tween);

            ACTIVE_TWEENS--;
        }
    }
}