using System;
using SFML.Audio;
using SFML.Graphics;
using BlackCoat.Properties;
using BlackCoat.Entities;

namespace BlackCoat
{
    /// <summary>
    /// Default intro animation of the Black Coat Engine.
    /// </summary>
    /// <example><code>core.SceneManager.ChangeScene(new BlackCoatIntro(core, new MyScene(core)));</code></example>
    /// <seealso cref="BlackCoat.Scene" />
    public class BlackCoatIntro : Scene
    {
        // Variables #######################################################################
        private Scene _NextScene;
        private Graphic _Bg;
        private Texture _BgTex;
        private SoundBuffer _SoundBuffer;
        private Sound _Sound;
        private Boolean _Done = false;


        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="BlackCoatIntro"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        /// <param name="nextScene">The next scene to load after completion of the intro animation.</param>
        /// <exception cref="ArgumentNullException">nextScene</exception>
        public BlackCoatIntro(Core core, Scene nextScene):base(core)
        {
            _NextScene = nextScene ?? throw new ArgumentNullException(nameof(nextScene));
        }


        // Methods #########################################################################
        /// <summary>
        /// Loads and prepares all required asset and entities for the scene.
        /// </summary>
        /// <returns>True on success.</returns>
        protected override bool Load()
        {
            _BgTex = TextureLoader.Load(nameof(Resources.Loader), Resources.Loader);
            _SoundBuffer = SfxLoader.Load(nameof(Resources.BCPad), Resources.BCPad);
            if (_BgTex == null || _SoundBuffer == null) return false;
            _Sound = new Sound(_SoundBuffer);

            _Bg = new Graphic(_Core)
            {
                Texture = _BgTex,
                Alpha = 0,
            };
            Layer_Background.Add(_Bg);
            _Bg.Scale /= 2;
            _Bg.Position = _Core.DeviceSize / 2 - _Bg.Texture.Size.ToVector2f() * _Bg.Scale.X / 2;

            Input.KeyPressed += k => _Done = true;
            Input.JoystickButtonPressed += (joyId, btn) => _Done = true;
            _Core.AnimationManager.Wait(1, Start);

            return true;
        }

        /// <summary>
        /// Helper to start the animation sequence
        /// </summary>
        private void Start()
        {
            if (_Done) return;
            _Sound.Play();
            _Core.AnimationManager.Run(0, 1, 4.5f, v => _Bg.Alpha = v);
            _Core.AnimationManager.Wait(4, () => _Core.AnimationManager.Run(1, 0, 1f, v => _Bg.Alpha = v, () => _Done = true));
        }

        /// <summary>
        /// Check each frame if we need to change to the next scene.
        /// </summary>
        /// <param name="deltaT">Current frame time.</param>
        protected override void Update(float deltaT)
        {
            if (_Done)
            {
                _Sound.Stop();
                _Core.SceneManager.ChangeScene(_NextScene);
            }
        }

        /// <summary>
        /// Cleanup everything that is not managed by either the scene graph or the asset loaders.
        /// </summary>
        protected override void Destroy()
        {
            _Sound.Dispose();
        }
    }
}