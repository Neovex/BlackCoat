﻿using System;
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
    public class Polygon : ShapeEntity<ConvexShape>, IPolygon
    {
        // Properties ######################################################################
        /// <summary>
        /// Vectors this <see cref="Polygon"/> is composed of. Read Only.
        /// </summary>
        public IReadOnlyList<Vector2f> Points => EnumeratePoints().ToList();

        /// <summary>
        /// Gets the <see cref="Vector2f"/> of the <see cref="Polygon"/> at the specified index.
        /// </summary>
        public Vector2f this[uint index]
        {
            get => Target.GetPoint(index);
            set
            {
                var count = Target.GetPointCount();
                if (index >= count) Target.SetPointCount(count + 1);
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
        public Polygon(Core core, IEnumerable<Vector2f> points = null):base(core, new ConvexShape((uint)points.Count()))
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