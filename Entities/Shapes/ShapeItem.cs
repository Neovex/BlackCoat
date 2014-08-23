using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackCoat.Entities.Shapes
{
    // TODO : fix shape madness
    public class ShapeItem:RectangleShape,IEntity
    {
        // Variables #######################################################################
        protected Core _Core;
        private Container _Parent;
        protected Boolean _Visible = true;
        protected List<Role> _Roles = new List<Role>();
        protected Single _Alpha = 1;

        protected RenderStates _RenderState = RenderStates.Default;
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
        /// Determines the visibility of the Entity
        /// </summary>
        public virtual Boolean Visible
        {
            get { return _Visible; }
            set { _Visible = value; }
        }

        /// <summary>
        /// Alpha Value
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
        /// Current Role that describes the Entities behavior
        /// </summary>
        public Role CurrentRole { get { return _Roles.Count == 0 ? null : _Roles[_Roles.Count - 1]; } }


        public View View
        {
            get { return _View ?? (_Parent == null ? _View : _Parent.View); }
            set { _View = value; }
        }


        // CTOR ############################################################################
        public ShapeItem(Core core)
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
            if (View != null) _Core.CurrentView = View;
            if (Parent != null) _RenderState.Transform = Parent.Transform;
            Draw(_Core.Device, _RenderState);
        }


        // Roles #########################################################################
        /// <summary>
        /// Assigns a new Role to the Entity without removing the current one.
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
        /// Assigns a new Role to the Entity after removing the current one.
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
        /// Removes the currently active Role from this Entity
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