using SFML.System;

namespace BlackCoat.Collision.Shapes
{
    /// <summary>
    /// Represents a simple Line for Collision Detection.
    /// </summary>
    /// <seealso cref="BlackCoat.Collision.CollisionShape"/>
    /// <seealso cref="BlackCoat.Collision.ILine"/>
    public class LineCollisionShape : CollisionShape, ILine
    {
        /// <summary>
        /// Startposition of the <see cref="LineCollisionShape"/>.
        /// </summary>
        public Vector2f Start { get; set; }

        /// <summary>
        /// Endposition of the <see cref="LineCollisionShape"/>.
        /// </summary>
        public Vector2f End { get; set; }

        /// <summary>
        /// Determines the geometric primitive used for collision detection
        /// </summary>
        public override Geometry CollisionGeometry => Geometry.Line;


        /// <summary>
        /// Initializes a new instance of the <see cref="LineCollisionShape"/> class.
        /// </summary>
        /// <param name="collisionSystem">The collision system used for collision checking.</param>
        /// <param name="start">The start vector.</param>
        /// <param name="end">The end vector.</param>
        public LineCollisionShape(CollisionSystem collisionSystem, Vector2f start, Vector2f end) : base(collisionSystem)
        {
            Start = start;
            End = end;
        }


        /// <summary>
        /// Determines if this <see cref="LineCollisionShape"/> contains the defined point
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True when the point is on the <see cref="LineCollisionShape"/></returns>
        public override bool CollidesWith(Vector2f point)
        {
            return _CollisionSystem.CheckCollision(point, this);
        }

        /// <summary>
        /// Determines if this <see cref="LineCollisionShape"/> is colliding with another <see cref="ICollisionShape"/>
        /// </summary>
        /// <param name="other">The other <see cref="ICollisionShape"/></param>
        /// <returns>True when the objetcs overlap or touch</returns>
        public override bool CollidesWith(ICollisionShape other)
        {
            return _CollisionSystem.CheckCollision(this, other);
        }
    }
}