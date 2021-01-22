using System;
using SFML.System;
using SFML.Audio;
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
        private Graphic _Logo;
        private Sound _Sound;


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
            var sndBuff = SfxLoader.Load(nameof(Resources.BCPad), Resources.BCPad);
            var tex = TextureLoader.Load(nameof(Resources.Loader), Resources.Loader);
            if (sndBuff == null || tex == null) return false;

            _Sound = new Sound(sndBuff);
            
            _Logo = new Graphic(_Core, tex)
            {
                Alpha = 0,
                Scale = new Vector2f(0.5f, 0.5f),
                Position = _Core.DeviceSize / 2 - tex.Size.ToVector2f() / 4
            };
            Layer_Background.Add(_Logo);

            Input.KeyPressed += k => Done();
            Input.JoystickButtonPressed += (joyId, btn) => Done();

            _Core.AnimationManager.Wait(1, Start);
            return true;
        }

        /// <summary>
        /// Helper to start the animation sequence
        /// </summary>
        private void Start()
        {
            if (!Destroyed)
            {
                _Sound.Play();
                _Core.AnimationManager.Run(0, 1, 4.5f, Blend);
                _Core.AnimationManager.Wait(4,
                    () => _Core.AnimationManager.Run(1, 0, 1f, Blend, Done));
            }
        }

        /// <summary>
        /// Helper to fade the logo in and out
        /// </summary>
        private void Blend(float alpha)
        {
            if (!Destroyed) _Logo.Alpha = alpha;
        }

        /// <summary>
        /// Helper to change to the next scene
        /// </summary>
        private void Done()
        {
            if (!Destroyed)
            {
                _Sound.Stop();
                _Core.SceneManager.ChangeScene(_NextScene);
            }
        }

        /// <summary>
        /// Called each Frame to update the scenes user code.
        /// </summary>
        /// <param name="deltaT">Current frame time.</param>
        protected override void Update(float deltaT)
        { }

        /// <summary>
        /// Cleanup everything that is not managed by either the scene graph or the asset loaders.
        /// </summary>
        protected override void Destroy()
        {
            _Sound.Dispose();
        }
    }
}