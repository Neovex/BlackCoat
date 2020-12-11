using SFML.Graphics;

namespace BlackCoat.ParticleSystem
{
    /// <summary>
    /// Abstract base class of all pixel based Particles.
    /// </summary>
    /// <seealso cref="BlackCoat.ParticleSystem.ParticleBase" />
    public abstract class PixelParticleBase : ParticleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PixelParticleBase"/> class.
        /// </summary>
        /// <param name="core">The Engine core.</param>
        public PixelParticleBase(Core core) : base(core)
        {
        }

        /// <summary>
        /// Resets the used vertices into a neutral/reusable state.
        /// </summary>
        /// <param name="vPtr">First vertex of this particle</param>
        override protected unsafe void Clear(Vertex* vPtr)
        {
            vPtr->Color = Color.Transparent;
        }

        /// <summary>
        /// Updates the particle with the behavior defined by inherited classes.
        /// </summary>
        /// <param name="deltaT">Current Frame Time.</param>
        /// <param name="vPtr">First vertex of this particle</param>
        /// <returns>True if the particle needs to be removed otherwise false.</returns>
        override protected unsafe bool UpdateInternal(float deltaT, Vertex* vPtr)
        {
            vPtr->Position = _Position;
            vPtr->Color = _Color; // Alpha component is updated in base class
            return false;
        }
    }
}