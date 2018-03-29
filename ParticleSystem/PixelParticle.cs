using SFML.Graphics;

namespace BlackCoat.ParticleSystem
{
    public abstract class PixelParticle : BaseParticle
    {
        public PixelParticle(Core core) : base(core)
        {
        }

        override protected unsafe void Clear(Vertex* vPtr)
        {
            vPtr->Color = Color.Transparent;
        }

        override protected unsafe bool UpdateInternal(float deltaT, Vertex* vPtr)
        {
            vPtr->Position = _Position;
            vPtr->Color = _Color;
            return false;
        }
    }
}