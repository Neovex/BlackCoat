using System;
using SFML.System;
using SFML.Graphics;
using BlackCoat.Collision;
using BlackCoat.Collision.Shapes;


namespace BlackCoat.Entities
{
    /// <summary>
    /// Renders Text onto the Scene
    /// </summary>
    public class TextItem : Text, IEntity, ICollidable
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
        /// Text to Render
        /// </summary>
        public String Text
        {
            get => DisplayedString;
            set => DisplayedString = value;
        }

        /// <summary>
        /// Name of the <see cref="IEntity" />
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Parent Container of this Entity
        /// </summary>
        public Container Parent
        {
            get => _Parent;
            set => _Parent = value == null || !value.Contains(this) ? value : _Parent;
        }

        /// <summary>
        /// Determines the Visibility of the Entity
        /// </summary>
        public Boolean Visible
        {
            get => _Visible && (_Parent == null || _Parent.Visible);
            set => _Visible = value;
        }

        /// <summary>
        /// Target Render View
        /// </summary>
        public View View
        {
            get => _View ?? _Parent?.View;
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
        /// Renderstate of the entity
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
            get => _CollisionShape ?? (_CollisionShape = new BasicTextCollisionShape(_Core.CollisionSystem, this));
            set => _CollisionShape = value;
        }

        /// <summary>
        /// Gets the position of this <see cref="IEntity"/> independent from scene graph and view.
        /// </summary>
        public Vector2f GlobalPosition => Parent == null ? Position : (Position - Origin).ToGlobal(Parent.GlobalPosition);


        // CTOR ############################################################################        
        /// <summary>
        /// Initializes a new instance of the <see cref="TextItem" /> class.
        /// </summary>
        /// <param name="core">Black Coat Engine Core.</param>
        /// <param name="text">The text to display.</param>
        /// <param name="font">Initial font.</param>
        public TextItem(Core core, String text = "", Font font = null)
        {
            _Core = core;
            Text = text;
            _Alpha = 1;
            Visible = true;
            RenderState = RenderStates.Default;
            Font = font ?? _Core.DefaultFont;
        }


        // Methods #########################################################################
        /// <summary>
        /// Updates the <see cref="TextItem"/>.
        /// Can be overridden by derived classes.
        /// </summary>
        /// <param name="deltaT">Current game time</param>
        public virtual void Update(Single deltaT) { }

        /// <summary>
        /// Draws the Text
        /// </summary>
        public virtual void Draw() => _Core.Draw(this);

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => Create.IdString(this);
    }
}