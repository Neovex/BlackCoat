using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace BlackCoat.ParticleSystem
{
    /// <summary>
    /// Abstract base class of all particle renderer.
    /// </summary>
    /// <seealso cref="BlackCoat.BlackCoatBase" />
    public abstract class VertexRenderer : BlackCoatBase
    {
        private const int _GROWTH_MULTIPLIER = 1000;

        private readonly int _GroupSize;
        private Stack<int> _FreeIndices;

        internal Vertex[] Verticies;

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexRenderer"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="groupSize">Size of the vertex group per particle.</param>
        public VertexRenderer(Core core, int groupSize) : base(core)
        {
            _GroupSize = groupSize;
            _FreeIndices = new Stack<int>();
            Verticies = new Vertex[0];
        }

        /// <summary>
        /// Reserves a set of vertices.
        /// </summary>
        /// <returns>Index of the first vertex</returns>
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
                ClearVertices(oldSize + _GroupSize, (newSize - oldSize) - _GroupSize);
                return oldSize;
            }
            else
            {
                return _FreeIndices.Pop();
            }
        }

        /// <summary>
        /// Frees the specified vertex.
        /// </summary>
        /// <param name="index">The index identifying the vertex.</param>
        internal void Free(int index)
        {
            _FreeIndices.Push(index);
        }

        /// <summary>
        /// Clears the vertices.
        /// </summary>
        /// <param name="index">The index identifying the first vertex.</param>
        /// <param name="groupSize">Size of the vertex group per particle.</param>
        private void ClearVertices(int index, int groupSize)
        {
            for (int i = index; i < index + groupSize; i++)
            {
                Verticies[i].Color.A = 0;
            }
        }

        /// <summary>
        /// Draws all active vertices on to the defined render target.
        /// </summary>
        /// <param name="target">The render target.</param>
        /// <param name="states">Additional render information.</param>
        public abstract void Draw(RenderTarget target, RenderStates states);
    }
}