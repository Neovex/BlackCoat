using System;
using System.Linq;
using System.Collections.Generic;
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
        private RenderTarget _RenderTarget;
        private List<Vector2f> _Points;


        // Properties ######################################################################
        /// <summary>
        /// Vectors this <see cref="Polygon"/> is composed of. Read Only.
        /// </summary>
        public IReadOnlyList<Vector2f> Points => _Points;

        /// <summary>
        /// Gets the <see cref="Vector2f"/> of the <see cref="Polygon"/> at the specified index.
        /// </summary>
        public Vector2f this[int index]
        {
            get => _Points[index % _Points.Count];
            set
            {
                if (index >= _Points.Count) _Points.Add(value);
                else _Points[index] = value;
                Update(); // TODO: mayhaps find a way to force convexity
            }
        }

        /// <summary>
        /// Name of the <see cref="IEntity" />
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Parent Container of this <see cref="Polygon"/>
        /// </summary>
        public Container Parent
        {
            get => _Parent;
            set => _Parent = value == null || !value.Contains(this) ? value : _Parent;
        }

        /// <summary>
        /// Determines the visibility of the <see cref="Polygon"/>
        /// </summary>
        public Boolean Visible
        {
            get => _Visible && (_Parent == null || _Parent.Visible);
            set => _Visible = value;
        }

        /// <summary>
        /// Target Render View
        /// </summary>
        public View View
        {
            get => _View ?? _Parent?.View;
            set => _View = value;
        }

        /// <summary>
        /// Alpha Visibility - 0-1f
        /// </summary>
        public virtual Single Alpha
        {
            get => _Alpha;
            set
            {
                _Alpha = value < 0 ? 0 : value > 1 ? 1 : value;
                var color = Color;
                color.A = (Byte)(GlobalAlpha * Byte.MaxValue);
                Color = color;
            }
        }
        /// <summary>
        /// Global Alpha Visibility according to the scene graph
        /// </summary>
        public virtual Single GlobalAlpha => _Alpha * (Parent == null ? 1 : _Parent.GlobalAlpha);

        /// <summary>
        /// Renderstate of the <see cref="Polygon"/>
        /// </summary>
        public virtual RenderStates RenderState { get; set; }

        /// <summary>
        /// Target device for rendering
        /// </summary>
        public RenderTarget RenderTarget
        {
            get => _RenderTarget ?? _Parent?.RenderTarget;
            set => _RenderTarget = value;
        }

        /// <summary>
        /// Fill color of the <see cref="Polygon"/>
        /// </summary>
        public Color Color
        {
            get => FillColor;
            set => FillColor = value;
        }

        /// <summary>
        /// Blending method used for Rendering
        /// </summary>
        public virtual BlendMode BlendMode
        {
            get => RenderState.BlendMode;
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
            get => RenderState.Shader;
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

        /// <summary>
        /// Gets the position of this <see cref="IEntity"/> independent from scene graph and view.
        /// </summary>
        public Vector2f GlobalPosition => Parent == null ? Position : (Position - Origin).ToGlobal(Parent.GlobalPosition);

        /// <summary>
        /// Determines whether this <see cref="IEntity" /> is destroyed.
        /// </summary>
        public bool Destroyed { get; private set; }


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
        public virtual void Update(Single deltaT) { }

        /// <summary>
        /// Draws the <see cref="Polygon"/> if it is visible.
        /// Can be overridden by derived classes.
        /// </summary>
        public virtual void Draw() => _Core.Draw(this);

        // Abstract Implementation #########################################################        
        /// <summary>
        /// Get the total number of points of the <see cref="Polygon"/>
        /// </summary>
        /// <returns>The total point count</returns>
        public override uint GetPointCount() => (uint)_Points.Count;

        /// <summary>
        /// Get the position of a point
        /// The returned point is in local coordinates, that is,
        /// the <see cref="Polygon"/>'s transforms (position, rotation, scale) are
        /// not taken into account.
        /// </summary>
        /// <param name="index">Index of the point to get, in range [0 .. PointCount - 1]</param>
        /// <returns>index-th point of the <see cref="Polygon"/></returns>
        public override Vector2f GetPoint(uint index) => this[(int)index];


        // Collision Implementation ########################################################
        /// <summary>
        /// Determines if this <see cref="Polygon"/> contains the defined point
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True when the point is inside the <see cref="Polygon"/></returns>
        public virtual bool CollidesWith(Vector2f point) => _Core.CollisionSystem.CheckCollision(point, this);

        /// <summary>
        /// Determines if this <see cref="Polygon"/> is colliding with another <see cref="ICollisionShape"/>
        /// </summary>
        /// <param name="other">The other <see cref="ICollisionShape"/></param>
        /// <returns>True when the objects overlap or touch</returns>
        public virtual bool CollidesWith(ICollisionShape other) => _Core.CollisionSystem.CheckCollision(this, other);

        /// <summary>
        /// Handle the destruction of the <see cref="IEntity"/>
        /// </summary>
        /// <param name="disposing">Is the GC disposing the object, or is it an explicit call ?</param>
        protected override void Destroy(bool disposing)
        {
            Destroyed = true;
            base.Destroy(disposing);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => Create.IdString(this);
    }
}