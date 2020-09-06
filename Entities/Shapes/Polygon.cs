using System;
using System.Linq;
using System.Collections.Generic;
using SFML.System;
using SFML.Graphics;
using BlackCoat.Collision;

namespace BlackCoat.Entities.Shapes
{
    /// <summary>
    /// Represents a convex Polygon with a fixed amount of points
    /// </summary>
    public class Polygon : ShapeEntity<ConvexShape>, IPolygon
    {
        // Properties ######################################################################
        /// <summary>
        /// Retrieves a <see cref="IReadOnlyList{Vector2f}"/> containing the points this <see cref="Polygon"/> is composed of.
        /// </summary>
        public IReadOnlyList<Vector2f> Points => EnumeratePoints().ToList();

        /// <summary>
        /// Gets or sets the <see cref="Vector2f"/> at the specified index.
        /// </summary>
        public Vector2f this[uint index]
        {
            get => Target.GetPoint(index);
            set
            {
                var count = Target.GetPointCount();
                if (index >= count) throw new IndexOutOfRangeException();
                Target.SetPoint(index, value);
            }
        }

        /// <summary>
        /// Determines the geometric primitive used for collision detection
        /// </summary>
        public override Geometry CollisionGeometry => Geometry.Polygon;


        // CTOR ############################################################################
        /// <summary>
        /// Creates a new <see cref="Polygon"/> instance
        /// </summary>
        /// <param name="core">Engine Core</param>
        /// <param name="points">The points this <see cref="Polygon"/> is composed of</param>
        /// <param name="fillColor">Color of the <see cref="Polygon"/>s area.</param>
        /// <param name="outlineColor">Color of the <see cref="Polygon"/>s outline.</param>
        public Polygon(Core core, IEnumerable<Vector2f> points = null, Color? fillColor = null, Color? outlineColor = null) :base(core, new ConvexShape((uint)points.Count()), fillColor, outlineColor)
        {
            if (points != null)
            {
                uint i = 0;
                foreach (var p in points)
                {
                    Target.SetPoint(i++, p);
                }
            }
        }


        // Methods #########################################################################
        private IEnumerable<Vector2f> EnumeratePoints()
        {
            for (uint i = 0; i < Target.GetPointCount(); i++)
            {
                yield return Target.GetPoint(i);
            }
        }
    }
}