using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace BlackCoat.ParticleSystem
{
    public abstract class VertexRenderer : BlackCoatBase
    {
        private const int _GROWTH_MULTIPLIER = 1000;

        private readonly int _GroupSize;
        private Stack<int> _FreeIndices;

        internal Vertex[] Verticies;


        public VertexRenderer(Core core, int groupSize) : base(core)
        {
            _GroupSize = groupSize;
            _FreeIndices = new Stack<int>();
            Verticies = new Vertex[0];
        }

        public int Reserve()
        {
            if (_FreeIndices.Count == 0)
            {
                var oldSize = Verticies.Length;
                var newSize = oldSize + _GroupSize * _GROWTH_MULTIPLIER;
                Array.Resize(ref Verticies, newSize);
                for (int i = oldSize + _GroupSize; i < Verticies.Length; i += _GroupSize)
                {
                    _FreeIndices.Push(i);
                }
                ClearVerticies(oldSize + _GroupSize, (newSize - oldSize) - _GroupSize);
                return oldSize;
            }
            else
            {
                return _FreeIndices.Pop();
            }
        }

        internal void Free(int index)
        {
            _FreeIndices.Push(index);
        }

        private void ClearVerticies(int first, int ammount)
        {
            for (int i = first; i < first + ammount; i++)
            {
                Verticies[i].Color.A = 0;
            }
        }

        public abstract void Draw(RenderTarget target, RenderStates states);
    }
}
