using SFML.Graphics;
using SFML.System;

namespace BlackCoat.ParticleSystem
{
    public abstract class PixelEmitter: BaseEmitter
    {
        /// <summary>
        /// Gets or sets the color for the particles.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Parent Emitter when part of a composition
        /// </summary>
        public CompositeEmitter Composition { get; internal set; }


        public PixelEmitter(Core core, int depth = 0):base(core, depth)
        {
            Color = Color.White;
        }
    }
}