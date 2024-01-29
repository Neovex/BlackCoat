using System;
using SFML.Graphics;

namespace BlackCoat.Entities.Animation
{
    /// <summary>
    /// Animated Graphic Entity. Requires one Texture per Frame.
    /// </summary>
    public class FrameAnimation:Graphic
    {
        /// <summary>
        /// Occurs when the animation completes and returns to its first frame.
        /// </summary>
        public event Action AnimationComplete = () => { };


        protected Int32 _CurrentFrame = -1;
        protected Texture[] _Frames;
        protected Single _FrameTime;


        /// <summary>
        /// Duration of each frame
        /// </summary>
        public virtual Single FrameDuration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="BlittingAnimation"/> is paused.
        /// </summary>
        public virtual Boolean Paused { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="BlittingAnimation"/> is looping its frames.
        /// </summary>
        public virtual bool Loop { get; set; } = true;

        /// <summary>
        /// Gets or sets the index of current frame.
        /// </summary>
        public int CurrentFrame
        {
            get => _CurrentFrame;
            set => Texture = _Frames[_CurrentFrame = value];
        }


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
            CurrentFrame = 0;
        }


        /// <summary>
        /// Updates the Animation and its applied Roles.
        /// </summary>
        /// <param name="deltaT">Duration of the last frame</param>
        public override void Update(float deltaT)
        {
            base.Update(deltaT);
            if (Paused) return;
            _FrameTime -= deltaT;
            while (_FrameTime <= 0)
            {
                _FrameTime += FrameDuration;
                if (CurrentFrame + 1 >= _Frames.Length)
                {
                    AnimationComplete.Invoke();
                    if (Loop) CurrentFrame = 0;
                    else Paused = true;
                }
                else
                {
                    CurrentFrame++;
                }
            }
        }
    }
}