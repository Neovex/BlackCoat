using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace BlackCoat.ParticleSystem
{
    /// <summary>
    /// Vertex renderer for vertices of particles
    /// </summary>
    /// <seealso cref="BlackCoat.BlackCoatBase" />
    public sealed class ParticleVertexRenderer : BlackCoatBase
    {
        // Constants #######################################################################
        private const int _GROWTH_MULTIPLIER = 1000;


        // Variables #######################################################################
        private readonly Stack<int> _FreeIndexes;
        private readonly int _GroupSize;
        internal Vertex[] Verticies;


        // Properties ######################################################################
        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        public bool IsEmpty => Verticies.Length == 0;
        /// <summary>
        /// Texture associated with this layer.
        /// </summary>
        public Texture Texture { get; }
        /// <summary>
        /// Gets or sets the blend mode of this layer.
        /// </summary>
        public BlendMode BlendMode { get; }
        /// <summary>
        /// Gets the type of the primitive.
        /// </summary>
        public PrimitiveType PrimitiveType { get; }
        /// <summary>
        /// Gets or sets the amount of associated emitters.
        /// </summary>
        public int AssociatedEmitterCount { get; internal set; }


        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="ParticleVertexRenderer" /> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="primitiveType">Type of the render primitive.</param>
        /// <param name="blendMode">The blend mode.</param>
        /// <param name="texture">Optional texture that will be mapped onto the vertices.</param>
        /// <exception cref="NotSupportedException">PrimitiveTypes TrianglesStrip, TrianglesFan, LinesStrip</exception>
        public ParticleVertexRenderer(Core core, PrimitiveType primitiveType, BlendMode blendMode, Texture texture = null) : base(core)
        {
            Verticies = new Vertex[0];
            PrimitiveType = primitiveType;
            BlendMode = blendMode;
            Texture = texture;
            _FreeIndexes = new Stack<int>();

            switch (PrimitiveType)
            {
                case PrimitiveType.Points:
                    _GroupSize = 1;
                    break;
                case PrimitiveType.Lines:
                    _GroupSize = 2;
                    break;
                case PrimitiveType.Triangles:
                    _GroupSize = 3;
                    break;
                case PrimitiveType.Quads:
                    _GroupSize = 4;
                    break;
                case PrimitiveType.TriangleStrip:
                case PrimitiveType.TriangleFan:
                case PrimitiveType.LineStrip:
                    throw new NotSupportedException($"The {nameof(SFML.Graphics.PrimitiveType)} \"{PrimitiveType}\" is not supported.");
            }
        }


        // Methods #########################################################################
        /// <summary>
        /// Draws the vertices on to the defined render target.
        /// </summary>
        /// <param name="target">The render target.</param>
        /// <param name="states">Additional render information.</param>
        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Texture = Texture;
            states.BlendMode = BlendMode;
            target.Draw(Verticies, PrimitiveType, states);
        }

        /// <summary>
        /// Reserves a set of vertices.
        /// </summary>
        /// <returns>Index of the first vertex</returns>
        public int Reserve()
        {
            if (_FreeIndexes.Count == 0)
            {
                // Resize Vertices Array
                var oldSize = Verticies.Length;
                var newSize = oldSize + _GroupSize * _GROWTH_MULTIPLIER;
                Array.Resize(ref Verticies, newSize);
                // Add new indexes
                for (int i = oldSize + _GroupSize; i < Verticies.Length; i += _GroupSize)
                {
                    _FreeIndexes.Push(i);
                }
                // Clear new vertices
                var index = oldSize + _GroupSize;
                var amount = (newSize - oldSize) - _GroupSize;
                for (int i = index; i < index + amount; i++)
                {
                    Verticies[i].Color.A = 0;
                }
                return oldSize; // old size is the first free index
            }
            else
            {
                return _FreeIndexes.Pop();
            }
        }

        /// <summary>
        /// Frees the specified vertex.
        /// </summary>
        /// <param name="index">The index identifying the vertex.</param>
        internal void Free(int index)
        {
            _FreeIndexes.Push(index);
        }

        /// <summary>
        /// Determines whether this <see cref="ParticleVertexRenderer"/> is compatible with the specified emitter.
        /// </summary>
        /// <param name="emitter">The emitter to validate.</param>
        /// <returns><c>true</c> when compatible; otherwise, <c>false</c>.</returns>
        internal bool IsCompatibleWith(EmitterBase emitter)
        {
            return PrimitiveType == emitter.PrimitiveType && BlendMode == emitter.BlendMode && Texture == emitter.Texture;
        }
    }
}