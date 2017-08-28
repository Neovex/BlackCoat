﻿using System;
using System.Collections.Generic;
using System.Linq;

using SFML.System;

namespace BlackCoat.Collision
{
    /// <summary>
    /// Contains methods to calculate collisions between geometric primitives
    /// </summary>
    public class CollisionSystem
    {
        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="CollisionSystem"/> should raise exceptions.
        /// </summary>
        public Boolean RaiseCollisionExceptions { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="CollisionSystem"/> class.
        /// </summary>
        public CollisionSystem()
        { }


        // Helper methods
        /// <summary>
        /// Determines if value-range minA-maxA intersects with a second value-range minB-maxB
        /// </summary>
        protected virtual bool Intersect(float minA, float maxA, float minB, float maxB)
        {
            return minA < minB ? maxA > minB : maxB > minA;
        }

        /// <summary>
        /// Finds the shortest Vector between a Circle center and a set of localized points
        /// </summary>
        protected virtual Vector2f FindCircleProjectionAxis(IEnumerable<Vector2f> points)
        {
            return points.OrderBy(p => p.LengthSquared()).First().Normalize();
        }

        /// <summary>
        /// Projects a set of points onto an axis and returns the min and max value
        /// </summary>
        protected virtual Vector2f CalcProjectionLimits(IEnumerable<Vector2f> points, Vector2f axis)
        {
            var projections = points.Select(p => p.DotProduct(axis)).OrderBy(v => v).ToArray();
            return new Vector2f((float)projections[0], (float)projections[projections.Length - 1]);
        }

        /// <summary>
        /// Retrieves a projection axis based on a polygon side
        /// </summary>
        protected virtual Vector2f FindPolyProjectionAxis(int i, IReadOnlyList<Vector2f> points)
        {
            return points[(i + 1) % points.Count].ToLocal(points[i]).FaceVector();
        }

        /// <summary>
        /// Extracts vertices from a Rectangle
        /// </summary>
        protected virtual Vector2f[] CalcRectVerticies(IRectangle rect)
        {
            return new[]
            {
                rect.Position, // TL
                new Vector2f(rect.Position.X + rect.Size.X, rect.Position.Y), // TR
                rect.Position + rect.Size, // BR
                new Vector2f(rect.Position.X, rect.Position.Y + rect.Size.Y) // BL
            };
        }

        /// <summary>
        /// Sorts 2 values.
        /// </summary>
        private static void Sort(float a, float b, out float min, out float max)
        {
            if (a < b)
            {
                min = a;
                max = b;
            }
            else
            {
                min = b;
                max = a;
            }
        }


        //#########################################################################################################################################

        public virtual Vector2f[] Intersections(Vector2f rayOrigin, float rayAngle, ICollisionShape target)
        {
            switch (target.CollisionGeometry)
            {
                case Geometry.Point:
                    break;
                case Geometry.Line:
                    var line = target as ILine;
                    return Intersections(rayOrigin, rayAngle, line.Start, line.End);
                case Geometry.Circle:
                    break;
                case Geometry.Rectangle:
                    break;
                case Geometry.Polygon:
                    return Intersections(rayOrigin, rayAngle, target as IPolygon);
            }
            HandlUnknownShape(target);
            return new Vector2f[0];
        }

        public virtual Vector2f[] Intersections(Vector2f rayOrigin, float rayAngle, Vector2f targetStart, Vector2f targetEnd)
        {
            var localStart = targetStart.ToLocal(rayOrigin);
            var localEnd = targetEnd.ToLocal(rayOrigin);

            if (Intersect(localStart.Angle(), localEnd.Angle(), rayAngle, rayAngle))
            {
                var axis = VectorExtensions.VectorFromAngle(rayAngle).FaceVector();
                var p1 = Math.Abs(localStart.DotProduct(axis));
                var p2 = Math.Abs(localEnd.DotProduct(axis));
                var percent = (float)(p1 / (p1 + p2));
                return new Vector2f[] { targetStart + (targetEnd.ToLocal(targetStart) * percent) }; //test this
            }
            return new Vector2f[0];
        }

        public virtual Vector2f[] Intersections(Vector2f rayOrigin, float rayAngle, IPolygon target)
        {
            var angles = target.Points.Select(p => p.ToLocal(rayOrigin).Angle()).OrderBy(v => v).ToArray();

            if (Intersect(angles[0], angles[angles.Length-1], rayAngle, rayAngle))
            {
                return target.Points.SelectMany((p, i) => Intersections(rayOrigin, rayAngle, p, target.Points[(i + 1) % target.Points.Count])).ToArray();
            }
            return new Vector2f[0];
        }

        //#########################################################################################################################################

        // Collision Calculations

        //POINT
        /// <summary>
        /// Determines if objects touch or intersect.
        /// </summary>
        /// <returns>True when the objects touch or intersect.</returns>
        public virtual bool CheckCollision(Vector2f a, Vector2f b) // passed
        {
            return a.ToLocal(b).LengthSquared() <= 1;
        }

        /// <summary>
        /// Determines if objects touch or intersect.
        /// </summary>
        /// <param name="tolerance">float tolerance when checking projection values</param>
        /// <returns>True when the objects touch or intersect.</returns>
        public virtual bool CheckCollision(Vector2f a, ILine b, float tolerance = Constants.POINT_PROJECTION_TOLERANCE) // passed
        {
            var p = a.ToLocal(b.Start);
            var localEnd = b.End.ToLocal(b.Start);
            if (!(Math.Abs(p.ProjectedLength(localEnd.FaceVector())) < tolerance)) return false;

            var axis = localEnd.Normalize();
            var projectedLength = p.DotProduct(axis);
            return !(projectedLength < -tolerance || projectedLength > localEnd.DotProduct(axis) + tolerance);
        }

        /// <summary>
        /// Determines if objects touch or intersect.
        /// </summary>
        /// <returns>True when the objects touch or intersect.</returns>
        public virtual bool CheckCollision(Vector2f a, IRectangle b) // passed
        {
            return !(a.X < b.Position.X || a.Y < b.Position.Y || a.X > b.Position.X + b.Size.X || a.Y > b.Position.Y + b.Size.Y);
        }

        /// <summary>
        /// Determines if objects touch or intersect.
        /// </summary>
        /// <returns>True when the objects touch or intersect.</returns>
        public virtual bool CheckCollision(Vector2f a, ICircle b) // passed
        {
            return a.ToLocal(b.Position).LengthSquared() <= b.Radius * b.Radius;
        }

        /// <summary>
        /// Determines if objects touch or intersect.
        /// </summary>
        /// <param name="offset">Optional offset to be taken into account when comparing projection values</param>
        /// <returns>True when the objects touch or intersect.</returns>
        public virtual bool CheckCollision(Vector2f a, IPolygon b) // passed
        {
            var v = a.ToLocal(b.Position);
            return b.Points.Select((p, i) => v.ToLocal(p).DotProduct(FindPolyProjectionAxis(i, b.Points))).All(p => p > 0);
        }


        //CIRCLE
        /// <summary>
        /// Determines if objects touch or intersect.
        /// </summary>
        /// <returns>True when the objects touch or intersect.</returns>
        public virtual bool CheckCollision(ICircle a, ICircle b) // passed
        {
            var radius = a.Radius + b.Radius;
            return a.Position.ToLocal(b.Position).LengthSquared() <= radius * radius;
        }

        /// <summary>
        /// Determines if objects touch or intersect.
        /// </summary>
        /// <returns>True when the objects touch or intersect.</returns>
        public virtual bool CheckCollision(ICircle a, ILine b) // passed
        {
            var localEnd = b.End.ToLocal(b.Start);
            var axis = localEnd.Normalize();
            var aFromStart = a.Position.ToLocal(b.Start);
            var product = aFromStart.DotProduct(axis);

            if (product <= 0)
            {
                return CheckCollision(b.Start, a);
            }

            if (product >= localEnd.DotProduct(axis))
            {
                return CheckCollision(b.End, a);
            }

            product = aFromStart.DotProduct(axis.FaceVector());
            return (product - a.Radius < 0) != (product + a.Radius < 0);
        }

        /// <summary>
        /// Determines if objects touch or intersect.
        /// </summary>
        /// <returns>True when the objects touch or intersect.</returns>
        public virtual bool CheckCollision(ICircle a, IRectangle b) // passed
        {
            var rectPoints = CalcRectVerticies(b);
            if (!Intersect(a.Position.X - a.Radius, a.Position.X + a.Radius, rectPoints[0].X, rectPoints[2].X)
             || !Intersect(a.Position.Y - a.Radius, a.Position.Y + a.Radius, rectPoints[0].Y, rectPoints[2].Y)) return false;

            var localPoints = rectPoints.Select(p => p.ToLocal(a.Position)).ToArray();
            var limits = CalcProjectionLimits(localPoints, FindCircleProjectionAxis(localPoints));
            return Intersect(-a.Radius, a.Radius, limits.X, limits.Y);
        }

        /// <summary>
        /// Determines if objects touch or intersect.
        /// </summary>
        /// <returns>True when the objects touch or intersect.</returns>
        public virtual bool CheckCollision(ICircle a, IPolygon b) // passed
        {
            var localPoints = b.Points.Select(p => p.ToGlobal(b.Position)).Select(p => p.ToLocal(a.Position)).ToArray();
            if (!localPoints.Select((p, i) => (-p).DotProduct(FindPolyProjectionAxis(i, b.Points).Normalize())).All(p => p + a.Radius > 0)) return false;

            var limits = CalcProjectionLimits(localPoints, FindCircleProjectionAxis(localPoints));
            return Intersect(-a.Radius, a.Radius, limits.X, limits.Y);
        }


        //RECT
        /// <summary>
        /// Determines if objects touch or intersect.
        /// </summary>
        /// <returns>True when the objects touch or intersect.</returns>
        public virtual bool CheckCollision(IRectangle a, ILine b) // passed
        {
            var rectPoints = CalcRectVerticies(a);

            // Rough Collision Box check
            float min, max;
            Sort(b.Start.X, b.End.X, out min, out max);
            if (!Intersect(rectPoints[0].X, rectPoints[2].X, min, max)) return false;
            Sort(b.Start.Y, b.End.Y, out min, out max);
            if (!Intersect(rectPoints[0].Y, rectPoints[2].Y, min, max)) return false;

            // Projection Check
            var localEnd = b.End.ToLocal(b.Start);
            var axis = localEnd.Normalize();
            var localRectPoints = rectPoints.Select(p => p.ToLocal(b.Start)).ToArray();
            var limitF = CalcProjectionLimits(localRectPoints, axis.FaceVector());
            var limitN = CalcProjectionLimits(localRectPoints, axis);
            return Intersect(limitF.X, limitF.Y, 0, 0) &&
                   Intersect(limitN.X, limitN.Y, 0, (float)localEnd.DotProduct(axis));
        }

        /// <summary>
        /// Determines if objects touch or intersect.
        /// </summary>
        /// <returns>True when the objects touch or intersect.</returns>
        public virtual bool CheckCollision(IRectangle a, IRectangle b) // passed
        {
            return Intersect(a.Position.X, a.Position.X + a.Size.X, b.Position.X, b.Position.X + b.Size.X)
                && Intersect(a.Position.Y, a.Position.Y + a.Size.Y, b.Position.Y, b.Position.Y + b.Size.Y);
        }

        /// <summary>
        /// Determines if objects touch or intersect.
        /// </summary>
        /// <returns>True when the objects touch or intersect.</returns>
        public virtual bool CheckCollision(IRectangle a, IPolygon b) // passed
        {
            var rectPoints = CalcRectVerticies(a);
            var polyPoints = b.Points.Select(p => p.ToGlobal(b.Position)).ToArray();

            // Rough Collision Box check
            var boxCheck = polyPoints.Select(p => p.X).OrderBy(x => x).ToArray();
            if (!Intersect(rectPoints[0].X, rectPoints[2].X, boxCheck[0], boxCheck[boxCheck.Length - 1])) return false;
            boxCheck = polyPoints.Select(p => p.Y).OrderBy(y => y).ToArray();
            if (!Intersect(rectPoints[0].Y, rectPoints[2].Y, boxCheck[0], boxCheck[boxCheck.Length - 1])) return false;

            // Thorough Polygon Collision check
            return polyPoints
                   .Select((p, i) => new
                   {
                       Origin = p,
                       Axis = FindPolyProjectionAxis(i, polyPoints)
                   })
                   .Select(f => new
                   {
                       limitA = CalcProjectionLimits(rectPoints.Select(p => p.ToLocal(f.Origin)), f.Axis),
                       limitB = CalcProjectionLimits(polyPoints.Select(p => p.ToLocal(f.Origin)), f.Axis)
                   })
                   .All(limits => Intersect(limits.limitA.X, limits.limitA.Y, limits.limitB.X, limits.limitB.Y));
        }


        // POLY
        /// <summary>
        /// Determines if objects touch or intersect.
        /// </summary>
        /// <returns>True when the objects touch or intersect.</returns>
        public virtual Boolean CheckCollision(IPolygon a, IPolygon b) // passed
        {
            var aGlobalPoints = a.Points.Select(p => p.ToGlobal(a.Position)).ToArray();
            var bGlobalPoints = b.Points.Select(p => p.ToGlobal(b.Position)).ToArray();

            // Rough Collision Box check
            var boxCheckA = aGlobalPoints.Select(p => p.X).OrderBy(x => x).ToArray();
            var boxCheckB = bGlobalPoints.Select(p => p.X).OrderBy(x => x).ToArray();
            if (!Intersect(boxCheckA[0], boxCheckA[boxCheckA.Length - 1], boxCheckB[0], boxCheckB[boxCheckB.Length - 1])) return false;
            boxCheckA = aGlobalPoints.Select(p => p.Y).OrderBy(y => y).ToArray();
            boxCheckB = bGlobalPoints.Select(p => p.Y).OrderBy(y => y).ToArray();
            if (!Intersect(boxCheckA[0], boxCheckA[boxCheckA.Length - 1], boxCheckB[0], boxCheckB[boxCheckB.Length - 1])) return false;

            // Thorough Polygon Collision check
            return aGlobalPoints
                   .Select((p, i) => new
                   {
                       Origin = p,
                       Axis = FindPolyProjectionAxis(i, aGlobalPoints)
                   })
                   .Concat(bGlobalPoints
                       .Select((p, i) => new
                       {
                           Origin = p,
                           Axis = FindPolyProjectionAxis(i, bGlobalPoints)
                       }))
                   .Select(f => new
                   {
                       limitA = CalcProjectionLimits(aGlobalPoints.Select(p => p.ToLocal(f.Origin)), f.Axis),
                       limitB = CalcProjectionLimits(bGlobalPoints.Select(p => p.ToLocal(f.Origin)), f.Axis)
                   })
                   .All(limits => Intersect(limits.limitA.X, limits.limitA.Y, limits.limitB.X, limits.limitB.Y));
        }

        /// <summary>
        /// Determines if objects touch or intersect.
        /// </summary>
        /// <returns>True when the objects touch or intersect.</returns>
        public virtual bool CheckCollision(IPolygon a, ILine b) //passed
        {
            var aGlobalPoints = a.Points.Select(p => p.ToGlobal(a.Position)).ToArray();

            // Projecting all points of A and the end point of B onto the face vector of B for a quick intersection check
            var axis = b.End.ToLocal(b.Start).FaceVector();
            var axisRoot = (float)b.End.DotProduct(axis);
            var rayProjection = CalcProjectionLimits(aGlobalPoints, axis);
            if (!Intersect(axisRoot, axisRoot, rayProjection.X, rayProjection.Y)) return false;

            // Projecting the line onto each polygon projection axis
            var bPoints = new[] { b.Start, b.End };
            return aGlobalPoints
                   .Select((p, i) => new
                   {
                       Origin = p,
                       Axis = FindPolyProjectionAxis(i, aGlobalPoints)
                   })
                   .Select(f => new
                   {
                       axisRoot = f.Origin.DotProduct(f.Axis),
                       projection = CalcProjectionLimits(bPoints, f.Axis)
                   })
                   .All(limits => limits.projection.X > limits.axisRoot || limits.projection.Y > limits.axisRoot);
        }


        // LINES
        /// <summary>
        /// Determines if objects touch or intersect.
        /// </summary>
        /// <returns>True when the objects touch or intersect.</returns>
        public virtual bool CheckCollision(ILine a, ILine b) //passed
        {
            float min, max;

            //A
            var axis = a.End.ToLocal(a.Start).FaceVector();
            var p = a.End.DotProduct(axis);
            Sort((float)b.Start.DotProduct(axis), (float)b.End.DotProduct(axis), out min, out max);
            if (min > p || max < p) return false;

            //B
            axis = b.End.ToLocal(b.Start).FaceVector();
            p = b.End.DotProduct(axis);
            Sort((float)a.Start.DotProduct(axis), (float)a.End.DotProduct(axis), out min, out max);
            return min <= p && max >= p;
        }


        //#########################################################################################################################################

        // MAPPERS
        /// <summary>
        /// Determines if objects touch or intersect.
        /// </summary>
        /// <returns>True when the objects touch or intersect.</returns>
        public virtual bool CheckCollision(ICollisionShape self, ICollisionShape other)
        {
            switch (self.CollisionGeometry)
            {
                case Geometry.Line: return CheckCollision(self as ILine, other);
                case Geometry.Circle: return CheckCollision(self as ICircle, other);
                case Geometry.Rectangle: return CheckCollision(self as IRectangle, other);
                case Geometry.Polygon: return CheckCollision(self as IPolygon, other);
            }
            return HandlUnknownShape(self);
        }

        /// <summary>
        /// Determines if objects touch or intersect.
        /// </summary>
        /// <returns>True when the objects touch or intersect.</returns>
        public virtual bool CheckCollision(ILine line, ICollisionShape other)
        {
            switch (other.CollisionGeometry)
            {
                case Geometry.Line: return CheckCollision(line, other as ILine);
                case Geometry.Circle: return CheckCollision(other as ICircle, line);
                case Geometry.Rectangle: return CheckCollision(other as IRectangle, line);
                case Geometry.Polygon: return CheckCollision(other as IPolygon, line);
            }
            return HandlUnknownShape(other);
        }
        /// <summary>
        /// Determines if objects touch or intersect.
        /// </summary>
        /// <returns>True when the objects touch or intersect.</returns>
        public virtual bool CheckCollision(ICircle circle, ICollisionShape other)
        {
            switch (other.CollisionGeometry)
            {
                case Geometry.Line: return CheckCollision(circle, other as ILine);
                case Geometry.Circle: return CheckCollision(circle, other as ICircle);
                case Geometry.Rectangle: return CheckCollision(circle, other as IRectangle);
                case Geometry.Polygon: return CheckCollision(circle, other as IPolygon);
            }
            return HandlUnknownShape(other);
        }
        /// <summary>
        /// Determines if objects touch or intersect.
        /// </summary>
        /// <returns>True when the objects touch or intersect.</returns>
        public virtual bool CheckCollision(IRectangle rect, ICollisionShape other)
        {
            switch (other.CollisionGeometry)
            {
                case Geometry.Line: return CheckCollision(rect, other as ILine);
                case Geometry.Circle: return CheckCollision(other as ICircle, rect);
                case Geometry.Rectangle: return CheckCollision(rect, other as IRectangle);
                case Geometry.Polygon: return CheckCollision(rect, other as IPolygon);
            }
            return HandlUnknownShape(other);
        }
        /// <summary>
        /// Determines if objects touch or intersect.
        /// </summary>
        /// <returns>True when the objects touch or intersect.</returns>
        public virtual bool CheckCollision(IPolygon poly, ICollisionShape other)
        {
            switch (other.CollisionGeometry)
            {
                case Geometry.Line: return CheckCollision(poly, other as ILine);
                case Geometry.Circle: return CheckCollision(other as ICircle, poly);
                case Geometry.Rectangle: return CheckCollision(other as IRectangle, poly);
                case Geometry.Polygon: return CheckCollision(poly, other as IPolygon);
            }
            return HandlUnknownShape(other);
        }

        // Helper method to handle invalid shape combinations - should never happen
        protected virtual bool HandlUnknownShape(ICollisionShape shape)
        {
            Log.Error("Invalid collision shape", shape, shape?.CollisionGeometry);
            if (RaiseCollisionExceptions) throw new Exception("Invalid collision shape");
            return false;
        }
    }
}