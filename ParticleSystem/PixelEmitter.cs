using SFML.Graphics;

namespace BlackCoat.ParticleSystem
{
    /// <summary>
    /// Basic Emitter that emits particles composed exclusively of single pixels, all using Alpha Blending.
    /// </summary>
    public sealed class PixelEmitter : Emitter<PixelParticle, PixelParticleInitializationInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PixelEmitter" /> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="info">The optional particle animation information.</param>
        /// <param name="depth">The optional hierarchical depth.</param>
        public PixelEmitter(Core core, PixelParticleInitializationInfo info, int depth = 0) :
                            base(core, info, PrimitiveType.Points, BlendMode.Alpha, null, depth)
        {
        }
    }
}