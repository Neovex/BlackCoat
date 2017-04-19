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
        /// <summary>
        /// Determines if valuerange minA-maxA intersects with a second valuerange minB-maxB
        /// </summary>
        protected bool Intersect(float minA, float maxA, float minB, float maxB)
        {
            return !(minA < minB ? minB > maxA : minA > maxB);
        }

        // Finds the shortest Vector between a Circle center and a set of points
        protected Vector2f FindCircleProjectionAxis(ICircle a, IEnumerable<Vector2f> points)
        {
            return points.AsParallel().OrderBy(p => p.ToLocal(a.Position).LengthSquared()).First().Normalize();
        }

        // Projects a set of of points onto an axis and returns the min and max value
        protected Vector2f CalcProjectionLimits(IEnumerable<Vector2f> points, Vector2f axis)
        {
            var projections = points.AsParallel().Select(p => p.DotProduct(axis)).OrderBy(v => v).ToArray();
            return new Vector2f((float)projections[0], (float)projections[projections.Length - 1]);
        }

        protected Vector2f FindPolyProjectionAxis(int i, IReadOnlyList<Vector2f> points)
        {
            return (i == points.Count - 1 ? points[0] : points[i + 1]).ToLocal(points[i]).FaceVector();
        }

        protected Vector2f[] CalcVerticies(IRectangle rect)
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
        public virtual bool CheckCollision(Vector2f a, Vector2f b)
        {
            return a.ToLocal(b).LengthSquared() <= 1;
        }

        public virtual bool CheckCollision(Vector2f a, ILine b)
        {
            var p = a.ToLocal(b.Start);
            var localEnd = b.End.ToLocal(b.Start);
            if (p.ProjectedLength(localEnd.FaceVector()) != 0) return false;

            var axis = localEnd.Normalize();
            var projectedLength = p.DotProduct(localEnd);
            return !(projectedLength < 0 || projectedLength > localEnd.DotProduct(axis));
        }

        public virtual bool CheckCollision(Vector2f a, ICircle b)
        {
            return a.ToLocal(b.Position).LengthSquared() <= b.Radius * b.Radius;
        }

        public virtual bool CheckCollision(Vector2f a, IRectangle b)
        {
            return !(a.X < b.Position.X || a.Y < b.Position.Y || a.X > b.Position.X + b.Size.X || a.Y > b.Position.Y + b.Size.Y);
        }

        public virtual bool CheckCollision(Vector2f a, IPoly b, double offset = 0)
        {
            var v = a.ToLocal(b.Position);
            return !b.Points.AsParallel().Select((p, i) => v.ToLocal(p).DotProduct(FindPolyProjectionAxis(i, b.Points))).Any(p => p + offset > 0);
        }


        //CIRCLE
        public virtual bool CheckCollision(ICircle a, ICircle b)
        {
            return a.Radius * a.Radius + b.Radius * b.Radius <= a.Position.ToLocal(b.Position).LengthSquared();
        }

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

        public virtual bool CheckCollision(ICircle a, IRectangle b)
        {
            var rectPoints = CalcVerticies(b);
            var limits = CalcProjectionLimits(rectPoints.AsParallel().Select(p => p.ToLocal(a.Position)), FindCircleProjectionAxis(a, rectPoints));

            return Intersect(-a.Radius, a.Radius, limits.X, limits.Y)
                && Intersect(a.Position.X - a.Radius, a.Position.X + a.Radius, rectPoints[0].X, rectPoints[2].X)
                && Intersect(a.Position.Y - a.Radius, a.Position.Y + a.Radius, rectPoints[0].Y, rectPoints[2].Y);
        }

        public virtual bool CheckCollision(ICircle circle, IRectangle rect, bool alternate)
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

        public virtual bool CheckCollision(ICircle a, IPoly b)
        {
            var pGlobalPoints = b.Points.AsParallel().Select(p => p.ToGlobal(b.Position)).ToArray();
            var limits = CalcProjectionLimits(pGlobalPoints.AsParallel().Select(p => p.ToLocal(a.Position)), FindCircleProjectionAxis(a, pGlobalPoints));
            return Intersect(-a.Radius, a.Radius, limits.X, limits.Y) && CheckCollision(a.Position, b, -a.Radius);
        }


        //RECT
        public virtual bool CheckCollision(IRectangle a, ILine b)
        {
            var localEnd = b.End.ToLocal(b.Start);
            var axis = localEnd.Normalize();
            var localRectPoints = CalcVerticies(a).AsParallel().Select(p => p.ToLocal(b.Start)).ToArray();
            var limitF = CalcProjectionLimits(localRectPoints, axis.FaceVector());
            var limitN = CalcProjectionLimits(localRectPoints, axis);
            return Intersect(limitF.X, limitF.Y, 0, 0) &&
                   Intersect(limitN.X, limitN.Y, 0, (float)localEnd.DotProduct(axis));
        }

        public virtual bool CheckCollision(IRectangle a, IRectangle b)
        {
            return Intersect(a.Position.X, a.Position.X + a.Size.X, b.Position.X, b.Position.X + b.Size.X)
                && Intersect(a.Position.Y, a.Position.Y + a.Size.Y, b.Position.Y, b.Position.Y + b.Size.Y);
        }

        public virtual bool CheckCollision(IRectangle a, IPoly b)
        {
            var rectPoints = CalcVerticies(a);
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
        public virtual Boolean CheckCollision(IPoly a, IPoly b)
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

        public virtual bool CheckCollision(IPoly a, ILine b)
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
    }
}