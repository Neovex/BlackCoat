using SFML.System;
using SFML.Graphics;
using SFML.Window;
using BlackCoat;
using BlackCoat.ParticleSystem;

namespace Particles
{
    class MyScene : Scene
    {
        private ParticleEmitterHost _Host;
        
        private TexturedSpawnInfo _TextureInfo;
        private TextureEmitter _TexEmitter;

        private ParticleSpawnInfo _PixelInfo;
        private PixelEmitter _PixelEmitter;


        public MyScene(Core core) : base(core)
        {
        }

        protected override bool Load()
        {
            _Core.Debug = true;

            // First we need a Host to give particles a home
            _Host = new ParticleEmitterHost(_Core);
            Layer_Game.Add(_Host);

            // Now we can create our emitters
            // Texture "Smoke" Emitter
            // 1 Create Spawn info object that is used to initialize each particle
            var tex = TextureLoader.Load("smoke");
            _TextureInfo = new TexturedSpawnInfo(tex)
            {
                TTL = 10,
                UseAlphaAsTTL = true,
                Alpha = .8f,
                AlphaFade = -0.2f,
                Velocity = new Vector2f(0, -60),
                Acceleration = new Vector2f(0, 10),
                Origin = tex.Size.ToVector2f() / 2,
                Scale = new Vector2f(0, 0),
                ScaleVelocity = new Vector2f(0.2f, 0.2f),
                RotationVelocity = 40,
                Loop = true
            };
            // 2 Create the Emitter itself
            _TexEmitter = new TextureEmitter(_Core, _TextureInfo, BlendMode.Add);
            // 3 Don't forget to add it to the host
            _Host.AddEmitter(_TexEmitter);

            // Pixel "Fireworks" Emitter
            // 1 Create Spawn info object that is used to initialize each particle
            // This would usually be a ParticleSpawnInfo object but since we would like
            // some randomness we created a custom class that inherits from it and add the random.
            _PixelInfo = new FireworkSpawnInfo(_Core)
            {
                TTL = 5,
                UseAlphaAsTTL = true,
                AlphaFade = -0.4f,
                ParticlesPerSpawn = 100,
                Acceleration = new Vector2f(0, 50)
            };
            // 2 Create the Emitter itself
            _PixelEmitter = new PixelEmitter(_Core, _PixelInfo);
            // 3 Don't forget to add it to the host
            _Host.AddEmitter(_PixelEmitter);

            // Setup input
            Input.MouseButtonPressed  += Input_MouseButtonPressed;
            Input.MouseButtonReleased += Input_MouseButtonReleased;

            // Start color animation
            ChangeColor();
            
            //OpenInspector();
            return true;
        }

        private void Input_MouseButtonPressed(Mouse.Button button)
        {
            switch (button)
            {
                case Mouse.Button.Left:
                    _TexEmitter.Triggered = true;
                break;
                case Mouse.Button.Right:
                    _PixelEmitter.Triggered = true;
                break;
            }
        }

        private void Input_MouseButtonReleased(Mouse.Button button)
        {
            if (button == Mouse.Button.Left) _TexEmitter.Triggered = false;
        }

        private void ChangeColor()
        {
            if (Destroyed) return;
            _TextureInfo.Color = CreateRandomColor();
            _PixelInfo.Color = CreateRandomColor();
            _Core.AnimationManager.Wait(0.5f, ChangeColor);
        }

        private Color CreateRandomColor() => new Color(CreateRandomByte(), CreateRandomByte(), CreateRandomByte());
        private byte  CreateRandomByte()  => (byte)_Core.Random.Next(50, byte.MaxValue);

        protected override void Update(float deltaT)
        {
            _TexEmitter.Position = Input.MousePosition;
            _PixelEmitter.Position = Input.MousePosition;
        }

        protected override void Destroy()
        {
            Input.MouseButtonPressed  -= Input_MouseButtonPressed;
            Input.MouseButtonReleased -= Input_MouseButtonReleased;
        }
    }
}