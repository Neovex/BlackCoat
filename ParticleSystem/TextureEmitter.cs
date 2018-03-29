using SFML.System;
using SFML.Graphics;

namespace BlackCoat.ParticleSystem
{
    public abstract class TextureEmitter : PixelEmitter
    {
        public Texture Texture { get; private set; }
        public BlendMode BlendMode { get; private set; }
        public Vector2f Origin { get; set; }
        public Vector2f Scale { get; set; }
        public float Alpha { get; set; }

        public TextureEmitter(Core core, Texture texture, BlendMode blendMode, int depth = 0) : base(core, depth)
        {
            Texture = texture;
            BlendMode = blendMode;
            Scale = new Vector2f(1, 1);
            Alpha = 1f;
        }
    }
}