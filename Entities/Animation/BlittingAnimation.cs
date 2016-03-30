using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;

namespace BlackCoat.Entities.Animation
{
    /// <summary>
    /// Animated Graphic Entity. Utilizes a single blitted Texture for all Frames.
    /// </summary>
    public class BlittingAnimation:Graphic
    {
        // Variables #######################################################################
        protected Int32 _CurrentFrame = -1;
        protected IntRect[] _Frames;
        protected Vector2u _FrameSize;
        protected Single _FrameTime;


        // Properties ######################################################################
        /// <summary>
        /// Duration of one frame
        /// </summary>
        public virtual Single FrameDuration { get; set; }

        /// <summary>
        /// Texture of this Sprite
        /// </summary>
        public new Texture Texture
        {
            get { return base.Texture; }
            set
            {
                base.Texture = value;
                if (value == null || _FrameSize.X == 0 || _FrameSize.Y == 0) return; // TODO : extract to method
                // Rebuild frames to match new texture
                var list = new List<IntRect>();
                for (UInt32 y = 0; y < value.Size.Y; y += _FrameSize.Y)
                {
                    for (UInt32 x = 0; x < value.Size.X; x += _FrameSize.X)
                    {
                        list.Add(new IntRect((Int32)x, (Int32)y, (Int32)_FrameSize.X, (Int32)_FrameSize.Y));
                    }
                }
                _Frames = list.ToArray();
            }
        }


        // CTOR ############################################################################
        /// <summary>
        /// Creates a new Instance of the BlittingAnimation class.
        /// </summary>
        /// <param name="core">Engine Core</param>
        /// <param name="frameSize">Size of a single frame inside the blitted texture</param>
        public BlittingAnimation(Core core, Vector2u frameSize) : base(core)
        {
            _FrameSize = frameSize;
        }

        /// <summary>
        /// Creates a new Instance of the BlittingAnimation class.
        /// </summary>
        /// <param name="core">Engine Core</param>
        /// <param name="frames">Determines the frame locations of the animation frames inside the blitted texture</param>
        public BlittingAnimation(Core core, IntRect[] frames) : base(core)
        {
            _Frames = frames;
        }


        // Methods #########################################################################
        /// <summary>
        /// Updates the Animation and its applied Roles.
        /// </summary>
        /// <param name="deltaT">Current game time</param>
        public override void Update(float deltaT)
        {
            base.Update(deltaT);
            _FrameTime -= deltaT;
            while (_FrameTime <= 0)
            {
                _FrameTime += FrameDuration;
                if (++_CurrentFrame >= _Frames.Length) _CurrentFrame = 0;
                TextureRect = _Frames[_CurrentFrame];
            }
        }
    }
}