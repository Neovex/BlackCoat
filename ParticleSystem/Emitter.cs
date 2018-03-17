using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace BlackCoat.ParticleSystem
{
    class Emitter : PixelEmitter
    {
        public Texture Texture { get; set; }
        public virtual BlendMode BlendMode { get; set; }

        public Emitter(int depth, Texture texture, BlendMode blendMode) : base(depth)
        {
            Texture = texture;
            BlendMode = blendMode;
        }

    }
}