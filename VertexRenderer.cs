using SFML.Graphics;

namespace BlackCoat
{
    /// <summary>
    /// Low level vertex renderer
    /// </summary>
    /// <seealso cref="BlackCoat.BlackCoatBase" />
    public class VertexRenderer : BlackCoatBase
    {
        protected internal Vertex[] Verticies;


        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        public bool IsEmpty => Verticies.Length == 0;
        /// <summary>
        /// Texture associated with this layer.
        /// </summary>
        public Texture Texture { get; protected set; }
        /// <summary>
        /// Gets or sets the blend mode of this layer.
        /// </summary>
        public BlendMode BlendMode { get; protected set; }
        /// <summary>
        /// Gets the type of the primitive.
        /// </summary>
        public PrimitiveType PrimitiveType { get; protected set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="VertexRenderer" /> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="primitiveType">Type of the render primitive.</param>
        /// <param name="blendMode">The blend mode.</param>
        /// <param name="texture">Optional texture that will be mapped onto the vertices.</param>
        /// <exception cref="NotSupportedException">PrimitiveTypes TrianglesStrip, TrianglesFan, LinesStrip</exception>
        public VertexRenderer(Core core, PrimitiveType primitiveType, BlendMode blendMode, Texture texture = null) : base(core)
        {
            Verticies = new Vertex[0];
            PrimitiveType = primitiveType;
            BlendMode = blendMode;
            Texture = texture;
        }

        /// <summary>
        /// Draws the vertices on to the defined render target.
        /// </summary>
        /// <param name="target">The render target.</param>
        /// <param name="states">Additional render information.</param>
        public virtual void Draw(RenderTarget target, RenderStates states)
        {
            states.Texture = Texture;
            states.BlendMode = BlendMode;
            target.Draw(Verticies, PrimitiveType, states);
        }
    }
}