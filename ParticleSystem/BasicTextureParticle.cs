using System;
using SFML.Graphics;
using SFML.System;

namespace BlackCoat.ParticleSystem
{
    /// <summary>
    /// Simple Texture Particle Implementation. Supports simple movement and blending.
    /// </summary>
    /// <seealso cref="BlackCoat.ParticleSystem.TextureParticle" />
    public class BasicTextureParticle : TextureParticle
    {
        protected Vector2f _Velocity;
        protected Vector2f _Acceleration;
        protected Single _RotationVelocity;
        protected Single _AlphaFade;
        protected Vector2f _ScaleVelocity;
        protected Boolean _UseAlphaAsTTL;


        /// <summary>
        /// Initializes a new instance of the <see cref="BasicTextureParticle"/> class.
        /// </summary>
        /// <param name="core">The Engine core.</param>
        public BasicTextureParticle(Core core) : base(core)
        {
        }

        /// <summary>
        /// Initializes the particle with the provided animation parameters.
        /// </summary>
        public virtual void Initialize(Texture texture, Vector2f position, TextureParticleAnimationInfo info)
        {
            // init particle
            _Texture = texture;
            _TextureRect.Width = (int)_Texture.Size.X;
            _TextureRect.Height = (int)_Texture.Size.Y;
            _Position = position;
            _Origin = info.Origin;
            _Scale = info.Scale;
            _Rotation = info.Rotation;
            _Color = info.Color;
            _Alpha = info.Alpha;

            // init movement
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