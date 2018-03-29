using System;
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
        private ICollisionShape _CollisionShape;


        // Properties ######################################################################
        /// <summary>
        /// Parent Container of the <see cref="Graphic"/>
        /// </summary>
        public Container Parent
        {
            get { return _Parent; }
            set { if (value == null || !value.HasChild(this)) _Parent = value; }
        }

        /// <summary>
        /// Determines the Visibility of the <see cref="Graphic"/>
        /// </summary>
        public virtual Boolean Visible
        {
            get { return _Visible && (_Parent == null ? true : _Parent.Visible); }
            set { _Visible = value; }
        }

        /// <summary>
        /// Target Render View
        /// </summary>
        public View View
        {
            get { return (_View ?? _Parent?.View) ?? _View; }
            set { _View = value; }
        }

        /// <summary>
        /// Renderstate of the <see cref="Graphic"/> 
        /// </summary>
        public RenderStates RenderState { get; set; }

        /// <summary>
        /// Target device for rendering
        /// </summary>
        public RenderTarget RenderTarget { get; set; }

        /// <summary>
        /// Alpha Visibility - 0-1f
        /// </summary>
        public Single Alpha
        {
            get { return _Alpha; }
            set
            {
                _Alpha = value = Math.Max(0, value);
                if (_Parent != null) value *= _Parent.Alpha;
                var color = Color;
                color.A = (Byte)(value * Byte.MaxValue);
                Color = color;
            }
        }

        /// <summary>
        /// Blend method for Rendering
        /// </summary>
        public virtual BlendMode BlendMode
        {
            get { return RenderState.BlendMode; }
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
            get { return RenderState.Shader; }
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
            get { return _CollisionShape ?? (_CollisionShape = new GraphicCollisionShape(_Core.CollisionSystem, this)); }
            set { _CollisionShape = value; }
        }



        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="Graphic"/> class.
        /// </summary>
        /// <param name="core">The render core.</param>
        public Graphic(Core core)
        {
            _Core = core;
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
        public virtual void Update(Single deltaT)
        {
        }

        /// <summary>
        /// Draws the Graphic of the Entity if it is visible.
        /// Can be overridden by derived classes.
        /// </summary>
        public virtual void Draw()
        {
            _Core.Draw(this);
        }
    }
}