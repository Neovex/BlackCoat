using System;
using SFML.Graphics;

namespace BlackCoat.ParticleSystem
{
    /// <summary>
    /// Very basic Emitter that continuously emits texture particles when triggered.
    /// </summary>
    /// <seealso cref="BlackCoat.ParticleSystem.ITriggerEmitter" />
    /// <seealso cref="BlackCoat.ParticleSystem.EmitterBase" />
    public sealed class TextureEmitter : Emitter<TextureParticle, TextureParticleInitializationInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextureEmitter" /> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="info">Particle animation information.</param>
        /// <param name="blendMode">Optional particle blend mode. Defaults to Alpha Blending.</param>
        /// <param name="depth">The optional hierarchical depth.</param>
        public TextureEmitter(Core core, TextureParticleInitializationInfo info, BlendMode? blendMode = null, int depth = 0) :
                                   base(core, info, PrimitiveType.Quads, blendMode ?? BlendMode.Alpha, info.Texture, depth)
        {
        }
    }
}