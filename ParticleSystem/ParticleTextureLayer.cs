using SFML.Graphics;

namespace BlackCoat.ParticleSystem
{
    /// <summary>
    /// Defines the second render hierarchy after the layer depth inside a <see cref="ParticleEmitterHost"/>.
    /// </summary>
    /// <seealso cref="BlackCoat.ParticleSystem.VertexRenderer" />
    internal class ParticleTextureLayer : VertexRenderer
    {
        /// <summary>
        /// Texture associated with this layer.
        /// </summary>
        public Texture Texture { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        public bool IsEmpty => Verticies.Length == 0;
        /// <summary>
        /// Gets or sets the blend mode of this layer.
        /// </summary>
        public BlendMode BlendMode { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="ParticleTextureLayer"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="texture">The texture associated with this layer.</param>
        /// <param name="blendMode">The blend mode this layer will be rendered in.</param>
        public ParticleTextureLayer(Core core, Texture texture, BlendMode blendMode) : base(core, 4)
        {
            Texture = texture;
            BlendMode = blendMode;
        }


        /// <summary>
        /// Adds the specified emitter.
        /// </summary>
        /// <param name="emitter">The emitter to be added.</param>
        public void Add(TextureEmitter emitter)
        {
            emitter.Initialize(this);
        }

        /// <summary>
        /// Removes the specified emitter.
        /// </summary>
        /// <param name="emitter">The emitter to be removed.</param>
        public void Remove(TextureEmitter emitter)
        {
            emitter.Cleanup();
        }

        /// <summary>
        /// Draws all active vertices on to the defined render target.
        /// </summary>
        /// <param name="target">The render target.</param>
        /// <param name="states">Additional render information.</param>
        public override void Draw(RenderTarget target, RenderStates states)
        {
            states.Texture = Texture;
            states.BlendMode = BlendMode;
            target.Draw(Verticies, PrimitiveType.Quads, states);
        }
    }
}