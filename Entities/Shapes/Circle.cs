using System;
using System.Collections.Generic;

using SFML.System;
using SFML.Graphics;
using BlackCoat.Collision;

namespace BlackCoat.Entities.Shapes
{
    /// <summary>
    /// Represents a Circle Primitive
    /// </summary>
    public class Circle : CircleShape, IEntity, ICollidable, ICircle
    {
        // Variables #######################################################################
        protected Core _Core;
        private Container _Parent;
        private Single _Alpha = 255;
        protected View _View;
        protected List<Role> _Roles = new List<Role>();


        // Properties ######################################################################
        /// <summary>
        /// Parent Container of this <see cref="Circle"/>
        /// </summary>
        public Container Parent
        {
            get { return _Parent; }
            set { if (value == null || !value.HasChild(this)) _Parent = value; }
        }

        /// <summary>
        /// Determines the visibility of the <see cref="Circle"/>
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
        /// Render-state of the <see cref="Circle"/>
        /// </summary>
        public virtual RenderStates RenderState { get; set; }

        /// <summary>
        /// Fill-color of the <see cref="Circle"/>
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
        public virtual ICollisionShape CollisionShape => this;

        /// <summary>
        /// Determines the geometric primitive used for collision detection
        /// </summary>
        public virtual Geometry CollisionGeometry => Geometry.Circle;

        /// <summary>
        /// Current Role that describes the <see cref="Circle"/>s behavior
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



        // CTOR ############################################################################
        /// <summary>
        /// Creates a new <see cref="Circle"/> instance
        /// </summary>
        /// <param name="core">Engine Core</param>
        public Circle(Core core)
        {
            _Core = core;
            Visible = true;
            RenderState = RenderStates.Default;
        }


        // Methods #########################################################################
        /// <summary>
        /// Updates the <see cref="Circle"/> using its applied Role.
        /// Can be overridden by derived classes.
        /// </summary>
        /// <param name="deltaT">Current game-time</param>
        public virtual void Update(Single deltaT)
        {
            for (int i = _Roles.Count - 1; i > -1 && _Roles[i].Update(deltaT); i--) ;
        }

        /// <summary>
        /// Draws the <see cref="Circle"/> if it is visible.
        /// Can be overridden by derived classes.
        /// </summary>
        public virtual void Draw()
        {
            _Core.Draw(this);
        }

        // Collision Implementation ########################################################
        /// <summary>
        /// Determines if this <see cref="Circle"/> contains the defined point
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True when the point is inside the <see cref="Circle"/></returns>
        public virtual bool Collide(Vector2f point)
        {
            return _Core.CollisionSystem.CheckCollision(point, this);
        }

        /// <summary>
        /// Determines if this <see cref="Circle"/> is colliding with another <see cref="ICollisionShape"/>
        /// </summary>
        /// <param name="other">The other <see cref="ICollisionShape"/></param>
        /// <returns>True when the objects overlap or touch</returns>
        public virtual bool Collide(ICollisionShape other)
        {
            return _Core.CollisionSystem.CheckCollision(this, other);
        }

        // Roles ###########################################################################
        /// <summary>
        /// Assigns a new Role to the <see cref="Circle"/> without removing the current one.
        /// Can be overridden by derived classes.
        /// </summary>
        /// <param name="role">The Role to assign</param>
        /// <param name="supressInitialization">Suppress initialization call on assigned role</param>
        public virtual void AssignRole(Role role, Boolean supressInitialization = false)
        {
            if (role == null) throw new ArgumentNullException("role");
            role.Target = this;
            if (!supressInitialization) role.Initialize();
            _Roles.Add(role);
        }

        /// <summary>
        /// Assigns a new Role to the <see cref="Circle"/> after removing the current one.
        /// Can be overridden by derived classes.
        /// </summary>
        /// <param name="role">The Role to assign</param>
        /// <param name="supressInitialization">Suppress initialization call on assigned role</param>
        /// <returns>The removed role if there was one - otherwise null</returns>
        public virtual Role ReplaceRole(Role role, Boolean supressInitialization = false)
        {
            if (role == null) throw new ArgumentNullException("role");
            var temp = RemoveRole();
            AssignRole(role, true);
            if (!supressInitialization) role.Initialize();
            return temp;
        }

        /// <summary>
        /// Removes the currently active Role from this <see cref="Circle"/>
        /// Can be overridden by derived classes.
        /// </summary>
        /// <returns>The removed role if there was one - otherwise null</returns>
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