using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.System;

namespace BlackCoat.Entities.Shapes
{
    /// <summary>
    /// Represents a convex Polygon
    /// </summary>
    public class Polygon : Shape, IEntity
    {
        // Variables #######################################################################
        protected Core _Core;
        private Container _Parent;
        protected List<Role> _Roles = new List<Role>();
        protected Single _Alpha = 1;
        protected View _View = null;
        protected List<Vector2f> _Points = new List<Vector2f>();


        // Properties ######################################################################        
        /// <summary>
        /// Vectors this <see cref="Polygon"/> is composed of.
        /// </summary>
        public IEnumerable<Vector2f> Points { get { return _Points; } }

        /// <summary>
        /// Gets the <see cref="Vector2f"/> of the <see cref="Polygon"/> at the specified index.
        /// </summary>
        public Vector2f this[int index]
        {
            get { return _Points[index]; }
            set
            {
                if (index >= _Points.Count) _Points.Add(value);
                else _Points[index] = value;
                Update();
            }
        }

        /// <summary>
        /// Parent Container of this <see cref="Polygon"/>
        /// </summary>
        public Container Parent
        {
            get { return _Parent; }
            set { if (value == null || !value.HasChild(this)) _Parent = value; }
        }

        /// <summary>
        /// Determines the visibility of the <see cref="Polygon"/>
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
        /// Fillcolor of the <see cref="Polygon"/>
        /// </summary>
        public Color Color
        {
            get { return FillColor; }
            set { FillColor = value; }
        }

        /// <summary>
        /// Renderstate of the <see cref="Polygon"/>
        /// </summary>
        public virtual RenderStates RenderState { get; set; }

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
        /// Current Role that describes the <see cref="Polygon"/>s behavior
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
        /// Creates a new <see cref="Polygon"/> instance
        /// </summary>
        /// <param name="core">Engine Core</param>
        public Polygon(Core core)
        {
            _Core = core;
            Visible = true;
            RenderState = RenderStates.Default;
            _Points = new List<Vector2f>();
        }


        // Methods #########################################################################
        /// <summary>
        /// Updates the <see cref="Polygon"/> using its applied Role.
        /// Can be overridden by derived classes.
        /// </summary>
        /// <param name="deltaT">Current game-time</param>
        public virtual void Update(Single deltaT)
        {
            for (int i = _Roles.Count - 1; i > -1 && _Roles[i].Update(deltaT); i--);
        }

        /// <summary>
        /// Draws the <see cref="Polygon"/> if it is visible.
        /// Can be overridden by derived classes.
        /// </summary>
        public virtual void Draw()
        {
            _Core.Draw(this);
        }

        // Abstract Implementation #######################################################
        public override uint GetPointCount()
        {
            return (uint)_Points.Count;
        }

        public override Vector2f GetPoint(uint index)
        {
            return _Points[(int)index];
        }

        // Roles #########################################################################
        /// <summary>
        /// Assigns a new Role to the <see cref="Polygon"/> without removing the current one.
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
        /// Assigns a new Role to the <see cref="Polygon"/> after removing the current one.
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
        /// Removes the currently active Role from this <see cref="Polygon"/>
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