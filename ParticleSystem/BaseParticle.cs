using System;
using SFML.System;
using SFML.Graphics;

namespace BlackCoat.ParticleSystem
{
    public abstract class BaseParticle : BlackCoatBase
    {
        internal static int _PARTICLES = 0;

        private float _TTL;
        private int _Index;
        protected Vector2f _Position;
        protected Color _Color;
        protected float _Alpha;


        internal int Index => _Index;


        public BaseParticle(Core core) : base(core)
        {
            _Index = -1;
            _Color = Color.White;
            _Alpha = 1f;
        }


        internal void Initialize(int index, float ttl)
        {
            _PARTICLES++;
            _Index = index;
            _TTL = ttl;
        }

        internal unsafe void Release(Vertex* vPtr)
        {
            Clear(vPtr + _Index);
            _PARTICLES--;
            _Index = -1;
            _TTL = -1;
        }

        protected abstract unsafe void Clear(Vertex* vPtr);
        protected abstract unsafe bool UpdateInternal(float deltaT, Vertex* vPtr);

        internal unsafe bool Update(float deltaT, Vertex* vPtr)
        {
            _Color.A = (byte)(_Alpha * Byte.MaxValue);
            return (_TTL -= deltaT) < 0 || UpdateInternal(deltaT, vPtr + _Index);
        }
    }
}