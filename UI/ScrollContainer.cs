using System.Linq;
using SFML.System;
using SFML.Graphics;
using BlackCoat;
using BlackCoat.Entities;
using System;

namespace BlackCoat.UI
{
    /// <summary>
    /// The <see cref="ScrollContainer"/> works just like a <see cref="Canvas"/> with the difference that all its child elements will be drawn
    /// onto its texture instead of the default <see cref="RenderTarget"/> of the <see cref="Core"/>. This way properly cropped scrolling can be achieved.
    /// </summary>
    /// <seealso cref="BlackCoat.UI.Canvas" />
    public class ScrollContainer : Canvas
    {
        // Statics #########################################################################
        public static int SCROLLSPEED = 25;



        // Variables #######################################################################
        private RenderTexture _RenderTarget;
        private Vector2u _BufferSize;


        // Properties ######################################################################
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
        /// Initializes a new instance of the <see cref="ScrollContainer"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="fixedSize">Optional size of the container.</param>
        public ScrollContainer(Core core, Vector2f? size = null) : base(core, size)
        {
            _BufferSize = MinSize.ToVector2u();
            _RenderTarget = new RenderTexture(_BufferSize.X, _BufferSize.Y);
            ClearColor = PrerenderedContainer.DEFAULT_CLEAR_COLOR;
        }


        // Methods #########################################################################
        protected override void InvokeSizeChanged()
        {
            // Calculate necessary buffer size
            var newSize = new Vector2u(Math.Max((uint)InnerSize.X, (uint)MinSize.X),
                                       Math.Max((uint)InnerSize.Y, (uint)MinSize.Y));
            var components = Components.Select(co => co.RelativeSize).ToArray();
            if (components.Length != 0)
            {
                newSize = new Vector2u(Math.Max(newSize.X, (uint)components.Max(v => v.X)),
                                       Math.Max(newSize.Y, (uint)components.Max(v => v.Y)));
            }
            if (_BufferSize != newSize)
            {
                _BufferSize = newSize;
                // Recreate render target
                _RenderTarget?.Dispose();
                _RenderTarget = new RenderTexture(Math.Min(Texture.MaximumSize, _BufferSize.X),
                                                  Math.Min(Texture.MaximumSize, _BufferSize.Y));
                Texture = _RenderTarget.Texture;
                // Update entities
                foreach (var entity in _Entities) entity.RenderTarget = _RenderTarget;
            }
            TextureRect = new IntRect(TextureRect.Position(), InnerSize.ToVector2i());
            base.InvokeSizeChanged();
        }

        protected override void HandleMouseWheelScrolled(float delta)
        {
            if (!CollisionShape.Collide(Input.Input.MousePosition)) return;
            // Scroll
            var newPos = TextureRect.Position() + new Vector2i(0, SCROLLSPEED * (delta < 0 ? 1 : -1));
            //Limit
            if (newPos.Y < 0) newPos.Y = 0;
            else if (newPos.Y + TextureRect.Height > Texture.Size.Y) newPos.Y = (int)Texture.Size.Y - TextureRect.Height;
            //Assign
            TextureRect = new IntRect(newPos, TextureRect.Size());
        }

        /// <summary>
        /// Adds an Entity to this Container
        /// </summary>
        /// <param name="entity">The Entity to add</param>
        /// <returns>True if the Entity could be added</returns>
        public override void Add(IEntity entity)
        {
            base.Add(entity);
            entity.RenderTarget = _RenderTarget;
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
        /// Draws the current surface of the container. When <see cref="RedrawEachFrame"/> is true all entities will be redrawn accordingly.
        /// </summary>
        public override void Draw()
        {
            if (!Visible) return;
            _RenderTarget.Clear(ClearColor);
            foreach (var entity in _Entities) entity.Draw();
            _RenderTarget.Display();
            Core.DRAW_CALLS++;

            _Core.Draw(this);
        }

        /// <summary>
        /// Handles the destruction of the object
        /// </summary>
        /// <param name="disposing">Determines if the GC is disposing the object (true), or it's an explicit call (false).</param>
        protected override void Destroy(bool disposing)
        {
            _RenderTarget.Dispose();
            _RenderTarget = null;
            base.Destroy(disposing);
        }
    }
}