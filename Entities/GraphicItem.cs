using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace BlackCoat.Entities
{
    public class GraphicItem : Sprite, IEntity
    {
        // Variables #######################################################################
        protected Core _Core;
        private Container _Parent;
        protected Boolean _Visible = true;
        protected List<Role> _Roles = new List<Role>();
        protected Single _Alpha = 1;

        protected RenderStates _RenderState = RenderStates.Default;


        // Properties ######################################################################
        /// <summary>
        /// Parent Container of this Entity
        /// </summary>
        public Container Parent
        {
            get { return _Parent; }
            set { /*if (value == null || !value.HasChild(this))*/ _Parent = value; }
        }

        /// <summary>
        /// Determines the Visibility of the Entity
        /// </summary>
        public virtual Boolean Visible
        {
            get { return _Visible; }
            set { _Visible = value; }
        }

        /// <summary>
        /// Alpha Visibility - 0-1f
        /// </summary>
        public virtual Single Alpha
        {
            get { return Color.A == 0 ? 0 : _Alpha / 255f; }
            set
            {
                _Alpha = value * 255;
                if (_Alpha < 0) _Alpha = 0;
                var color = Color;
                color.A = (Byte)_Alpha;
                Color = color;
            }
        }

        /// <summary>
        /// Blendingmethod used for Rendering
        /// </summary>
        public virtual BlendMode BlendMode
        {
            get { return _RenderState.BlendMode; }
            set { _RenderState.BlendMode = value; }
        }

        /// <summary>
        /// Current Role that describes the Entities Behavior
        /// </summary>
        public Role CurrentRole { get { return _Roles.Count == 0 ? null : _Roles[_Roles.Count - 1]; } }


        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="GraphicItem"/> class.
        /// </summary>
        /// <param name="core">The BC core.</param>
        public GraphicItem(Core core)
        {
            _Core = core;
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
            if (!_Visible) return;
            if (Parent != null) _RenderState.Transform = Parent.Transform;
            Draw(_Core.Device, _RenderState);
            //_Core.Render(this/*, Parent*/); // CHECK THIS
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
            AssignRole(role);
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