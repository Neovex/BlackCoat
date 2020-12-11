using SFML.System;
using SFML.Graphics;

namespace BlackCoat
{
    /// <summary>
    /// Shared interface of all transformable types
    /// </summary>
    public interface ITransformable
    {
        /// <summary>
        /// The origin of an object defines the center point for
        /// all transformations (position, scale, rotation).
        /// The coordinates of this point must be relative to the
        /// top-left corner of the object, and ignore all
        /// transformations (position, scale, rotation).
        /// </summary>
        Vector2f Origin { get; set; }

        /// <summary>
        /// Location of the object within its parent container
        /// </summary>
        Vector2f Position { get; set; }
        
        /// <summary>
        /// Rotation of the object
        /// </summary>
        float Rotation { get; set; }
        
        /// <summary>
        /// Scale of the object
        /// </summary>
        Vector2f Scale { get; set; }
        
        /// <summary>
        /// The combined transform of the object
        /// </summary>
        Transform Transform { get; }

        /// <summary>
        /// The inverse of the combined transform of the object
        /// </summary>
        Transform InverseTransform { get; }
    }
}