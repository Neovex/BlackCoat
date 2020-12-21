using System;
using SFML.Graphics;
using SFML.System;

namespace BlackCoat.ParticleSystem
{
    /// <summary>
    /// Simple Pixel Particle Implementation. Supports simple movement and alpha blending.
    /// </summary>
    /// <seealso cref="BlackCoat.ParticleSystem.PixelParticleBase" />
    public class PixelParticle : PixelParticleBase, IInitializableByInfo<ParticleSpawnInfo>
    {
        // Variables #######################################################################
        protected Vector2f _Velocity;
        protected Vector2f _Acceleration;
        protected Single _AlphaFade;
        protected Boolean _UseAlphaAsTTL;


        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="PixelParticle"/> class.
        /// </summary>
        /// <param name="core">The Engine core.</param>
        public PixelParticle(Core core) : base(core)
        {
        }


        // Methods #########################################################################
        /// <summary>
        /// Initializes the particle with the provided animation parameters.
        /// </summary>
        public virtual void Initialize(Vector2f position, ParticleSpawnInfo info)
        {
            // init particle
            _Position = position + info.Offset;
            _Color = info.Color;
            _Alpha = info.Alpha;

            // init movement
            _Velocity = info.Velocity;
            _Acceleration = info.Acceleration;
            _AlphaFade = info.AlphaFade;
            _UseAlphaAsTTL = info.UseAlphaAsTTL;
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
            _Alpha += _AlphaFade * deltaT;
            return base.UpdateInternal(deltaT, vPtr) || (_UseAlphaAsTTL && _Alpha <= 0);
        }
    }
}