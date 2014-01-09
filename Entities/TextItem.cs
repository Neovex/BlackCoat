using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace BlackCoat
{
    public class TextItem : String2D, IEntity
    {
        // Variables #######################################################################
        protected Core _Core;
        private Container _Parent;
        protected Boolean _Visible = true;
        protected List<Role> _Roles = new List<Role>();


        // Properties ######################################################################
        /// <summary>
        /// Local  Position
        /// </summary>
        public override Vector2 Position
        {
            get
            {
                if (_Parent == null) return base.Position;
                return _Parent.TransformToLocal(base.Position);
            }
            set
            {
                if (_Parent == null) base.Position = value;
                else base.Position = _Parent.TransformToGlobal(value);
            }
        }

        /// <summary>
        /// Local Rotation
        /// </summary>
        public override Single Rotation
        {
            get
            {
                if (_Parent == null) return GlobalRotation;
                return GlobalRotation - _Parent.GlobalRotation;
            }
            set
            {
                if (_Parent == null) GlobalRotation = value;
                else GlobalRotation = _Parent.GlobalRotation + value;
            }
        }

        /// <summary>
        /// World Rotation
        /// </summary>
        public virtual Single GlobalRotation
        {
            get { return base.Rotation; }
            set { base.Rotation = value; }
        }

        /// <summary>
        /// Parent Container of this Entity
        /// </summary>
        public Container Parent
        {
            get { return _Parent; }
            set
            {
                if (_Parent != null && !_Parent.HasChild(this)) return;
                _Parent = value;
            }
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
        public Single Alpha
        {
            get { return 255 / Color.A; }
            set
            {
                Byte b = (Byte)(value * 255);
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
        public TextItem(Core core)
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
        /// Draws the Text
        /// </summary>
        public virtual void Draw()
        {
            if (_Visible) _Core.Render(this);
        }


        // Roles #########################################################################
        /// <summary>
        /// Assigns a new Role to the Entity without removing the current one.
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
        /// </summary>
        /// <returns>The removed role if there was one - otherwhise null</returns>
        public virtual Role RemoveRole()
        {
            Role temp = null;
            if (_Roles.Count != 0)
            {
                temp = _Roles[_Roles.Count - 1];
                _Roles.RemoveAt(_Roles.Count - 1);
                temp.Target = null;
            }
            return temp;
        }
    }
}