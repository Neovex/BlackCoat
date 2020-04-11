using SFML.System;
using SFML.Graphics;

namespace BlackCoat.Entities
{
    /// <summary>
    /// The <see cref="PrerenderedContainer"/> works just like a <see cref="Container"/> with the difference that all its child elements will be drawn
    /// onto its texture instead of the default <see cref="RenderTarget"/> of the <see cref="Core"/>. This way it can be used to reduce draw calls or 
    /// create blending effects that weren't possible otherwise.
    /// </summary>
    /// <seealso cref="BlackCoat.Entities.Container" />
    public class PrerenderedContainer : Container
    {
        // Statics #########################################################################
        public static readonly Color DEFAULT_CLEAR_COLOR = new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);


        // Variables #######################################################################
        private RenderTexture _RenderTarget;


        // Properties ######################################################################
        /// <summary>
        /// Gets a value indicating whether this <see cref="PrerenderedContainer"/> has a fixed size.
        /// </summary>
        public bool FixedSize { get; }
        /// <summary>
        /// Gets or sets a value indicating whether child elements should be redrawn each frame.
        /// Be wary: setting this to true will defeat any performance increase purposes.
        /// </summary>
        public bool RenderEachFrame { get; set; }
        /// <summary>
        /// The <see cref="Color"/> to fill the <see cref="Texture"/> with before all child elements will drawn on top.
        /// </summary>
        public Color ClearColor { get; set; }
        /// <summary>
        /// Transformation Matrix defining Position, Scale and Rotation of the Entity within the scene graph
        /// </summary>
        public override Transform GlobalTransform => Transform.Identity;
        /// <summary>
        /// Global Alpha Visibility according to the scene graph
        /// </summary>
        public override float GlobalAlpha => 1;


        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="PrerenderedContainer"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="fixedSize">Optional fixed size of the container. When this value is not set the <see cref="PrerenderedContainer"/> will automatically follow <see cref="Core.DeviceSize"/>.</param>
        public PrerenderedContainer(Core core, Vector2f? fixedSize = null) : base(core)
        {
            if (FixedSize = fixedSize.HasValue)
            {
                Handle_DeviceResized(fixedSize.Value);
            }
            else
            {
                Handle_DeviceResized(_Core.DeviceSize);
                _Core.DeviceResized += Handle_DeviceResized;
            }

            RenderEachFrame = false;
            ClearColor = DEFAULT_CLEAR_COLOR;
        }


        // Methods #########################################################################
        /// <summary>
        /// Recreate internal render target with current device size.
        /// </summary>
        /// <param name="size">The size.</param>
        private void Handle_DeviceResized(Vector2f size)
        {
            _RenderTarget?.Dispose();
            _RenderTarget = new RenderTexture((uint)size.X, (uint)size.Y);
            Texture = _RenderTarget.Texture;
            TextureRect = new IntRect(default(Vector2i), size.ToVector2i());
            foreach (var entity in _Entities) entity.RenderTarget = _RenderTarget;
            RedrawNow();
        }

        /// <summary>
        /// Immediately renders all entities within this container onto its surface.
        /// </summary>
        public void RedrawNow()
        {
            if (!Visible) return;
            _RenderTarget.Clear(ClearColor);
            foreach (var entity in _Entities) entity.Draw();
            _RenderTarget.Display();
            Core.DRAW_CALLS++;
        }

        /// <summary>
        /// Adds an Entity to this Container
        /// </summary>
        /// <param name="entity">The Entity to add</param>
        /// <returns>True if the Entity could be added</returns>
        public override void Add(IEntity entity)
        {
            entity.RenderTarget = _RenderTarget;
            base.Add(entity);
        }

        /// <summary>
        /// Removes the provided Entity from the Container
        /// </summary>
        /// <param name="entity">The Entity to remove</param>
        public override void Remove(IEntity entity)
        {
            entity.RenderTarget = null;
            base.Remove(entity);
        }

        /// <summary>
        /// Draws the current surface of the container. When <see cref="RenderEachFrame"/> is true all entities will be redrawn accordingly.
        /// </summary>
        public override void Draw()
        {
            if (RenderEachFrame) RedrawNow();
            _Core.Draw(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!FixedSize) _Core.DeviceResized -= Handle_DeviceResized;
            _RenderTarget.Dispose();
            _RenderTarget = null;
            base.Dispose(disposing);
        }
    }
}