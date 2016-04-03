﻿using System;

namespace BlackCoat.Animation
{
    /// <summary>
    /// Abstract basclass of all animation types
    /// </summary>
    public abstract class Animation
    {   // Events ##########################################################################
        /// <summary>
        /// Occurs when the Timer reached its assigned target value and is therefore complete.
        /// </summary>
        public event Action<Animation> Complete = (t) => { };
        /// <summary>
        /// Occurs on each update cycle of the Animation with the interpolated value
        /// </summary>
        public event Action<float> Update = (v) => { };


        // Properties ######################################################################
        /// <summary>
        /// Value the Animation is headed
        /// </summary>
        public virtual float TargetValue { get; protected set; }
        /// <summary>
        /// Last calculated value
        /// </summary>
        public virtual float CurrentValue { get; protected set; }
        /// <summary>
        /// Determines if this <see cref="Animation"/> has finished.
        /// </summary>
        public virtual Boolean Finished { get; private set; }
        /// <summary>
        /// Determines if this <see cref="Animation"/> is currently paused.
        /// </summary>
        public virtual bool Paused { get; set; }
        /// <summary>
        /// Gets or sets an object that contains additional data for this <see cref="Animation"/>.
        /// </summary>
        public Object Tag { get; set; }


        // Methods #########################################################################
        /// <summary>
        /// Updates the current Value of this <see cref="Animation"/> based on the current frame time.
        /// </summary>
        /// <param name="deltaT">Frame Time</param>
        public abstract void UpdateAnimation(float deltaT);

        /// <summary>
        /// Immediately stops the <see cref="Animation"/>.
        /// </summary>
        public virtual void Cancel()
        {
            if (Finished) return;
            Finished = true;
            Complete(this);
        }

        /// <summary>
        /// Raises the Update Event
        /// </summary>
        /// <param name="value">Current Animation Value</param>
        protected void OnUpdate(float value) { Update(value); }

        /// <summary>
        /// Raises the Complete Event
        /// </summary>
        protected void OnComplete() { Complete(this); }
    }
}