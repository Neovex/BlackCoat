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
        protected Single _Blending;


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
        public virtual void Initialize(Texture texture, Vector2f position, Vector2f origin, Vector2f scale, float rotation, Color color, Single alpha,
                                       Vector2f velocity, Vector2f acceleration, Single rotationVelocity, Single blending)
        {
            // init base class properties
            _Texture = texture;
            _TextureRect.Width = (int)_Texture.Size.X;
            _TextureRect.Height = (int)_Texture.Size.Y;
            _Position = position;
            _Origin = origin;
            _Scale = scale;
            _Rotation = rotation; // todo: simplify - to much mods for this class?
            _Color = color;
            _Alpha = alpha;

            // init movement
            _Velocity = velocity;
            _Acceleration = acceleration;
            _RotationVelocity = rotationVelocity;
            _Blending = blending;
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
            _Rotation += _RotationVelocity * deltaT;
            _Alpha += _Blending * deltaT;
            return base.UpdateInternal(deltaT, vPtr);
        }
    }
}