using System.Linq;
using System.Collections.Generic;
using SFML.System;

namespace BlackCoat.Collision.Shapes
{
    /// <summary>
    /// Represents a Polygon for Collision Detection.
    /// </summary>
    /// <seealso cref="BlackCoat.Collision.CollisionShape"/>
    /// <seealso cref="BlackCoat.Collision.IPolygon"/>
    public class PolygonCollisionShape : CollisionShape, IPolygon
    {
        /// <summary>
        /// Position of the <see cref="PolygonCollisionShape"/>.
        /// </summary>
        public Vector2f Position { get; set; }
        
        /// <summary>
        /// Vectors this <see cref="PolygonCollisionShape" /> is composed of.
        /// </summary>
        public List<Vector2f> Points { get; set; }

        /// <summary>
        /// Vectors this <see cref="PolygonCollisionShape" /> is composed of. Read Only.
        /// </summary>
        IReadOnlyList<Vector2f> IPolygon.Points => Points;

        /// <summary>
        /// Determines the geometric primitive used for collision detection
        /// </summary>
        public override Geometry CollisionGeometry => Geometry.Polygon;


        /// <summary>
        /// Initializes a new instance of the <see cref="PolygonCollisionShape"/> class.
        /// </summary>
        /// <param name="collisionSystem">The collision system used for collision checking.</param>
        /// <param name="position">The <see cref="PolygonCollisionShape"/>'s position.</param>
        /// <param name="points">The points this <see cref="PolygonCollisionShape" /> is composed of.</param>
        public PolygonCollisionShape(CollisionSystem collisionSystem, Vector2f position, IEnumerable<Vector2f> points) : base(collisionSystem)
        {
            Position = position;
            Points = points.ToList();
        }


        /// <summary>
        /// Determines if this <see cref="PolygonCollisionShape"/> contains the defined point
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True when the point is inside the <see cref="PolygonCollisionShape"/></returns>
        public override bool Collide(Vector2f point)
        {
            return _CollisionSystem.CheckCollision(point, this);
        }

        /// <summary>
        /// Determines if this <see cref="PolygonCollisionShape"/> is colliding with another <see cref="ICollisionShape"/>
        /// </summary>
        /// <param name="other">The other <see cref="ICollisionShape"/></param>
        /// <returns>True when the objetcs overlap or touch</returns>
        public override bool Collide(ICollisionShape other)
        {
            return _CollisionSystem.CheckCollision(this, other);
        }
    }
}