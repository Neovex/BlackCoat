using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;

namespace BlackCoat.Entities.Animation
{
    /// <summary>
    /// Animated Graphic Entity. Utilizes a single Texture for all frames of the animation.
    /// </summary>
    public class BlittingAnimation:Graphic
    {
        // Events ##########################################################################
        /// <summary>
        /// Occurs when the animation completes and returns to its first frame.
        /// </summary>
        public event Action AnimationComplete = () => { };


        // Variables #######################################################################
        protected Int32 _CurrentFrame;
        protected IntRect[] _Frames;
        protected Single _FrameTime;


        // Properties ######################################################################
        /// <summary>
        /// Duration of one frame
        /// </summary>
        public virtual Single FrameDuration { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="BlittingAnimation"/> is paused.
        /// </summary>
        public virtual Boolean Paused { get; set; }

        /// <summary>
        /// Gets or sets the index of current frame.
        /// </summary>
        public int CurrentFrame
        {
            get => _CurrentFrame;
            set => TextureRect = _Frames[_CurrentFrame = value];
        }


        // CTOR ############################################################################
        /// <summary>
        /// Creates a new Instance of the BlittingAnimation class.
        /// </summary>
        /// <param name="core">Engine Core</param>
        /// <param name="frameDuration">Duration of one frame</param>
        /// <param name="texture">The texture containing all frames</param>
        /// <param name="frameSize">Size of a single frame inside the texture</param>
        public BlittingAnimation(Core core, Single frameDuration, Texture texture, Vector2u frameSize) : this(core, frameDuration, texture, CalculateFrames(texture, frameSize))
        {
        }

        /// <summary>
        /// Creates a new Instance of the BlittingAnimation class.
        /// </summary>
        /// <param name="core">Engine Core</param>
        /// <param name="frameDuration">Duration of one frame</param>
        /// <param name="texture">The texture containing all frames</param>
        /// <param name="frames">Determines the frame locations of the animation frames inside the texture</param>
        public BlittingAnimation(Core core, Single frameDuration, Texture texture, IntRect[] frames) : base(core)
        {
            FrameDuration = frameDuration;
            Texture = texture;
            _Frames = frames;
            CurrentFrame = 0;
        }


        // Methods #########################################################################
        /// <summary>
        /// Updates the Animation and its applied Roles.
        /// </summary>
        /// <param name="deltaT">Current game time</param>
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
                    CurrentFrame = 0;
                    AnimationComplete.Invoke();
                }
                else
                {
                    CurrentFrame++;
                }
            }
        }

        /// <summary>
        /// Rebuilds the animation frame info based on the current texture
        /// </summary>
        protected static IntRect[] CalculateFrames(Texture texture, Vector2u frameSize)
        {
            if (texture == null) throw new ArgumentNullException(nameof(texture));
            if (frameSize.X == 0 || frameSize.Y == 0) throw new ArgumentException(nameof(frameSize));

            var list = new List<IntRect>();
            for (UInt32 y = 0; y < texture.Size.Y; y += frameSize.Y)
            {
                for (UInt32 x = 0; x < texture.Size.X; x += frameSize.X)
                {
                    list.Add(new IntRect((Int32)x, (Int32)y, (Int32)frameSize.X, (Int32)frameSize.Y));
                }
            }
            return list.ToArray();
        }
    }
}