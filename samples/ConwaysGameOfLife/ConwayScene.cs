using System;
using System.Collections.Generic;
using System.Linq;

using SFML.System;
using SFML.Graphics;

using BlackCoat;
using BlackCoat.Entities.Shapes;


namespace ConwaysGameOfLife
{
    class ConwayScene : Scene
    {
        private Vector2i _GridSize;
        private bool[,] _Map;
        private FreeShape _MapDisplay;
        private float _TimeTillUpdate;


        public float UpdateImpulse { get; set; }
        public float FadeIn { get; set; }
        public float FadeOut { get; set; }


        public ConwayScene(Core core) : base(core)
        {
            UpdateImpulse = 0.1f;
            FadeIn = 900;
            FadeOut = 600;
        }


        protected override bool Load()
        {
            // Create Data
            var cellSize = CalcCellSize(_Core.DeviceSize.X, _Core.DeviceSize.Y, 0.8f);
            _GridSize = (_Core.DeviceSize / cellSize).ToVector2i();
            _Map = new bool[_GridSize.X, _GridSize.Y];
            InitializeDataMap(_GridSize, 0.1f);

            // Create Display
            _MapDisplay = new FreeShape(_Core, PrimitiveType.Quads, Array.Empty<Vertex>());
            Layer_Game.Add(_MapDisplay);

            // Create Vertices
            var c = Color.Cyan;
            for (int y = 0; y < _GridSize.Y; y++)
            {
                for (int x = 0; x < _GridSize.X; x++)
                {
                    _MapDisplay.Add(new Vertex(new Vector2f(x * cellSize, y * cellSize), c));
                    _MapDisplay.Add(new Vertex(new Vector2f(x * cellSize + cellSize, y * cellSize), c));
                    _MapDisplay.Add(new Vertex(new Vector2f(x * cellSize + cellSize, y * cellSize + cellSize), c));
                    _MapDisplay.Add(new Vertex(new Vector2f(x * cellSize, y * cellSize + cellSize), c));
                }
            }
#if DEBUG
            OpenInspector();
#endif
            return true;
        }

        protected override void Update(float deltaT)
        {
            _TimeTillUpdate -= deltaT;

            if (Input.LeftMouseButtonPressed)
            {
                // Mouse Drawing
                var m = new Vector2f(MathHelper.Clamp(Input.MousePosition.X, 0, _Core.DeviceSize.X - 1),
                                     MathHelper.Clamp(Input.MousePosition.Y, 0, _Core.DeviceSize.Y - 1));
                _Map[(int)(m.X / _Core.DeviceSize.X * _GridSize.X),
                     (int)(m.Y / _Core.DeviceSize.Y * _GridSize.Y)] = true;
            }
            else if (_TimeTillUpdate <= 0)
            {
                // Update data map on every update impulse except when mouse is down
                UpdateDataMap();
                _TimeTillUpdate = UpdateImpulse;
            }


            // Update Display
            for (int y = 0; y < _GridSize.Y; y++)
            {
                for (int x = 0; x < _GridSize.X; x++)
                {
                    var index = (x + y * _GridSize.X) * 4;
                    var blend = (_Map[x, y] ? FadeIn : -FadeOut) * deltaT;

                    for (int k = 0; k < 4; k++)
                    {
                        var vertex = _MapDisplay[index + k];
                        vertex.Color.A = (byte)MathHelper.Clamp(vertex.Color.A + blend, Byte.MinValue, Byte.MaxValue);
                        _MapDisplay[index + k] = vertex;
                    }
                }
            }
        }

        protected override void Destroy()
        {
            // Intentionally empty. There is no need for explicit destruction.
        }


        // HELPERS:

        private int CalcCellSize(float x, float y, float size)
        {
            var min = (int)Math.Min(x, y);
            var lst = new List<int>();
            for (int i = min - 1; i >= 1; i--)
            {
                if (x % i + y % i == 0) lst.Add(i);
            }

            var index = (int)MathHelper.Clamp(lst.Count * size, 0, lst.Count - 1);
            return lst[index];
        }

        private void InitializeDataMap(Vector2i cells, float fillRatio)
        {
            Array.Clear(_Map, 0, _Map.Length);
            var max = cells - new Vector2i(1, 1);
            for (int i = 0; i < (cells.X * cells.Y) * fillRatio; i++)
            {
                Vector2i rnd;
                do
                {
                    rnd = _Core.Random.NextVector(default, max.ToVector2f()).ToVector2i();
                }
                while (_Map[rnd.X, rnd.Y]);

                _Map[rnd.X, rnd.Y] = true;
            }
        }

        private void UpdateDataMap()
        {
            var newMap = new bool[_GridSize.X, _GridSize.Y];

            for (int y = 0; y < _GridSize.Y; y++)
            {
                for (int x = 0; x < _GridSize.X; x++)
                {
                    var aliveNeighbors = GetNeighbors(x, y).Count(c => _Map[c.x, c.y]);
                    // Rule implementation as defined here https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life
                    newMap[x, y] = aliveNeighbors == 3 || _Map[x, y] && aliveNeighbors == 2;
                }
            }
            _Map = newMap;
        }

        private IEnumerable<(int x, int y)> GetNeighbors(int x, int y)
        {
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    var r = (x: x + i, y: y + j);

                    if (r.x < 0 || r.x >= _GridSize.X ||
                        r.y < 0 || r.y >= _GridSize.Y ||
                       (r.x == x && r.y == y)) continue;

                    yield return r;
                }
            }
        }
    }
}