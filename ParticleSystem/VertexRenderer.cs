using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace BlackCoat.ParticleSystem
{
    public abstract class VertexRenderer
    {
        private const int _GROW_MULTIPLIER = 1000;

        private readonly int _GroupSize;
        private List<int> _FreeIndices;

        internal Vertex[] Verticies;


        public VertexRenderer(int groupSize)
        {
            _GroupSize = groupSize;
            _FreeIndices = new List<int>();
            Verticies = new Vertex[0];
        }
        
        public int Reserve()
        {
            if (_FreeIndices.Count == 0)
            {
                var oldSize = Verticies.Length;
                var newSize = oldSize + _GroupSize * _GROW_MULTIPLIER;
                Array.Resize(ref Verticies, newSize);
                for (int i = oldSize + _GroupSize; i < Verticies.Length; i += _GroupSize)
                {
                    _FreeIndices.Add(i);
                }
                ClearVerticies(oldSize + _GroupSize, (newSize-oldSize) - _GroupSize);
                return oldSize;
            }
            else
            {
                var index = _FreeIndices[0];
                _FreeIndices.RemoveAt(0);
                return index;
            }
        }

        public void Free(int index)
        {
            _FreeIndices.Add(index);
            ClearVerticies(index, _GroupSize);
        }

        private void ClearVerticies(int first, int ammount)
        {
            for (int i = first; i < first+ammount; i++) // performance?
            {
                Verticies[i].Color.A = 0;
            }
        }

        public abstract void Draw(RenderTarget target, RenderStates states);
    }
}
