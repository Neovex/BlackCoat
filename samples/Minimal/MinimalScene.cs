using SFML.System;
using SFML.Graphics;

using BlackCoat;
using BlackCoat.Entities.Shapes;

namespace Minimal
{
    class MinimalScene : Scene
    {
        private Rectangle _Rect;

        public MinimalScene(Core core) : base(core) { }

        protected override bool Load()
        {
            var size = new Vector2f(100, 100);

            // Create a Rectangle
            _Rect = new Rectangle(_Core, size, Color.Cyan)
            {
                Origin = size / 2,
                Position = _Core.DeviceSize / 2
            };

            // Add it to the scene
            Layer_Game.Add(_Rect);
            return true;
        }

        protected override void Update(float deltaT)
        {
            // Make it spin
            _Rect.Rotation += deltaT * 100;
        }

        protected override void Destroy() { }
    }
}