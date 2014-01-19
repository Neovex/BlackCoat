using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackCoat.Entities
{
    public class ShapeItem:Shape,IEntity
    {
        // Variables #######################################################################
        protected Core _Core;
        private Container _Parent;
        protected Boolean _Visible = true;
        protected List<Role> _Roles = new List<Role>();
        protected Single _Alpha = 1;


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
        /// Determines the Visisbility of the Entity
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
                if (Color.A == 0) return 0;
                return _Alpha / 255f;
            }
            set
            {
                _Alpha = value * 255;
                if (_Alpha < 0) _Alpha = 0;
                Byte b = (Byte)_Alpha;
                var color = Color;
                color.A = b;
                Color = color;
            }
        }

        /// <summary>
        /// Current Role that descripes the Entities Behaviour
        /// </summary>
        public Role CurrentRole { get { return _Roles.Count == 0 ? null : _Roles[_Roles.Count - 1]; } }


        // CTOR ############################################################################
        public ShapeItem(Core core)
        {
            _Core = core;
        }


        // Methods #########################################################################
        /// <summary>
        /// Updates the Current Entity using its applied Role.
        /// Can be overriden by derived classes.
        /// </summary>
        /// <param name="deltaT">Current gametime</param>
        public virtual void Update(Single deltaT)
        {
            for (int i = _Roles.Count - 1; i > -1 && _Roles[i].Update(deltaT); i--);
        }

        /// <summary>
        /// Draws the Graphic of the Entity if it is visible.
        /// Can be overriden by derived classes.
        /// </summary>
        public virtual void Draw()
        {
            if (_Visible) _Core.Render(this);
        }


        // Roles #########################################################################
        /// <summary>
        /// Assigns a new Role to the Entity without removing the current one.
        /// Can be overriden by derived classes.
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
        /// Can be overriden by derived classes.
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
        /// Can be overriden by derived classes.
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