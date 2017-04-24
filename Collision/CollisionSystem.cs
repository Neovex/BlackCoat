using System;
using System.Collections.Generic;
using System.Linq;

using SFML.System;

namespace BlackCoat.Collision
{
    /// <summary>
    /// Contains methods to calculate collisions between geometric primitves
    /// </summary>
    public class CollisionSystem
    {
        public Boolean RaiseCollisionExceptions { get; set; }


        /// <summary>
        /// Determines if valuerange minA-maxA intersects with a second valuerange minB-maxB
        /// </summary>
        protected virtual bool Intersect(float minA, float maxA, float minB, float maxB)
        {
            return !(minA < minB ? minB > maxA : minA > maxB);
        }

        // Finds the shortest Vector between a Circle center and a set of points
        protected virtual Vector2f FindCircleProjectionAxis(ICircle a, IEnumerable<Vector2f> points)
        {
            return points.AsParallel().OrderBy(p => p.ToLocal(a.Position).LengthSquared()).First().Normalize();
        }

        // Projects a set of of points onto an axis and returns the min and max value
        protected virtual Vector2f CalcProjectionLimits(IEnumerable<Vector2f> points, Vector2f axis)
        {
            var projections = points.AsParallel().Select(p => p.DotProduct(axis)).OrderBy(v => v).ToArray();
            return new Vector2f((float)projections[0], (float)projections[projections.Length - 1]);
        }

        // Retrieves a projection axis based on a polygon side
        protected virtual Vector2f FindPolyProjectionAxis(int i, IReadOnlyList<Vector2f> points)
        {
            return (i == points.Count - 1 ? points[0] : points[i + 1]).ToLocal(points[i]).FaceVector();
        }

        // Extracts verticies from a Rectangle
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

        //POINT
        /// <summary>
        /// Determines if objects touch or intersect.
        /// </summary>
        /// <returns>True when the objects touch or intersect.</returns>
        public virtual bool CheckCollision(Vector2f a, Vector2f b)
        {
            return a.ToLocal(b).LengthSquared() <= 1;
        }

        /// <summary>
        /// Determines if objects touch or intersect.
        /// </summary>
        /// <returns>True when the objects touch or intersect.</returns>
        public virtual bool CheckCollision(Vector2f a, ILine b)
        {
            var p = a.ToLocal(b.Start);
            var localEnd = b.End.ToLocal(b.Start);
            if (p.ProjectedLength(localEnd.FaceVector()) != 0) return false;

            var axis = localEnd.Normalize();
            var projectedLength = p.DotProduct(localEnd);
            return !(projectedLength < 0 || projectedLength > localEnd.DotProduct(axis));
        }

        /// <summary>
        /// Determines if objects touch or intersect.
        /// </summary>
        /// <returns>True when the objects touch or intersect.</returns>
        public virtual bool CheckCollision(Vector2f a, ICircle b)
        {
            return a.ToLocal(b.Position).LengthSquared() <= b.Radius * b.Radius;
        }

        /// <summary>
        /// Determines if objects touch or intersect.
        /// </summary>
        /// <returns>True when the objects touch or intersect.</returns>
        public virtual bool CheckCollision(Vector2f a, IRectangle b)
        {
            return !(a.X < b.Position.X || a.Y < b.Position.Y || a.X > b.Position.X + b.Size.X || a.Y > b.Position.Y + b.Size.Y);
        }

        /// <summary>
        /// Determines if objects touch or intersect.
        /// </summary>
        /// <param name="offset">Optional offset to be taken into account when comparing projection values</param>
        /// <returns>True when the objects touch or intersect.</returns>
        public virtual bool CheckCollision(Vector2f a, IPolygon b, double offset = 0)
        {
            var v = a.ToLocal(b.Position);
            return !b.Points.AsParallel().Select((p, i) => v.ToLocal(p).DotProduct(FindPolyProjectionAxis(i, b.Points))).Any(p => p + offset > 0);
        }


        //CIRCLE
        /// <summary>
        /// Determines if objects touch or intersect.
        /// </summary>
        /// <returns>True when the objects touch or intersect.</returns>
        public virtual bool CheckCollision(ICircle a, ICircle b)
        {
            return a.Radius * a.Radius + b.Radius * b.Radius <= a.Position.ToLocal(b.Position).LengthSquared();
        }

        /// <summary>
        /// Determines if objects touch or intersect.
        /// </summary>
        /// <returns>True when the objects touch or intersect.</returns>
        public virtual bool CheckCollision(ICircle a, ILine b)
        {
            var localEnd = b.End.ToLocal(b.Start);
            var axis = localEnd.Normalize();
            var localCircleCenter = a.Position.ToLocal(b.Start);
            var productF = (float)localCircleCenter.DotProduct(axis.FaceVector());
            var productN = (float)localCircleCenter.DotProduct(axis);
            return Intersect(productF - a.Radius, productF + a.Radius, 0, 0) &&
                   Intersect(productN - a.Radius, productN + a.Radius, 0, (float)localEnd.DotProduct(axis));
        }

