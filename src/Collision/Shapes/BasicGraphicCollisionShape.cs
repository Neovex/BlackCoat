using System;
using SFML.System;
using BlackCoat.Entities;

namespace BlackCoat.Collision.Shapes
{
    /// <summary>
    /// This class provides a Rectangular Collision Shape based on a associated <see cref="Graphic"/> instance.
    /// </summary>
    /// <seealso cref="BlackCoat.Collision.CollisionShape" />
    /// <seealso cref="BlackCoat.Collision.IRectangle" />
    public class BasicGraphicCollisionShape : CollisionShape, IRectangle
    {
        private Graphic _Graphic;

        /// <summary>
        /// Determines the geometric primitive used for collision detection
        /// </summary>
        public override Geometry CollisionGeometry => Geometry.Rectangle;

        /// <summary>
        /// Gets the position of the associated <see cref="Graphic"/>.
        /// </summary>
        public Vector2f Position => _Graphic.GlobalPosition;

        /// <summary>
        /// Gets the size of the associated <see cref="Graphic"/>.
        /// </summary>
        public Vector2f Size => _Graphic.TextureRect.Size().ToVector2f().MultiplyBy(_Graphic.Scale);


        /// <summary>
        /// Initializes a new instance of the <see cref="BasicGraphicCollisionShape"/> class.
        /// </summary>
        /// <param name="collisionSystem">The collision system used for collision checking.</param>
        /// <param name="graphic">The <see cref="Graphic"/> that will be associated with this <see cref="BasicGraphicCollisionShape"/>.</param>
        public BasicGraphicCollisionShape(CollisionSystem collisionSystem, Graphic graphic) : base(collisionSystem)
        {
            _Graphic = graphic ?? throw new ArgumentNullException(nameof(graphic));
        }

        /// <summary>
        /// Determines if this <see cref="BasicGraphicCollisionShape"/> contains the defined point
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True when the point is inside the <see cref="BasicGraphicCollisionShape"/></returns>
        override public bool CollidesWith(Vector2f point)
        {
            return _CollisionSystem.CheckCollision(point, this);
        }

        /// <summary>
        /// Determines if this <see cref="BasicGraphicCollisionShape"/> is colliding with another <see cref="ICollisionShape"/>
        /// </summary>
        /// <param name="other">The other <see cref="ICollisionShape"/></param>
        /// <returns>True when the objects overlap or touch</returns>
        override public bool CollidesWith(ICollisionShape other)
        {
            return _CollisionSystem.CheckCollision(this, other);
        }
    }
}