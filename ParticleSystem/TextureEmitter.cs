﻿using SFML.System;
using SFML.Graphics;

namespace BlackCoat.ParticleSystem
{
    /// <summary>
    /// Abstract base class for all texture based emitter classes.
    /// </summary>
    /// <seealso cref="BlackCoat.ParticleSystem.PixelEmitter" />
    public abstract class TextureEmitter : BaseEmitter
    {
        public Texture Texture { get; private set; }
        public BlendMode BlendMode { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureEmitter"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="texture">The texture for all particles.</param>
        /// <param name="blendMode">The particle blend mode.</param>
        /// <param name="depth">The optional hierarchical depth.</param>
        public TextureEmitter(Core core, Texture texture, BlendMode blendMode, int depth = 0) : base(core, depth)
        {
            Texture = texture;
            BlendMode = blendMode;
        }
    }
}