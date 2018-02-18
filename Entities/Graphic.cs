using System;
using System.Collections.Generic;

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
        private Single _Alpha = 255;
        private bool _Visible;
        protected View _View;
        private ICollisionShape _CollisionShape;
        protected List<Role> _Roles = new List<Role>();


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
            get { return _View ?? (_Parent == null ? _View : _Parent.View); }
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
        public virtual Single Alpha
        {
            get { return Color.A == 0 ? 0 : _Alpha / 255f; }
            set
            {
                _Alpha =  (_Parent == null ? value : value * _Parent.Alpha) * 255;
                if (_Alpha < 0) _Alpha = 0;
                var color = Color;
                color.A = (Byte)_Alpha;
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

        /// <summary>
        /// Current Role that describes the <see cref="IEntity"/>s Behavior
        /// </summary>
        public Role CurrentRole { get { return _Roles.Count == 0 ? null : _Roles[_Roles.Count - 1]; } }



        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="Graphic"/> class.
        /// </summary>
        /// <param name="core">The render core.</param>
        public Graphic(Core core)
        {
            _Core = core;
            Visible = true;
            RenderState = RenderStates.Default;
        }


        // Methods #########################################################################
        /// <summary>
        /// Updates the Current Entity using its applied Role.
        /// Can be overridden by derived classes.
        /// </summary>
        /// <param name="deltaT">Current game-time</param>
        public virtual void Update(Single deltaT)
        {
            for (int i = _Roles.Count - 1; i > -1 && _Roles[i].Update(deltaT); i--);
        }

        /// <summary>
        /// Draws the Graphic of the Entity if it is visible.
        /// Can be overridden by derived classes.
        /// </summary>
        public virtual void Draw()
        {
            _Core.Draw(this);
        }


        // Roles #########################################################################
        /// <summary>
        /// Assigns a new Role to the Entity without removing the current one.
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
        /// Assigns a new Role to the Entity after removing the current one.
        /// Can be overridden by derived classes.
        /// </summary>
        /// <param name="role">The Role to assign</param>
        /// <param name="supressInitialization">Suppress initialization call on assigned role</param>
        /// <returns>The removed role if there was one - otherwise null</returns>
        public virtual Role ReplaceRole(Role role, Boolean supressInitialization = false)
        {
            if (role == null) throw new ArgumentNullException("role");
            var ret = RemoveRole();
            AssignRole(role, true);
            if (!supressInitialization) role.Initialize();
            return ret;
        }

        /// <summary>
        /// Removes the currently active Role from this Entity
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