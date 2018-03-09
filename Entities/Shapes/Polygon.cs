using System;
using System.Collections.Generic;
using System.Linq;

using SFML.System;
using SFML.Graphics;
using BlackCoat.Collision;

namespace BlackCoat.Entities.Shapes
{
    /// <summary>
    /// Represents a convex Polygon
    /// </summary>
    public class Polygon : Shape, IEntity, ICollidable, IPolygon
    {
        // Variables #######################################################################
        protected Core _Core;
        private Container _Parent;
        private Boolean _Visible;
        private View _View;
        private float _Alpha;
        private List<Vector2f> _Points;


        // Properties ######################################################################
        /// <summary>
        /// Vectors this <see cref="Polygon"/> is composed of. Read Only.
        /// </summary>
        public IReadOnlyList<Vector2f> Points { get { return _Points; } }

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
                Update(); // TODO: mayhaps find a way to force convexity
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
            get { return (_View ?? _Parent?.View) ?? _View; }
            set { _View = value; }
        }

        /// <summary>
        /// Renderstate of the <see cref="Polygon"/>
        /// </summary>
        public virtual RenderStates RenderState { get; set; }

        /// <summary>
        /// Target device for rendering
        /// </summary>
        public RenderTarget RenderTarget { get; set; }

        /// <summary>
        /// Fill color of the <see cref="Polygon"/>
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
            get { return _Alpha; }
            set
            {
                _Alpha = value = Math.Max(0, value);
                if (_Parent != null) value *= _Parent.Alpha;
                var color = Color;
                color.A = (Byte)(value * 255);
                Color = color;
            }
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
        public virtual Geometry CollisionGeometry => Geometry.Polygon;


        // CTOR ############################################################################
        /// <summary>
        /// Creates a new <see cref="Polygon"/> instance
        /// </summary>
        /// <param name="core">Engine Core</param>
        public Polygon(Core core, IEnumerable<Vector2f> points = null)
        {
            _Core = core;
            _Alpha = 1;
            Visible = true;
            RenderState = RenderStates.Default;

            _Points = points?.ToList() ?? new List<Vector2f>();
            Update();
        }


        // Methods #########################################################################
        /// <summary>
        /// Updates the <see cref="Polygon"/>.
        /// Can be overridden by derived classes.
        /// </summary>
        /// <param name="deltaT">Current game-time</param>
        public virtual void Update(Single deltaT)
        {
        }

        /// <summary>
        /// Draws the <see cref="Polygon"/> if it is visible.
        /// Can be overridden by derived classes.
        /// </summary>
        public virtual void Draw()
        {
            _Core.Draw(this);
        }

        // Abstract Implementation #########################################################        
        /// <summary>
        /// Get the total number of points of the <see cref="Polygon"/>
        /// </summary>
        /// <returns>The total point count</returns>
        public override uint GetPointCount()
        {
            return (uint)_Points.Count;
        }

        /// <summary>
        /// Get the position of a point
        /// The returned point is in local coordinates, that is,
        /// the <see cref="Polygon"/>'s transforms (position, rotation, scale) are
        /// not taken into account.
        /// </summary>
        /// <param name="index">Index of the point to get, in range [0 .. PointCount - 1]</param>
        /// <returns>index-th point of the <see cref="Polygon"/></returns>
        public override Vector2f GetPoint(uint index)
        {
            return _Points[(int)index];
        }


        // Collision Implementation ########################################################
        /// <summary>
        /// Determines if this <see cref="Polygon"/> contains the defined point
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True when the point is inside the <see cref="Polygon"/></returns>
        public virtual bool Collide(Vector2f point)
        {
            return _Core.CollisionSystem.CheckCollision(point, this);
        }

        /// <summary>
        /// Determines if this <see cref="Polygon"/> is colliding with another <see cref="ICollisionShape"/>
        /// </summary>
        /// <param name="other">The other <see cref="ICollisionShape"/></param>
        /// <returns>True when the objects overlap or touch</returns>
        public virtual bool Collide(ICollisionShape other)
        {
            return _Core.CollisionSystem.CheckCollision(this, other);
        }
    }
}