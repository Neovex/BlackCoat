using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace BlackCoat.Entities
{
    /// <summary>
    /// Basic abstract implementation of IEntity
    /// </summary>
    public abstract class BaseEntity: Transformable, IEntity
    {
        // Variables #######################################################################
        protected Core _Core;
        private Container _Parent;
        protected List<Role> _Roles = new List<Role>();
        protected Single _Alpha = 255;
        protected View _View = null;


        // Properties ######################################################################
        /// <summary>
        /// Parent Container of this Entity
        /// </summary>
        public Container Parent
        {
            get { return _Parent; }
            set { if (value == null || !value.HasChild(this)) _Parent = value; }
        }

        /// <summary>
        /// Determines the Visibility of the Entity
        /// </summary>
        public virtual Boolean Visible { get; set; }

        /// <summary>
        /// Target Render View
        /// </summary>
        public virtual View View
        {
            get { return _View ?? (_Parent == null ? _View : _Parent.View); }
            set { _View = value; }
        }

        /// <summary>
        /// Renderstate of the entity
        /// </summary>
        public virtual RenderStates RenderState { get; set; }

        /// <summary>
        /// Target device for rendering
        /// </summary>
        public virtual RenderTarget RenderTarget { get; set; }

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
        /// Entity Color or Tint
        /// </summary>
        public abstract Color Color { get; set; }

        /// <summary>
        /// Alpha Visibility - 0-1f
        /// </summary>
        public virtual Single Alpha
        {
            get { return Color.A == 0 ? 0 : _Alpha / 255f; }
            set
            {
                _Alpha = (_Parent == null ? value : value * _Parent.Alpha) * 255;
                if (_Alpha < 0) _Alpha = 0;
                var color = Color;
                color.A = (Byte)_Alpha;
                Color = color;
            }
        }

        /// <summary>
        /// Current Role that describes the Entities Behavior
        /// </summary>
        public Role CurrentRole { get { return _Roles.Count == 0 ? null : _Roles[_Roles.Count - 1]; } }


        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="Graphic"/> class.
        /// </summary>
        /// <param name="core">The render core.</param>
        protected BaseEntity(Core core)
        {
            _Core = core;
            RenderState = RenderStates.Default;
        }


        // Methods #########################################################################
        /// <summary>
        /// Updates the Current Entity using its applied Role(s).
        /// Can be overridden by derived classes.
        /// </summary>
        /// <param name="deltaT">Current game-time</param>
        public virtual void Update(Single deltaT)
        {
            for (int i = _Roles.Count - 1; i > -1 && _Roles[i].Update(deltaT); i--);
        }

        /// <summary>
        /// Renders the entity into the scene if correctly implemented in derived classes.
        /// </summary>
        public abstract void Draw();

        /// <summary>
        /// Renders the entity into the scene if correctly implemented in derived classes.
        /// </summary>
        /// <param name="target">Render device</param>
        /// <param name="states">Additional render information</param>
        public abstract void Draw(RenderTarget target, RenderStates states);


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