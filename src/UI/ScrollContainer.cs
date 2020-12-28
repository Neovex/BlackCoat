using System;
using System.Linq;
using System.Collections.Generic;

using SFML.System;
using SFML.Graphics;

using BlackCoat.Entities;

namespace BlackCoat.UI
{
    /// <summary>
    /// The <see cref="ScrollContainer"/> works just like a <see cref="Canvas"/> with the difference that all its child elements will be drawn
    /// onto its texture instead of the default <see cref="RenderTarget"/> of the <see cref="Core"/>. This way properly cropped scrolling is achieved.
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
        /// The <see cref="Color"/> to clear the render <see cref="Texture"/> before all child components will be drawn.
        /// </summary>
        public Color ClearColor { get; set; }

        /// <summary>
        /// Transformation Matrix defining Position, Scale and Rotation of the Entity within the scene graph
        /// </summary>
        public override Transform GlobalTransform => Transform.Identity;

        /// <summary>
        /// Global Alpha Visibility according to the scene graph
        /// </summary>
        public override float GlobalAlpha => 1f;



        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollContainer" /> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="size">The size of the display area.</param>
        /// <param name="components">Optional <see cref="UIComponent"/>s for functional construction.</param>
        public ScrollContainer(Core core, Vector2f? size = null, params UIComponent[] components) :
                          this(core, size, components as IEnumerable<UIComponent>)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollContainer" /> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="size">The size of the display area.</param>
        /// <param name="components">Optional enumeration of <see cref="UIComponent" />s for functional construction.</param>
        public ScrollContainer(Core core, Vector2f? size = null, IEnumerable<UIComponent> components = null) : base(core, size, components)
        {
            _BufferSize = MinSize.ToVector2u();
            _RenderTarget = new RenderTexture(_BufferSize.X, _BufferSize.Y);
            ClearColor = PrerenderedContainer.DEFAULT_CLEAR_COLOR;
        }


        // Methods #########################################################################
        /// <summary>
        /// Invokes the size changed event.
        /// </summary>
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

        /// <summary>
        /// Handles the mouse wheel scrolled event.
        /// </summary>
        /// <param name="direction">The scroll direction.</param>
        protected override void HandleMouseWheelScrolled(float direction)
        {
            if (!CollisionShape.CollidesWith(Input.Input.MousePosition)) return;
            // Scroll
            var newPos = TextureRect.Position() + (Create.Vector2fFromAngleLookup(direction) * SCROLLSPEED).ToVector2i();
            //Limit
            newPos.X = MathHelper.Clamp(newPos.X, 0, (int)Texture.Size.X - TextureRect.Width);
            newPos.Y = MathHelper.Clamp(newPos.Y, 0, (int)Texture.Size.Y - TextureRect.Height);
            //Assign
            TextureRect = new IntRect(newPos, TextureRect.Size());
        }

        /// <summary>
        /// Adds an Entity to this Container
        /// </summary>
        /// <param name="entity">The Entity to add</param>
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
        /// Releases unmanaged and managed resources.
        /// </summary>
        /// <param name="disposeManaged"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposeManaged)
        {
            _RenderTarget.Dispose();
            _RenderTarget = null;
            base.Dispose(disposeManaged);
        }
    }
}