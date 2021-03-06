﻿using System;
using SFML.Graphics;

namespace BlackCoat.Entities.Animation
{
    /// <summary>
    /// Animated Graphic Entity. Requires one Texture per Frame.
    /// </summary>
    public class FrameAnimation:Graphic
    {
        // Variables #######################################################################
        protected Int32 _CurrentFrame = -1;
        protected Texture[] _Frames;
        protected Single _FrameTime;


        // Properties ######################################################################
        /// <summary>
        /// Duration of each frame
        /// </summary>
        public virtual Single FrameDuration { get; set; }


        // CTOR ############################################################################
        /// <summary>
        /// Creates a new Instance of the FrameAnimation class.
        /// </summary>
        /// <param name="core">The Engine Core</param>
        /// <param name="frameDuration">Duration of one frame</param>
        /// <param name="frames">All frame textures - one per frame</param>
        public FrameAnimation(Core core, Single frameDuration, Texture[] frames) : base(core)
        {
            FrameDuration = frameDuration;
            if (frames == null) throw new NullReferenceException(nameof(frames));
            if (frames.Length < 2) throw new ArgumentException("Animations must have at least 2 frames");
            _Frames = frames;
        }


        // Methods #########################################################################
        /// <summary>
        /// Updates the Animation and its applied Roles.
        /// </summary>
        /// <param name="deltaT">Duration of the last frame</param>
        public override void Update(float deltaT)
        {
            base.Update(deltaT);
            _FrameTime -= deltaT;
            while (_FrameTime <= 0)
            {
                _FrameTime += FrameDuration;
                if (++_CurrentFrame >= _Frames.Length) _CurrentFrame = 0;
                Texture = _Frames[_CurrentFrame];
            }
        }
    }
}