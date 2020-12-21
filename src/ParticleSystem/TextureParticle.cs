using System;
using SFML.System;
using SFML.Graphics;

namespace BlackCoat.ParticleSystem
{
    /// <summary>
    /// Simple Texture Particle Implementation. Supports simple movement and blending.
    /// </summary>
    /// <seealso cref="BlackCoat.ParticleSystem.TextureParticleBase" />
    public class TextureParticle : TextureParticleBase, IInitializableByInfo<TexturedSpawnInfo>
    {
        // Variables #######################################################################
        protected Vector2f _Velocity;
        protected Vector2f _Acceleration;
        protected Single _AlphaFade;
        protected Boolean _UseAlphaAsTTL;
        protected Single _RotationVelocity;
        protected Vector2f _ScaleVelocity;


        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="TextureParticle"/> class.
        /// </summary>
        /// <param name="core">The Engine core.</param>
        public TextureParticle(Core core) : base(core)
        {
        }


        // Methods #########################################################################
        /// <summary>
        /// Initializes the particle with the provided animation parameters.
        /// </summary>
        public virtual void Initialize(Vector2f position, TexturedSpawnInfo info)
        {
            // Init particle
            _TextureSize = info.Texture.Size;
            _TextureRect.Width = (int)_TextureSize.X;
            _TextureRect.Height = (int)_TextureSize.Y;
            _Position = position + info.Offset;
            _Origin = info.Origin;
            _Scale = info.Scale;
            _Rotation = info.Rotation;
            _Color = info.Color;
            _Alpha = info.Alpha;

            // Init movement
            _Velocity = info.Velocity;
            _Acceleration = info.Acceleration;
            _RotationVelocity = info.RotationVelocity;
            _AlphaFade = info.AlphaFade;
            _UseAlphaAsTTL = info.UseAlphaAsTTL;
            _ScaleVelocity = info.ScaleVelocity;
        }

        /// <summary>
        /// Updates the particle animation.
        /// </summary>
        /// <param name="deltaT">Current Frame Time.</param>
        /// <param name="vPtr">First vertex of this particle</param>
        /// <returns>True if the particle needs to be removed otherwise false.</returns>
        override protected unsafe bool UpdateInternal(float deltaT, Vertex* vPtr)
        {
            _Velocity += _Acceleration * deltaT;
            _Position += _Velocity * deltaT;
            _Alpha += _AlphaFade * deltaT;
            _Rotation += _RotationVelocity * deltaT;
            _Scale += _ScaleVelocity * deltaT;
            return base.UpdateInternal(deltaT, vPtr) || (_UseAlphaAsTTL && _Alpha <= 0);
        }
    }
}