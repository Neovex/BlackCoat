﻿using System;
using SFML.System;

namespace BlackCoat.Collision.Shapes
{
    /// <summary>
    /// Abstract base class of all Collision Shapes
    /// </summary>
    /// <seealso cref="BlackCoat.Collision.ICollisionShape" />
    public abstract class CollisionShape : ICollisionShape
    {
        // Variables #######################################################################
        protected CollisionSystem _CollisionSystem;


        // Properties ######################################################################
        /// <summary>
        /// Determines the geometric primitive used for collision detection
        /// </summary>
        public abstract Geometry CollisionGeometry { get; }


        // CTOR ############################################################################
        /// <summary>
        /// Initializes the base class <see cref="CollisionShape"/>
        /// </summary>
        /// <param name="collisionSystem">The collision system used for collision checking.</param>
        public CollisionShape(CollisionSystem collisionSystem)
        {
            _CollisionSystem = collisionSystem ?? throw new ArgumentNullException(nameof(collisionSystem));
        }


        // Methods #########################################################################
        /// <summary>
        /// Determines if this  <see cref="CollisionShape"/> contains the defined point
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True when the point is inside the object</returns>
        public abstract bool CollidesWith(Vector2f point);

        /// <summary>
        /// Determines if this <see cref="CollisionShape"/> is colliding with another <see cref="ICollisionShape"/>
        /// </summary>
        /// <param name="other">The other <see cref="ICollisionShape"/></param>
        /// <returns>True when the objects overlap or touch</returns>
        public abstract bool CollidesWith(ICollisionShape other);
    }
}