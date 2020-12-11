using System;
using System.Linq;
using System.Collections.Generic;

using SFML.System;
using SFML.Graphics;

using BlackCoat.Collision;

namespace BlackCoat.Entities.Shapes
{
    /// <summary>
    /// Represents a drawable collection of Vertices
    /// </summary>
    public class FreeShape : EntityBase
    {
        // Variables #######################################################################
        private readonly List<Vertex> _VertexList;
        private Vertex[] _Vertices;
        private bool _Dirty;
        private Vector2f _Position;
        private float _Rotation;
        private Vector2f _Origin;
        private Vector2f _Scale = new Vector2f(1, 1);


        // Properties ######################################################################
        /// <summary>
        /// Gets or sets the collision shape for collision detection
        /// </summary>
        public ICollisionShape CollisionShape { get; set; }

        public override Vector2f Position
        {
            get => _Position;
            set
            {
                _Position = value;
                _Dirty = true;
            }
        }
        public override float Rotation
        {
            get => _Rotation;
            set
            {
                _Rotation = value;
                _Dirty = true;
            }
        }
        public override Vector2f Origin
        {
            get => _Origin;
            set
            {
                _Origin = value;
                _Dirty = true;
            }
        }
        public override Vector2f Scale
        {
            get => _Scale;
            set
            {
                _Scale = value;
                _Dirty = true;
            }
        }
        /// <summary>
        /// Vertex Color
        /// </summary>
        public override Color Color
        {
            get => _Vertices[0].Color;
            set
            {
                for (int i = 0; i < _Vertices.Length; i++)
                {
                    _Vertices[i].Color = value;
                    _VertexList[i] = _Vertices[i];
                }
            }
        }

        /// <summary>
        /// Always False. A <see cref="FreeShape"/> cannot be explicitly disposed.
        /// </summary>
        public override bool Disposed => false;

        /// <summary>
        /// Amount of Vertices this <see cref="FreeShape"/> is made of.
        /// </summary>
        public int VertexCount => _VertexList.Count;

        /// <summary>
        /// Gets or sets the <see cref="Vertex"/> at the specified index.
        /// </summary>
        public Vertex this[int index]
        {
            get
            {
                if (index < 0 || index >= VertexCount) throw new IndexOutOfRangeException();
                return _VertexList[index];
            }
            set
            {
                if (index < 0 || index >= VertexCount) throw new IndexOutOfRangeException();
                _VertexList[index] = value;
                _Dirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="PrimitiveType"/> for rendering.
        /// </summary>
        public PrimitiveType Primitive { get; set; }


        // CTOR ############################################################################
        /// <summary>
        /// Creates a <see cref="FreeShape" /> instance.
        /// </summary>
        /// <param name="core">Engine Core</param>
        /// <param name="primitive">The <see cref="PrimitiveType"/> for rendering</param>
        /// <param name="points">A <see cref="Vector2f" /> collection that creates the outline of the <see cref="FreeShape" /></param>
        /// <param name="color">Shape color</param>
        /// <exception cref="ArgumentNullException">points</exception>
        public FreeShape(Core core, PrimitiveType primitive, IEnumerable<Vector2f> points, Color color) : base(core)
        {
            Primitive = primitive;
            if (points == null) throw new ArgumentNullException(nameof(points));
            _VertexList = new List<Vertex>(points.Select(p => new Vertex(p, color)));
            UpdateVertexArray();
        }

        /// <summary>
        /// Creates a <see cref="FreeShape"/> instance.
        /// </summary>
        /// <param name="core">Engine Core</param>
        /// <param name="primitive">The <see cref="PrimitiveType"/> for rendering</param>
        /// <param name="vertices">A <see cref="Vertex"/> collection that represents the contents of the <see cref="FreeShape"/></param>
        public FreeShape(Core core, PrimitiveType primitive, IEnumerable<Vertex> vertices) : base(core)
        {
            Primitive = primitive;
            if (vertices == null) throw new ArgumentNullException(nameof(vertices));
            _VertexList = new List<Vertex>(vertices);
            UpdateVertexArray();
        }


        // Methods #########################################################################
        /// <summary>
        /// Draws the <see cref="FreeShape"/> respective to its parameters and view
        /// </summary>
        public override void Draw(RenderTarget target, RenderStates states)
        {
            if (_Dirty) UpdateVertexArray();
            target.Draw(_Vertices, Primitive, states);
        }

        public void Add(Vector2f position, Color color, int index = -1)
        {
            _VertexList.Insert(index < 0 ? _VertexList.Count : index, new Vertex(position, color));
            _Dirty = true;
        }

        public void Add(Vertex vertex, int index = -1)
        {
            _VertexList.Insert(index < 0 ? _VertexList.Count : index, vertex);
            _Dirty = true;
        }

        public void Remove(int index)
        {
            _VertexList.RemoveAt(index);
            _Dirty = true;
        }

        private void UpdateVertexArray()
        {
            _Vertices = _VertexList.ToArray();
            for (int i = 0; i < _Vertices.Length; i++)
            {
                var local = (_Vertices[i].Position - Origin).MultiplyBy(Scale);
                if (Rotation != 0)
                {
                    local = Create.Vector2fFromAngle(local.Angle() + Rotation, (float)local.Length());
                }
                _Vertices[i].Position = local + Position;
            }
            _Dirty = false;
        }
    }
}