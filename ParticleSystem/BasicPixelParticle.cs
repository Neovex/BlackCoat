using SFML.Graphics;
using SFML.System;

namespace BlackCoat.ParticleSystem
{
    /// <summary>
    /// Simple Pixel Particle Implementation. Supports simple movement and blending.
    /// </summary>
    /// <seealso cref="BlackCoat.ParticleSystem.PixelParticle" />
    public class BasicPixelParticle : PixelParticle
    {
        protected Vector2f _Velocity;
        protected Vector2f _Acceleration;


        /// <summary>
        /// Initializes a new instance of the <see cref="BasicPixelParticle"/> class.
        /// </summary>
        /// <param name="core">The Engine core.</param>
        public BasicPixelParticle(Core core) : base(core)
        {
        }


        /// <summary>
        /// Initializes the particle with the provided animation parameters.
        /// </summary>
        public virtual void Initialize(Vector2f position, Color color, Vector2f velocity, Vector2f acceleration)
        {
            _Position = position;
            _Color = color;
            _Velocity = velocity;
            _Acceleration = acceleration;
        }

        /// <summary>
        /// Updates the particle with the behavior defined by inherited classes.
        /// </summary>
        /// <param name="deltaT">Current Frame Time.</param>
        /// <param name="vPtr">First vertex of this particle</param>
        /// <returns>True if the particle needs to be removed otherwise false.</returns>
        override protected unsafe bool UpdateInternal(float deltaT, Vertex* vPtr)
        {
            _Velocity += _Acceleration * deltaT;
            _Position += _Velocity * deltaT;
            return base.UpdateInternal(deltaT, vPtr);
        }
    }
}