using System;

using SFML.System;
using SFML.Window;
using SFML.Graphics;

using BlackCoat;
using BlackCoat.Entities;
using BlackCoat.Entities.Shapes;

namespace Pong
{
    class PongScene : Scene
    {
        private Rectangle _Player1;
        private Rectangle _Player2;
        private Rectangle _TopWall;
        private Rectangle _BottomWall;
        private Circle _Ball;

        private int _Score1;
        private TextItem _ScoreText1;
        private int _Score2;
        private TextItem _ScoreText2;


        public Vector2f BallVelocity { get; set; }
        public Vector2f Player1Velocity { get; set; }
        public Vector2f Player2Velocity { get; set; }


        public PongScene(Core core) : base(core, "PONG") { }


        protected override bool Load()
        {
            // Input
            Input.KeyPressed += HandleKeyPressed;
            Input.KeyReleased += HandleKeyReleased;

            // Players
            _Player1 = new Rectangle(_Core, new Vector2f(10, 50), Color.Cyan)
            {
                Position = new Vector2f(10, _Core.DeviceSize.Y / 2)
            };
            Layer_Game.Add(_Player1);

            _Player2 = new Rectangle(_Core, _Player1.Size, Color.Cyan)
            {
                Position = new Vector2f(_Core.DeviceSize.X - 20, _Core.DeviceSize.Y / 2)
            };
            Layer_Game.Add(_Player2);

            // Field
            var net = new Rectangle(_Core, new Vector2f(1, _Core.DeviceSize.Y -10), Color.Green)
            {
                Position = new Vector2f(_Core.DeviceSize.X / 2, 5)
            };
            Layer_Background.Add(net);

            _TopWall = new Rectangle(_Core, new Vector2f(_Core.DeviceSize.X, 10), Color.White)
            {
                Position = new Vector2f(0, 5)
            };
            Layer_Background.Add(_TopWall);

            _BottomWall = new Rectangle(_Core, new Vector2f(_Core.DeviceSize.X, 10), Color.White)
            {
                Position = new Vector2f(0, _Core.DeviceSize.Y - 15)
            };
            Layer_Background.Add(_BottomWall);

            // Score
            _ScoreText1 = new TextItem(_Core, _Score1.ToString(), 60)
            {
                Position = new Vector2f(50, 50),
                Alpha = 0.5f
            };
            Layer_Overlay.Add(_ScoreText1);

            _ScoreText2 = new TextItem(_Core, _Score2.ToString(), _ScoreText1.CharacterSize)
            {
                Position = new Vector2f(_Core.DeviceSize.X - 100, _ScoreText1.Position.Y),
                Alpha = _ScoreText1.Alpha
            };
            Layer_Overlay.Add(_ScoreText2);

            // Ball
            _Ball = new Circle(_Core, 5, Color.Blue)
            {
                Position = _Player1.Position + new Vector2f(_Player1.Size.X * 2, _Player1.Size.Y / 2),
                Origin = new Vector2f(5, 5)
            };
            Layer_Game.Add(_Ball);

            // Start Game
            BallVelocity = _Core.Random.NextVector(80, 150, -50, 50);
            return true;
        }

        protected override void Update(float deltaT)
        {
            _Ball.Position += BallVelocity * deltaT;
            _Player1.Position += Player1Velocity * deltaT;
            _Player2.Position += Player2Velocity * deltaT;

            if (_Ball.CollidesWith(_Player1))
            {
                HandlePlayerCollision(deltaT, _Player1, t => 300 + 120 * t);
            }
            else if (_Ball.CollidesWith(_Player2))
            {
                HandlePlayerCollision(deltaT, _Player2, t => 240 - 120 * t);
            }
            else if (_Ball.CollidesWith(_TopWall) || _Ball.CollidesWith(_BottomWall))
            {
                _Ball.Position -= BallVelocity * deltaT;
                BallVelocity = new Vector2f(BallVelocity.X, BallVelocity.Y * -1);
            }
            else if (_Ball.Position.X < 0)
            {
                HandleScore(ref _Score2, _ScoreText2, _Player1, 1);
            }
            else if (_Ball.Position.X > _Core.DeviceSize.X)
            {
                HandleScore(ref _Score1, _ScoreText1, _Player2, -1);
            }
        }

        private void HandlePlayerCollision(float deltaT, Rectangle player, Func<float, float> angleCalc)
        {
            _Ball.Position -= BallVelocity * deltaT;
            float tmp = MathHelper.Clamp(_Ball.Position.Y - player.Position.Y, 0, player.Size.Y) / player.Size.Y;
            BallVelocity = Create.Vector2fFromAngle(angleCalc.Invoke(tmp), (float)(BallVelocity.Length() * 1.4));
        }

        private void HandleScore(ref int score, TextItem scoreBoard, Rectangle player, int direction)
        {
            score++;
            scoreBoard.Text = score.ToString();

            _Ball.Position = player.Position + new Vector2f(player.Size.X * 2 * direction, player.Size.Y / 2);

            var min = direction == 1 ? 80 : -150;
            var max = direction == 1 ? 150 : -80;
            BallVelocity = _Core.Random.NextVector(min, max, -50, 50);
        }

        private void HandleKeyPressed(Keyboard.Key key)
        {
            var playerSpeed = 150;
            switch (key)
            {
                case Keyboard.Key.W:
                    Player1Velocity = new Vector2f(0, -playerSpeed);
                    break;
                case Keyboard.Key.S:
                    Player1Velocity = new Vector2f(0, playerSpeed);
                    break;
                case Keyboard.Key.Up:
                    Player2Velocity = new Vector2f(0, -playerSpeed);
                    break;
                case Keyboard.Key.Down:
                    Player2Velocity = new Vector2f(0, playerSpeed);
                    break;
            }
        }

        private void HandleKeyReleased(Keyboard.Key key)
        {
            if (key == Keyboard.Key.W || key == Keyboard.Key.S)
                Player1Velocity = default;
            else if (key == Keyboard.Key.Up || key == Keyboard.Key.Down)
                Player2Velocity = default;
        }

        protected override void Destroy()
        {
            Input.KeyPressed -= HandleKeyPressed;
            Input.KeyReleased -= HandleKeyReleased;
        }
    }
}