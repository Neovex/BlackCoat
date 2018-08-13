using SFML.Graphics;

namespace BlackCoat.ParticleSystem
{
    /// <summary>
    /// Abstract base class for pixel based Emitters
    /// </summary>
    /// <seealso cref="BlackCoat.ParticleSystem.BaseEmitter" />
    public abstract class PixelEmitter : BaseEmitter
    {
        /// <summary>
        /// Gets or sets the color for the particles.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Parent Emitter when part of a composition
        /// </summary>
        public CompositeEmitter Composition { get; internal set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="PixelEmitter"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="depth">The optional hierarchical depth.</param>
        public PixelEmitter(Core core, int depth = 0):base(core, depth)
        {
            Color = Color.White;
        }
    }
}