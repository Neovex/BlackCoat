using System;
using System.Collections.Generic;
using System.Linq;

using SFML.System;
using SFML.Graphics;

using BlackCoat;
using BlackCoat.Entities;
using BlackCoat.Entities.Shapes;
using BlackCoat.Animation;


namespace Animation
{
    class AnimationScene : Scene
    {
        public AnimationScene(Core core) : base(core)
        {
        }

        protected override bool Load()
        {
            var pos = new Vector2f(20, 20);
            var size = new Vector2f(100, 100);

            // Create a preview for each interpolation type
            foreach (var type in Enum.GetValues(typeof(InterpolationType)).Cast<InterpolationType>())
            {
                var startpos = pos + new Vector2f(0, 100);

                // Title and frame
                Layer_Game.Add(new TextItem(_Core, type.ToString(), 8u) { Position = pos + new Vector2f(2, -12) });
                Layer_Game.Add(new Rectangle(_Core, size, outlineColor: Color.Magenta) { Position = pos });

                // Preview line
                Layer_Game.Add(new FreeShape(_Core, PrimitiveType.LineStrip, GetPositions(startpos, type), Color.Cyan));

                // Moving Dot
                var dot = new Rectangle(_Core, new Vector2f(3, 3), Color.White) { Position = startpos, Origin = new Vector2f(1, 1) };
                Layer_Game.Add(dot);

                // Start Animation
                Animate(startpos, dot, type);

                // Move to next preview
                pos += new Vector2f(110, 0);
                if (pos.X + size.X > _Core.DeviceSize.X)
                {
                    pos = new Vector2f(20, pos.Y + 140);
                }
            }
            return true;
        }

        private IEnumerable<Vector2f> GetPositions(Vector2f startpos, InterpolationType type)
        {
            for (int i = 0; i < 100; i++)
            {
                var v = Interpolation.Calculate(type, startpos.Y, -100, i, 100);
                yield return new Vector2f(startpos.X + i, v);
            }
        }

        private void Animate(Vector2f startpos, IEntity entity, InterpolationType type)
        {
            // Animate X linear
            _Core.AnimationManager.Run(startpos.X, startpos.X + 100, 3, 
                v => entity.Position = new Vector2f(v, entity.Position.Y));

            // Animate Y with respective interpolation and restart animation when done
            _Core.AnimationManager.Run(startpos.Y, startpos.Y - 100, 3, 
                v => entity.Position = new Vector2f(entity.Position.X, v),
                () => Animate(startpos, entity, type), type);
        }

        protected override void Update(float deltaT)
        {
            // Animations update them self keeping the update free for your own code
        }

        protected override void Destroy()
        {
            // Nothing to destroy
        }
    }
}
