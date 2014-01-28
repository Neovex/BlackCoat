using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackCoat.Entities.Animation
{
    /// <summary>
    /// Animation Graphic Item using a single blitted Texture for all Frames
    /// </summary>
    public class BlittingAnimation:GraphicItem
    {
        // Variables #######################################################################
        protected Int32 _CurrentFrame = 0;
        protected IntRect[] _Frames;
        protected Vector2u _FrameSize;

        protected Single _AnimationTime;
        protected Single _CurrentTime;


        // Properties ######################################################################
        /// <summary>
        /// Duration of one frame
        /// </summary>
        public virtual Single AnimationTime
        {
            get { return _AnimationTime; }
            set { _AnimationTime = _CurrentTime = value; }
        }

        /// <summary>
        /// Texture of this Sprite
        /// </summary>
        public new Texture Texture
        {
            get { return base.Texture; }
            set
            {
                base.Texture = value;
                if (value == null || _FrameSize.X == 0 || _FrameSize.Y == 0) return;
                // Rebuild frames to match new texture
                var list = new List<IntRect>();
                for (UInt32 y = 0; y < value.Size.Y; y+=_FrameSize.Y)
                {
                    for (UInt32 x = 0; x < value.Size.X; x+=_FrameSize.X)
                    {
                        list.Add(new IntRect((Int32)x, (Int32)y, (Int32)_FrameSize.X, (Int32)_FrameSize.Y));
                    }
                }
                _Frames = list.ToArray();
            }
        }


        // CTOR ############################################################################
        public BlittingAnimation(Core core, Vector2u frameSize) : base(core) { _FrameSize = frameSize; }
        public BlittingAnimation(Core core, IntRect[] frames) : base(core) { _Frames = frames; }


        // Methods #########################################################################
        /// <summary>
        /// Updates the Animation and its applied Roles.
        /// </summary>
        /// <param name="deltaT">Current gametime</param>
        public override void Update(float deltaT)
        {
            base.Update(deltaT);
            _CurrentTime -= deltaT;
            if (_CurrentTime < 0)
            {
                _CurrentTime = _AnimationTime;
                if (++_CurrentFrame >= _Frames.Length) _CurrentFrame = 0;
                TextureRect = _Frames[_CurrentFrame];
            }
        }
    }
}