using SFML.Graphics;
using SFML.System;
using BlackCoat.Collision;

namespace BlackCoat.Entities.Shapes
{
    /// <summary>
    /// Represents a Line Primitive
    /// </summary>
    public class Line : EntityBase, ICollidable, ILine
    {
        // Variables #######################################################################
        private readonly Vertex[] _Vertices;


        /// <summary>
        /// Point to start the <see cref="Line"/> from
        /// </summary>
        public Vertex Start;

        /// <summary>
        /// Point to draw the <see cref="Line"/> to
        /// </summary>
        public Vertex End;


        // Properties ######################################################################

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


        public override Vector2f Position
        {
            get => Start.Position;
            set
            {
                var offset = Start.Position - value;
                Start.Position += offset;
                End.Position += offset;
            }
        }
        public override float Rotation { get => Start.Position.AngleTowards(End.Position); set { } }
        public override Vector2f Scale { get => new Vector2f(1,1); set { } }
        public override Vector2f Origin { get => new Vector2f(); set { } }
        public override Color Color { get => Start.Color; set => Start.Color = End.Color = value.ApplyAlpha(GlobalAlpha); }

        public override bool Disposed => false; // Due to vertices being simple structs explicit disposal is unnecessary


        // CTOR ############################################################################
        /// <summary>
        /// Creates a <see cref="Line"/> instance.
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
            _Vertices = new[] { Start, End };
        }


        // Methods #########################################################################
        /// <summary>
        /// Draws the <see cref="Line"/> respective to its parameters and view
        /// </summary>
        public override void Draw(RenderTarget target, RenderStates states)
        {
            _Vertices[0] = Start;
            _Vertices[1] = End;
            target.Draw(_Vertices, PrimitiveType.Lines, states);
        }

        // Collision Implementation ########################################################

        /// <summary>
        /// Determines if this <see cref="Line"/> contains the defined point
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True when the point is on the <see cref="Line"/></returns>
        public virtual bool CollidesWith(Vector2f point) => _Core.CollisionSystem.CheckCollision(point, this);

        /// <summary>
        /// Determines if this <see cref="Line"/> is colliding with another <see cref="ICollisionShape"/>
        /// </summary>
        /// <param name="other">The other <see cref="ICollisionShape"/></param>
        /// <returns>True when the objects overlap or touch</returns>
        public virtual bool CollidesWith(ICollisionShape other) => _Core.CollisionSystem.CheckCollision(this, other);
    }
}