using System;

using SFML.Graphics;
using SFML.System;
using BlackCoat.Collision;

namespace BlackCoat.Entities.Shapes
{
    /// <summary>
    /// Represents a Line Primitive
    /// </summary>
    public class Line : BaseEntity, IEntity, ICollidable, ILine
    {
        // Variables #######################################################################
        private Color _Color;
        private Vertex[] _Verticies;


        /// <summary>
        /// Point to start the line from
        /// </summary>
        public Vertex Start;

        /// <summary>
        /// Point to draw the line to
        /// </summary>
        public Vertex End;


        // Properties ######################################################################
        /// <summary>
        /// Line Color
        /// </summary>
        public override Color Color
        {
            get { return _Color; }
            set
            {
                _Color = value;
                Start.Color = value;
                End.Color = value;
            }
        }

        /// <summary>
        /// Gets or sets the collision shape for collision detection
        /// </summary>
        public virtual ICollisionShape CollisionShape => this;

        /// <summary>
        /// Determines the geometric primitive used for collision detection
        /// </summary>
        public virtual Geometry CollisionGeometry => Geometry.Line;

        /// <summary>
        /// Start position of the <see cref="ILine"/>. Read only.
        /// </summary>
        Vector2f ILine.Start => Start.Position;

        /// <summary>
        /// End position of the <see cref="ILine"/>. Read only.
        /// </summary>
        Vector2f ILine.End => End.Position;


        // CTOR ############################################################################
        /// <summary>
        /// Creates a Line instance.
        /// </summary>
        /// <param name="core">Engine Core</param>
        /// <param name="start">Start Position</param>
        /// <param name="end">End Position</param>
        /// <param name="color">Optional line color</param>
        public Line(Core core, Vector2f start, Vector2f end, Color? color = null) : base(core)
        {
            Start = new Vertex(start);
            End = new Vertex(end);
            Color = color ?? Color.Cyan;
            _Verticies = new[] { Start, End };
        }


        // Methods #########################################################################
        /// <summary>
        /// Draws the Line respective to its parameters and view
        /// </summary>
        public override void Draw()
        {
            _Verticies[0] = Start;
            _Verticies[1] = End;
            _Core.Draw(_Verticies, PrimitiveType.Lines, RenderState);
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            throw new NotImplementedException(); // FIXME
        }

        // Collision Implementation ########################################################
        /// <summary>
        /// Determines if this <see cref="Line"/> contains the defined point
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True when the point is on the <see cref="Line"/></returns>
        public virtual bool Collide(Vector2f point)
        {
            return _Core.CollisionSystem.CheckCollision(point, this);
        }

        /// <summary>
        /// Determines if this <see cref="Line"/> is colliding with another <see cref="ICollisionShape"/>
        /// </summary>
        /// <param name="other">The other <see cref="ICollisionShape"/></param>
        /// <returns>True when the objects overlap or touch</returns>
        public virtual bool Collide(ICollisionShape other)
        {
            return _Core.CollisionSystem.CheckCollision(this, other);
        }
    }
}