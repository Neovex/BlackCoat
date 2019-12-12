using System;
using SFML.System;
using SFML.Graphics;
using BlackCoat.Collision;
using BlackCoat.Collision.Shapes;

namespace BlackCoat.Entities
{
    /// <summary>
    /// Represents a single texture <see cref="IEntity"/> in the Scene
    /// </summary>
    public class Graphic : Sprite, IEntity, ICollidable
    {
        // Variables #######################################################################
        protected Core _Core;
        private Container _Parent;
        private Boolean _Visible;
        private View _View;
        private float _Alpha;
        private RenderTarget _RenderTarget;
        private ICollisionShape _CollisionShape;


        // Properties ######################################################################
        /// <summary>
        /// Name of the <see cref="IEntity" />
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Parent Container of the <see cref="Graphic"/>
        /// </summary>
        public Container Parent
        {
            get => _Parent;
            set => _Parent = value == null || !value.Contains(this) ? value : _Parent;
        }

        /// <summary>
        /// Determines the Visibility of the <see cref="Graphic"/>
        /// </summary>
        public virtual Boolean Visible
        {
            get => _Visible && (_Parent == null || _Parent.Visible);
            set => _Visible = value;
        }

        /// <summary>
        /// Target Render View
        /// </summary>
        public virtual View View
        {
            get => _View ?? (Parent is PrerenderedContainer ? null : _Parent?.View);
            set => _View = value;
        }

        /// <summary>
        /// Alpha Visibility - 0-1f
        /// </summary>
        public virtual Single Alpha
        {
            get => _Alpha;
            set
            {
                _Alpha = value < 0 ? 0 : value > 1 ? 1 : value;
                var color = Color;
                color.A = (Byte)(GlobalAlpha * Byte.MaxValue);
                Color = color;
            }
        }
        /// <summary>
        /// Global Alpha Visibility according to the scene graph
        /// </summary>
        public virtual Single GlobalAlpha => _Alpha * (Parent == null ? 1 : _Parent.GlobalAlpha);

        /// <summary>
        /// Renderstate of the <see cref="Graphic"/> 
        /// </summary>
        public RenderStates RenderState { get; set; }

        /// <summary>
        /// Target device for rendering
        /// </summary>
        public RenderTarget RenderTarget
        {
            get => _RenderTarget ?? _Parent?.RenderTarget;
            set => _RenderTarget = value;
        }

        /// <summary>
        /// Blend method for Rendering
        /// </summary>
        public virtual BlendMode BlendMode
        {
            get => RenderState.BlendMode;
            set
            {
                var state = RenderState;
                state.BlendMode = value;
                RenderState = state;
            }
        }

        /// <summary>
        /// Shader for Rendering
        /// </summary>
        public virtual Shader Shader
        {
            get => RenderState.Shader;
            set
            {
                var state = RenderState;
                state.Shader = value;
                RenderState = state;
            }
        }

        /// <summary>
        /// Gets or sets the collision shape for collision detection
        /// </summary>
        public ICollisionShape CollisionShape
        {
            get => _CollisionShape ?? (_CollisionShape = new BasicGraphicCollisionShape(_Core.CollisionSystem, this));
            set => _CollisionShape = value;
        }

        /// <summary>
        /// Gets the position of this <see cref="IEntity"/> independent from scene graph and view.
        /// </summary>
        public Vector2f GlobalPosition => Position - Origin.MultiplyBy(Scale) + (Parent == null ? default : Parent.GlobalPosition);

        /// <summary>
        /// Determines whether this <see cref="IEntity" /> is destroyed.
        /// </summary>
        public bool Disposed => CPointer == IntPtr.Zero;



        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="Graphic"/> class.
        /// </summary>
        /// <param name="core">The render core.</param>
        public Graphic(Core core, Texture texture = null)
        {
            _Core = core ?? throw new ArgumentNullException(nameof(core));
            Texture = texture;
            _Alpha = 1;
            Visible = true;
            RenderState = RenderStates.Default;
        }


        // Methods #########################################################################
        /// <summary>
        /// Updates the <see cref="Graphic"/>.
        /// Can be overridden by derived classes.
        /// </summary>
        /// <param name="deltaT">Current game-time</param>
        public virtual void Update(Single deltaT) { }

        /// <summary>
        /// Draws the Graphic of the Entity if it is visible.
        /// Can be overridden by derived classes.
        /// </summary>
        public virtual void Draw() => _Core.Draw(this);

        /// <summary>
        /// Handle the destruction of the object
        /// </summary>
        /// <param name="disposing">Is the GC disposing the object, or is it an explicit call ?</param>
        protected override void Destroy(bool disposing)
        {
            if (Parent != null) Parent.Remove(this);
            base.Destroy(disposing);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => this.CreateIdString();
    }
}