        /// <summary>
        /// Determines if objects touch or intersect.
        /// </summary>
        /// <returns>True when the objects touch or intersect.</returns>
        public virtual bool CheckCollision(ICircle a, IRectangle b)
        {
            var rectPoints = CalcRectVerticies(b);
            var limits = CalcProjectionLimits(rectPoints.AsParallel().Select(p => p.ToLocal(a.Position)), FindCircleProjectionAxis(a, rectPoints));

            return Intersect(-a.Radius, a.Radius, limits.X, limits.Y)
                && Intersect(a.Position.X - a.Radius, a.Position.X + a.Radius, rectPoints[0].X, rectPoints[2].X)
                && Intersect(a.Position.Y - a.Radius, a.Position.Y + a.Radius, rectPoints[0].Y, rectPoints[2].Y);
        }

        /*
        public virtual bool CheckCollision(ICircle circle, IRectangle rect)
        {
            var circleDistance = new Vector2f();
            circleDistance.X = Math.Abs(circle.Position.X - rect.Position.X);
            circleDistance.Y = Math.Abs(circle.Position.Y - rect.Position.Y);
            var rectSizeHalfed = rect.Size / 2;

            if (circleDistance.X > (rectSizeHalfed.X + circle.Radius)) { return false; }
            if (circleDistance.Y > (rectSizeHalfed.Y + circle.Radius)) { return false; }

            if (circleDistance.X <= (rectSizeHalfed.X)) { return true; }
            if (circleDistance.Y <= (rectSizeHalfed.Y)) { return true; }

            var cornerDistance_sq = Math.Pow(circleDistance.X - rectSizeHalfed.X, 2) +
                                    Math.Pow(circleDistance.Y - rectSizeHalfed.Y, 2);

            return (cornerDistance_sq <= (circle.Radius * circle.Radius));
        }
        */

        /// <summary>
        /// Determines if objects touch or intersect.
        /// </summary>
        /// <returns>True when the objects touch or intersect.</returns>
        public virtual bool CheckCollision(ICircle a, IPolygon b)
        {
            var pGlobalPoints = b.Points.AsParallel().Select(p => p.ToGlobal(b.Position)).ToArray();
            var limits = CalcProjectionLimits(pGlobalPoints.AsParallel().Select(p => p.ToLocal(a.Position)), FindCircleProjectionAxis(a, pGlobalPoints));
            return Intersect(-a.Radius, a.Radius, limits.X, limits.Y) && CheckCollision(a.Position, b, -a.Radius);
        }


        //RECT
        /// <summary>
        /// Determines if objects touch or intersect.
        /// </summary>
        /// <returns>True when the objects touch or intersect.</returns>
        public virtual bool CheckCollision(IRectangle a, ILine b)
        {
            var localEnd = b.End.ToLocal(b.Start);
            var axis = localEnd.Normalize();
            var localRectPoints = CalcRectVerticies(a).AsParallel().Select(p => p.ToLocal(b.Start)).ToArray();
            var limitF = CalcProjectionLimits(localRectPoints, axis.FaceVector());
            var limitN = CalcProjectionLimits(localRectPoints, axis);
            return Intersect(limitF.X, limitF.Y, 0, 0) &&
                   Intersect(limitN.X, limitN.Y, 0, (float)localEnd.DotProduct(axis));
        }

        /// <summary>
        /// Determines if objects touch or intersect.
        /// </summary>
        /// <returns>True when the objects touch or intersect.</returns>
        public virtual bool CheckCollision(IRectangle a, IRectangle b)
        {
            return Intersect(a.Position.X, a.Position.X + a.Size.X, b.Position.X, b.Position.X + b.Size.X)
                && Intersect(a.Position.Y, a.Position.Y + a.Size.Y, b.Position.Y, b.Position.Y + b.Size.Y);
        }

        /// <summary>
        /// Determines if objects touch or intersect.
        /// </summary>
        /// <returns>True when the objects touch or intersect.</returns>
        public virtual bool CheckCollision(IRectangle a, IPolygon b)
        {
            var rectPoints = CalcRectVerticies(a);
            var polyPoints = b.Points.AsParallel().Select(p => p.ToGlobal(b.Position)).ToArray();

            // Rough Collision Box check
            var boxCheck = polyPoints.AsParallel().Select(p => p.X).OrderBy(x => x).ToArray();
            if (!Intersect(rectPoints[0].X, rectPoints[2].X, boxCheck[0], boxCheck[boxCheck.Length - 1])) return false;
            boxCheck = polyPoints.AsParallel().Select(p => p.Y).OrderBy(y => y).ToArray();
            if (!Intersect(rectPoints[0].Y, rectPoints[2].Y, boxCheck[0], boxCheck[boxCheck.Length - 1])) return false;

            // Thorough Poligon Collision check
            return !polyPoints.AsParallel()
                .Select((p, i) => new
                {
                    Origin = p,
                    Axis = FindPolyProjectionAxis(i, polyPoints)
                })
                .Select(f => new
                {
                    limitA = CalcProjectionLimits(rectPoints.AsParallel().Select(p => p.ToLocal(f.Origin)), f.Axis),
                    limitB = CalcProjectionLimits(polyPoints.AsParallel().Select(p => p.ToLocal(f.Origin)), f.Axis)
                })
                .Any(limits => Intersect(limits.limitA.X, limits.limitA.Y, limits.limitB.X, limits.limitB.Y));
        }


