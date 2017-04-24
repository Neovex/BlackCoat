using System;
using SFML.System;
using BlackCoat.Entities;

namespace BlackCoat.Collision
{
    /// <summary>
    /// This class provides a Rectangular Collision Shape based on a associated <see cref="Graphic"/> instance.
    /// </summary>
    /// <seealso cref="BlackCoat.Collision.CollisionShape" />
    /// <seealso cref="BlackCoat.Collision.IRectangle" />
    public class GraphicCollisionShape : CollisionShape, IRectangle
    {
        private Graphic _Graphic;

        /// <summary>
        /// Determines the geometric primitive used for collision detection
        /// </summary>
        public override Geometry CollisionGeometry => Geometry.Rectangle;

        /// <summary>
        /// Gets the position of the associated <see cref="Graphic"/>.
        /// </summary>
        public Vector2f Position => _Graphic.Position;

        /// <summary>
        /// Gets the size of the associated <see cref="Graphic"/>.
        /// </summary>
        public Vector2f Size => _Graphic.Texture?.Size.ToVector2f() ?? new Vector2f();


        /// <summary>
        /// Initializes a new instance of the <see cref="GraphicCollisionShape"/> class.
        /// </summary>
        /// <param name="collisionSystem">The collision system used for collision checking.</param>
        /// <param name="graphic">The <see cref="Graphic"/> that will be associated with this <see cref="GraphicCollisionShape"/>.</param>
        public GraphicCollisionShape(CollisionSystem collisionSystem, Graphic graphic) : base(collisionSystem)
        {
            if (graphic == null) throw new ArgumentNullException(nameof(graphic));
            _Graphic = graphic;
        }

        /// <summary>
        /// Determines if this <see cref="GraphicCollisionShape"/> contains the defined point
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True when the point is inside the <see cref="GraphicCollisionShape"/></returns>
        override public bool Collide(Vector2f point)
        {
            return _CollisionSystem.CheckCollision(point, this);
        }

        /// <summary>
        /// Determines if this <see cref="GraphicCollisionShape"/> is colliding with another <see cref="ICollisionShape"/>
        /// </summary>
        /// <param name="other">The other <see cref="ICollisionShape"/></param>
        /// <returns>True when the objetcs overlap or touch</returns>
        override public bool Collide(ICollisionShape other)
        {
            return _CollisionSystem.CheckCollision(this, other);
        }
    }
}