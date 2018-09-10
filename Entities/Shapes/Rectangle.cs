using System;
using System.Collections.Generic;

using SFML.System;
using SFML.Graphics;

using BlackCoat.Collision;

namespace BlackCoat.Entities.Shapes
{
    /// <summary>
    /// Represents a Rectangle Primitive
    /// </summary>
    public class Rectangle : RectangleShape, IEntity, ICollidable, IRectangle
    {
        // Variables #######################################################################
        protected Core _Core;
        private Container _Parent;
        private Boolean _Visible;
        private View _View;
        private float _Alpha;


        // Properties ######################################################################
        /// <summary>
        /// Name of the <see cref="IEntity" />
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Parent Container of this <see cref="Rectangle"/>
        /// </summary>
        public Container Parent
        {
            get { return _Parent; }
            set { if (value == null || !value.Contains(this)) _Parent = value; }
        }

        /// <summary>
        /// Determines the visibility of the <see cref="Rectangle"/>
        /// </summary>
        public Boolean Visible
        {
            get { return _Visible && (_Parent == null || _Parent.Visible); }
            set { _Visible = value; }
        }

        /// <summary>
        /// Target Render View
        /// </summary>
        public View View
        {
            get { return _View ?? _Parent?.View; }
            set { _View = value; }
        }

        /// <summary>
        /// Alpha Visibility - 0-1f
        /// </summary>
        public Single Alpha
        {
            get { return _Alpha * (Parent == null ? 1 : _Parent.Alpha); }
            set { _Alpha = value; }
        }

        /// <summary>
        /// Renderstate of the <see cref="Rectangle"/>
        /// </summary>
        public virtual RenderStates RenderState { get; set; }

        /// <summary>
        /// Target device for rendering
        /// </summary>
        public RenderTarget RenderTarget { get; set; }

        /// <summary>
        /// Fill color of the <see cref="Rectangle"/>
        /// </summary>
        public Color Color
        {
            get { return FillColor; }
            set { FillColor = value; }
        }

        /// <summary>
        /// Blending method used for Rendering
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
        public virtual ICollisionShape CollisionShape => this;

        /// <summary>
        /// Determines the geometric primitive used for collision detection
        /// </summary>
        public virtual Geometry CollisionGeometry => Geometry.Rectangle;

        /// <summary>
        /// Gets the position of this <see cref="IEntity"/> independent from scene graph and view.
        /// </summary>
        public Vector2f GlobalPosition => Parent == null ? Position : Position.ToGlobal(Parent.GlobalPosition);



        // CTOR ############################################################################
        /// <summary>
        /// Creates a new <see cref="Rectangle"/> instance
        /// </summary>
        /// <param name="core">Engine Core</param>
        public Rectangle(Core core)
        {
            _Core = core;
            _Alpha = 1;
            Visible = true;
            RenderState = RenderStates.Default;
        }


        // Methods #########################################################################
        /// <summary>
        /// Updates the <see cref="Rectangle"/>.
        /// Can be overridden by derived classes.
        /// </summary>
        /// <param name="deltaT">Current game-time</param>
        public virtual void Update(Single deltaT) { }

        /// <summary>
        /// Draws the <see cref="Rectangle"/> if it is visible.
        /// Can be overridden by derived classes.
        /// </summary>
        public virtual void Draw() => _Core.Draw(this);

        // Collision Implementation ########################################################
        /// <summary>
        /// Determines if this <see cref="Rectangle"/> contains the defined point
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True when the point is inside the <see cref="Rectangle"/></returns>
        public virtual bool Collide(Vector2f point) => _Core.CollisionSystem.CheckCollision(point, this);

        /// <summary>
        /// Determines if this <see cref="Rectangle"/> is colliding with another <see cref="ICollisionShape"/>
        /// </summary>
        /// <param name="other">The other <see cref="ICollisionShape"/></param>
        /// <returns>True when the objects overlap or touch</returns>
        public virtual bool Collide(ICollisionShape other) => _Core.CollisionSystem.CheckCollision(this, other);

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => Create.IdString(this);
    }
}