using System;

using SFML.Window;
using SFML.System;

using BlackCoat;
using BlackCoat.Entities;
using BlackCoat.Animation;

namespace Gemmy
{
    /// <summary>
    /// Gemmy - a Simple Tetris clone
    /// </summary>
    class GameScene : Scene
    {
        public const int BLOCKSIZE = 32;
        // Game field offset
        public const int FIELD_X = BLOCKSIZE * 5;
        public const int FIELD_Y = 2;

        private Grid _Grid;
        private BlockSet _BlockSet;
        private BlockType _NextBlockType;
        private Animation _MovePulse;
        private Animation _DifficultyMovement;

        private float _Difficulty;
        private int _Points;
        private int[] _CrystalPoints;

        private Graphic _DifficultyIndicator;
        private Graphic _BlockPreview;
        private TextItem _PointsDisplay;
        private TextItem[] _CrystalPointDisplays;
        private Graphic _StartScreen;
        private Graphic _GameOverScreen;


        public GameScene(Core core) : base(core, "Gemmy", "assets")
        { }


        protected override bool Load()
        {
            // Init
            BlockConfig.Initialize("assets/Blocks.cfg");
            Layer_Background.Add(new Graphic(_Core, TextureLoader.Load("Background")));
            Input.KeyPressed += HandleKeyDown;

            // GAME LOGIC
            _Grid = new Grid(15, 17);

            // Actors
            _DifficultyIndicator = new Graphic(_Core, TextureLoader.Load("BlockBox"));
            _DifficultyIndicator.Position = new Vector2f(BLOCKSIZE * 2, BLOCKSIZE * 16);
            Layer_Game.Add(_DifficultyIndicator);

            _BlockPreview = new Graphic(_Core);
            _BlockPreview.Position = new Vector2f(BLOCKSIZE * 21, BLOCKSIZE * 2);
            Layer_Game.Add(_BlockPreview);

            _CrystalPointDisplays = new TextItem[4];
            for (int i = 0; i < _CrystalPointDisplays.Length; i++)
            {
                var pointDeco = new Graphic(_Core, TextureLoader.Load($"Crystal{i + 1}"));
                pointDeco.Position = new Vector2f(BLOCKSIZE * 21, BLOCKSIZE * (8 + i));
                Layer_Game.Add(pointDeco);

                _CrystalPointDisplays[i] = new TextItem(_Core, characterSize: 10);
                _CrystalPointDisplays[i].Position = pointDeco.Position + new Vector2f(45, 10);
                Layer_Game.Add(_CrystalPointDisplays[i]);
            }
            var pointHeader = new TextItem(_Core, "Points", 14);
            pointHeader.Position = new Vector2f(BLOCKSIZE * 21 + 20, BLOCKSIZE * 6 + 4);
            Layer_Game.Add(pointHeader);

            _PointsDisplay = new TextItem(_Core, characterSize: 14);
            _PointsDisplay.Position = new Vector2f(BLOCKSIZE * 21 + 11, BLOCKSIZE * 7 + 4);
            Layer_Game.Add(_PointsDisplay);

            // Overlays
            _StartScreen = new Graphic(_Core, TextureLoader.Load("Start"));
            Layer_Overlay.Add(_StartScreen);

            _GameOverScreen = new Graphic(_Core, TextureLoader.Load("GameOver"));
            _GameOverScreen.Visible = false;
            Layer_Overlay.Add(_GameOverScreen);
            return true;
        }

        protected override void Update(float deltaT)
        {
            // no manual updates needed, everything is handled by animations
        }

        protected override void Destroy()
        {
            Input.KeyPressed -= HandleKeyDown;
        }

        private void StartGame()
        {
            // Remove/reset old stuff
            _StartScreen.Visible = false;
            _GameOverScreen.Visible = false;
            UpdateDifficulty(1);
            _Points = 0;
            _PointsDisplay.Text = _Points.ToString("D7");
            _CrystalPoints = new[] { 0, 0, 0, 0 };
            for (int i = 0; i < _CrystalPointDisplays.Length; i++)
            {
                _CrystalPointDisplays[i].Text = "x   0";
            }
            if (_BlockSet != null) _BlockSet.Destroy();
            _Grid.Clear();
            var duration = (float)new TimeSpan(0, 6, 30).TotalSeconds;
            _DifficultyMovement = _Core.AnimationManager.RunAdvanced(1, 0, duration, UpdateDifficulty);
            _NextBlockType = (BlockType)_Core.Random.Next(Enum.GetNames(typeof(BlockType)).Length);
            _BlockPreview.Texture = null;

            // Init Blocks
            SpawnNextBlocks();
            MoveCurrentBlocks();
        }

        private void UpdateDifficulty(float value)
        {
            // Update Difficulty
            _Difficulty = (value * (2 - 0.1f)) + 0.1f;
            // Move Difficulty Indicator
            var pos = _DifficultyIndicator.Position;
            pos.Y = (value * (BLOCKSIZE * 16 - BLOCKSIZE * 2)) + BLOCKSIZE * 2;
            _DifficultyIndicator.Position = pos;
        }

        private Boolean SpawnNextBlocks()
        {
            _BlockSet = new BlockSet(_Grid, _Core);
            if (!_BlockSet.Spawn(_NextBlockType, Layer_Game, TextureLoader))
            {
                _GameOverScreen.Visible = true;
                _DifficultyMovement.Stop();
                return false;
            }
            _NextBlockType = (BlockType)_Core.Random.Next(Enum.GetNames(typeof(BlockType)).Length);
            _BlockPreview.Texture = TextureLoader.Load(_NextBlockType.ToString());
            return true;
        }

        private void MoveCurrentBlocks()
        {
            if (!_BlockSet.Move(0, 1))
            {
                // freeze blocks into field
                _BlockSet.Freeze();
                // check for resolved (full) lines
                _Grid.ClearFullLinesAndMoveRemains(UpdatePoints);
                // and spawn next blockset
                if (!SpawnNextBlocks()) return;
            }

            // Start next Pulse
            _MovePulse = _Core.AnimationManager.Wait(_Difficulty, a => MoveCurrentBlocks());
        }

        private void UpdatePoints(int points)
        {
            _Points += points;
            _PointsDisplay.Text = _Points.ToString("D7");
            _CrystalPointDisplays[points - 1].Text = $"x  {++_CrystalPoints[points - 1]}";
        }

        private void HandleKeyDown(Keyboard.Key key)
        {
            // Start or restart game on any key
            if (_StartScreen.Visible || _GameOverScreen.Visible)
            {
                StartGame();
                return;
            }

            // Game Controls
            switch (key)
            {
                case Keyboard.Key.Left:
                case Keyboard.Key.A:
                    _BlockSet.Move(-1, 0);
                break;
                case Keyboard.Key.Right:
                case Keyboard.Key.D:
                    _BlockSet.Move(1, 0);
                break;
                case Keyboard.Key.Up:
                case Keyboard.Key.W:
                    _BlockSet.Rotate();
                break;
                case Keyboard.Key.Down:
                case Keyboard.Key.S:
                    if (!_BlockSet.Move(0, 1)) 
                        _MovePulse.Stop();
                break;
                case Keyboard.Key.Space:
                case Keyboard.Key.Enter:
                    while (_BlockSet.Move(0, 1));
                    _MovePulse.Stop();
                break;
            }
        }
    }
}