        // POLY
        /// <summary>
        /// Determines if objects touch or intersect.
        /// </summary>
        /// <returns>True when the objects touch or intersect.</returns>
        public virtual Boolean CheckCollision(IPolygon a, IPolygon b)
        {
            var aGlobalPoints = a.Points.AsParallel().Select(p => p.ToGlobal(a.Position)).ToArray();
            var bGlobalPoints = b.Points.AsParallel().Select(p => p.ToGlobal(b.Position)).ToArray();

            // Rough Collision Box check
            var boxCheckA = aGlobalPoints.AsParallel().Select(p => p.X).OrderBy(x => x).ToArray();
            var boxCheckB = bGlobalPoints.AsParallel().Select(p => p.X).OrderBy(x => x).ToArray();
            if (!Intersect(boxCheckA[0], boxCheckA[boxCheckA.Length - 1], boxCheckB[0], boxCheckB[boxCheckB.Length - 1])) return false;
            boxCheckA = aGlobalPoints.AsParallel().Select(p => p.Y).OrderBy(y => y).ToArray();
            boxCheckB = bGlobalPoints.AsParallel().Select(p => p.Y).OrderBy(y => y).ToArray();
            if (!Intersect(boxCheckA[0], boxCheckA[boxCheckA.Length - 1], boxCheckB[0], boxCheckB[boxCheckB.Length - 1])) return false;

            // Thorough Poligon Collision check
            return !aGlobalPoints.AsParallel()
                .Select((p, i) => new
                {
                    Origin = p,
                    Axis = FindPolyProjectionAxis(i, aGlobalPoints)
                })
                .Concat(bGlobalPoints.AsParallel()
                    .Select((p, i) => new
                    {
                        Origin = p,
                        Axis = FindPolyProjectionAxis(i, bGlobalPoints)
                    }))
                .Select(f => new
                {
                    limitA = CalcProjectionLimits(aGlobalPoints.AsParallel().Select(p => p.ToLocal(f.Origin)), f.Axis),
                    limitB = CalcProjectionLimits(bGlobalPoints.AsParallel().Select(p => p.ToLocal(f.Origin)), f.Axis)
                })
                .Any(limits => Intersect(limits.limitA.X, limits.limitA.Y, limits.limitB.X, limits.limitB.Y));
        }

        /// <summary>
        /// Determines if objects touch or intersect.
        /// </summary>
        /// <returns>True when the objects touch or intersect.</returns>
        public virtual bool CheckCollision(IPolygon a, ILine b)
        {
            var localEnd = b.End.ToLocal(b.Start);
            var axis = localEnd.Normalize();
            var localPolyPoints = a.Points.AsParallel().Select(p => p.ToGlobal(a.Position).ToLocal(b.Start)).ToArray();
            var limitF = CalcProjectionLimits(localPolyPoints, axis.FaceVector());
            var limitN = CalcProjectionLimits(localPolyPoints, axis);
            return Intersect(limitF.X, limitF.Y, 0, 0) &&
                   Intersect(limitN.X, limitN.Y, 0, (float)localEnd.DotProduct(axis));
        }


        // LINES
        /// <summary>
        /// Determines if objects touch or intersect.
        /// </summary>
        /// <returns>True when the objects touch or intersect.</returns>
        public virtual bool CheckCollision(ILine a, ILine b)
        {
            var localEnd = a.End.ToLocal(a.Start);
            var axis = localEnd.FaceVector().Normalize();
            var limits = CalcProjectionLimits(new[] { b.Start.ToLocal(a.Start), b.End.ToLocal(a.Start) }, axis);
            if (!Intersect(0, (float)localEnd.DotProduct(axis), limits.X, limits.Y)) return false;
            // consider encapsulation to counter code redundancy
            localEnd = b.End.ToLocal(b.Start);
            axis = localEnd.FaceVector().Normalize();
            limits = CalcProjectionLimits(new[] { a.Start.ToLocal(b.Start), a.End.ToLocal(b.Start) }, axis);
            return Intersect(0, (float)localEnd.DotProduct(axis), limits.X, limits.Y);
        }


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

        // Helpermethod to handle invalid shape combinations - should never happen
        protected virtual bool HandlUnknownShape(ICollisionShape shape)
        {
            Log.Error("Invalid collision shape", shape, shape?.CollisionGeometry);
            if (RaiseCollisionExceptions) throw new Exception("Invalid collision shape");
            return false;
        }
    }
}