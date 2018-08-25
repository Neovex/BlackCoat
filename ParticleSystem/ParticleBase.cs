using System;
using SFML.System;
using SFML.Graphics;

namespace BlackCoat.ParticleSystem
{
    /// <summary>
    /// Abstract base class of all Particle Classes using the Black Coat Particle System
    /// </summary>
    /// <seealso cref="BlackCoat.BlackCoatBase" />
    public abstract class ParticleBase : BlackCoatBase
    {
        internal static int _PARTICLES = 0;

        private float _TTL;
        private int _Index;
        protected Vector2f _Position;
        protected Color _Color;
        protected float _Alpha;

        /// <summary>
        /// Assigned particle index. Required by the <see cref="ParticleVertexRenderer"/>
        /// </summary>
        internal int Index => _Index;


        /// <summary>
        /// Initializes a new instance of the <see cref="ParticleBase"/> class.
        /// </summary>
        /// <param name="core">The Engine core.</param>
        public ParticleBase(Core core) : base(core)
        {
            _Index = -1;
            _Color = Color.White;
            _Alpha = 1f;
        }


        /// <summary>
        /// Initializes the <see cref="ParticleBase" />.
        /// </summary>
        /// <param name="index">The index of the first vertex.</param>
        /// <param name="ttl">The particles maximum lifetime.</param>
        internal void Initialize(int index, float ttl)
        {
            _PARTICLES++;
            _Index = index;
            _TTL = ttl;
        }

        /// <summary>
        /// Releases the <see cref="ParticleBase"/> clearing up all used recourses.
        /// </summary>
        /// <param name="vPtr">Root vertex of the <see cref="ParticleVertexRenderer"/></param>
        internal unsafe void Release(Vertex* vPtr)
        {
            Clear(vPtr + _Index);
            _PARTICLES--;
            _Index = -1;
            _TTL = -1;
        }

        /// <summary>
        /// Updates the particle with the behavior defined by inherited classes.
        /// </summary>
        /// <param name="deltaT">Current Frame Time.</param>
        /// <param name="vPtr">Root vertex of the <see cref="ParticleVertexRenderer"/></param>
        /// <returns>True if the particle needs to be removed otherwise false.</returns>
        internal unsafe bool Update(float deltaT, Vertex* vPtr)
        {
            _Color.A = (byte)(_Alpha * Byte.MaxValue);
            return (_TTL -= deltaT) < 0 || UpdateInternal(deltaT, vPtr + _Index);
        }

        /// <summary>
        /// Resets the used vertices into a neutral/reusable state.
        /// </summary>
        /// <param name="vPtr">First vertex of this particle</param>
        protected abstract unsafe void Clear(Vertex* vPtr);

        /// <summary>
        /// Updates the particle with the behavior defined by inherited classes.
        /// </summary>
        /// <param name="deltaT">Current Frame Time.</param>
        /// <param name="vPtr">First vertex of this particle</param>
        /// <returns>True if the particle needs to be removed otherwise false.</returns>
        protected abstract unsafe bool UpdateInternal(float deltaT, Vertex* vPtr);
    }
}