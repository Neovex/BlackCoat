using System;
using System.Collections.Generic;

using SFML.Graphics;

using BlackCoat.Collision;
using SFML.System;

namespace BlackCoat.Entities.Shapes
{
    /// <summary>
    /// Represents a Rectangle Primitve
    /// </summary>
    public class Rectangle : RectangleShape, IEntity, ICollidable, IRectangle
    {
        // Variables #######################################################################
        protected Core _Core;
        private Container _Parent;
        private Single _Alpha = 255;
        protected View _View;
        private ICollisionShape _CollisionShape;
        protected List<Role> _Roles = new List<Role>();


        // Properties ######################################################################
        /// <summary>
        /// Parent Container of this <see cref="Rectangle"/>
        /// </summary>
        public Container Parent
        {
            get { return _Parent; }
            set { if (value == null || !value.HasChild(this)) _Parent = value; }
        }

        /// <summary>
        /// Determines the visibility of the <see cref="Rectangle"/>
        /// </summary>
        public virtual Boolean Visible { get; set; }

        /// <summary>
        /// Target Render View
        /// </summary>
        public View View
        {
            get { return _View ?? (_Parent == null ? _View : _Parent.View); }
            set { _View = value; }
        }

        /// <summary>
        /// Renderstate of the <see cref="Rectangle"/>
        /// </summary>
        public virtual RenderStates RenderState { get; set; }

        /// <summary>
        /// Fillcolor of the <see cref="Rectangle"/>
        /// </summary>
        public Color Color
        {
            get { return FillColor; }
            set { FillColor = value; }
        }

        /// <summary>
        /// Alpha Value (0-1f)
        /// </summary>
        public virtual Single Alpha
        {
            get
            {
                if (FillColor.A == 0) return 0;
                return _Alpha / 255f;
            }
            set
            {
                _Alpha = value * 255;
                if (_Alpha < 0) _Alpha = 0;
                Byte b = (Byte)_Alpha;
                var color = FillColor;
                color.A = b;
                FillColor = color;
            }
        }

        /// <summary>
        /// Gets or sets the collision shape for collision detection
        /// </summary>
        public ICollisionShape CollisionShape
        {
            get { return _CollisionShape ?? this; }
            set { _CollisionShape = value; }
        }

        /// <summary>
        /// Current Role that describes the <see cref="Rectangle"/>s behavior
        /// </summary>
        public Role CurrentRole { get { return _Roles.Count == 0 ? null : _Roles[_Roles.Count - 1]; } }

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

        public Geometry CollisionGeometry
        {
            get
            {
                throw new NotImplementedException();
            }
        }


        // CTOR ############################################################################
        /// <summary>
        /// Creates a new <see cref="Rectangle"/> instance
        /// </summary>
        /// <param name="core">Engine Core</param>
        public Rectangle(Core core)
        {
            _Core = core;
            Visible = true;
            RenderState = RenderStates.Default;
        }


        // Methods #########################################################################
        /// <summary>
        /// Updates the <see cref="Rectangle"/> using its applied Role.
        /// Can be overridden by derived classes.
        /// </summary>
        /// <param name="deltaT">Current game-time</param>
        public virtual void Update(Single deltaT)
        {
            for (int i = _Roles.Count - 1; i > -1 && _Roles[i].Update(deltaT); i--) ;
        }

        /// <summary>
        /// Draws the <see cref="Rectangle"/> if it is visible.
        /// Can be overridden by derived classes.
        /// </summary>
        public virtual void Draw()
        {
            _Core.Draw(this);
        }

        /// <summary>
        /// Determines if this <see cref="Rectangle"/> is colliding with another <see cref="ICollisionShape"/>
        /// </summary>
        /// <param name="other">The other <see cref="ICollisionShape"/></param>
        /// <returns>True when the objetcs overlap or touch</returns>
        public bool Collide(ICollisionShape other)
        {
            if (_CollisionShape != null) return _CollisionShape.Collide(other);

            switch (other.CollisionGeometry)
            {
                case Geometry.Line:      return _Core.CollisionSystem.CheckCollision(this, other as ILine);
                case Geometry.Circle:    return _Core.CollisionSystem.CheckCollision(other as ICircle, this);
                case Geometry.Rectangle: return _Core.CollisionSystem.CheckCollision(this, other as IRectangle);
                case Geometry.Polygon:   return _Core.CollisionSystem.CheckCollision(this, other as IPoly);
            }

            Log.Error("Invalid collision shape", other, other?.CollisionGeometry);
            throw new Exception("Invalid collision shape");
        }

        // Roles #########################################################################
        /// <summary>
        /// Assigns a new Role to the <see cref="Rectangle"/> without removing the current one.
        /// Can be overridden by derived classes.
        /// </summary>
        /// <param name="role">The Role to assign</param>
        /// <param name="supressInitialization">Supress initialization call on assigned role</param>
        public virtual void AssignRole(Role role, Boolean supressInitialization = false)
        {
            if (role == null) throw new ArgumentNullException("role");
            role.Target = this;
            if (!supressInitialization) role.Initialize();
            _Roles.Add(role);
        }

        /// <summary>
        /// Assigns a new Role to the <see cref="Rectangle"/> after removing the current one.
        /// Can be overridden by derived classes.
        /// </summary>
        /// <param name="role">The Role to assign</param>
        /// <param name="supressInitialization">Supress initialization call on assigned role</param>
        /// <returns>The removed role if there was one - otherwhise null</returns>
        public virtual Role ReplaceRole(Role role, Boolean supressInitialization = false)
        {
            if (role == null) throw new ArgumentNullException("role");
            Role temp = RemoveRole();
            AssignRole(role);
            if (!supressInitialization) role.Initialize();
            return temp;
        }

        /// <summary>
        /// Removes the currently active Role from this <see cref="Rectangle"/>
        /// Can be overridden by derived classes.
        /// </summary>
        /// <returns>The removed role if there was one - otherwhise null</returns>
        public virtual Role RemoveRole()
        {
            if (_Roles.Count == 0) return null;

            var temp = _Roles[_Roles.Count - 1];
            _Roles.Remove(temp);
            temp.Target = null;
            return temp;
        }
    }
}