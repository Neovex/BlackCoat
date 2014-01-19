using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackCoat.Entities.Animation
{
    /// <summary>
    /// Animation Graphic Item using one Texture per Frame
    /// </summary>
    public class FrameAnimation:GraphicItem
    {
        // Variables #######################################################################
        protected Int32 _CurrentFrame = 0;
        protected Texture[] _Frames;

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


        // CTOR ############################################################################
        public FrameAnimation(Core core, Texture[] frames) : base(core)
        {
            if (frames == null || frames.Length < 2) throw new NullReferenceException("FrameAnimation must have at least 2 frames");
            _Frames = frames;
        }


        // Methods #########################################################################
        /// <summary>
        /// Updates the Animation and its applied Roles.
        /// </summary>
        /// <param name="deltaT">Current gametime</param>
        public override void Update(float deltaT)
        {
            _CurrentTime -= deltaT;
            if (_CurrentTime < 0)
            {
                _CurrentTime = _AnimationTime;
                Texture = _Frames[_CurrentFrame++];
                if (_CurrentFrame >= _Frames.Length) _CurrentFrame = 0;
            }
            base.Update(deltaT);
        }
    }
}