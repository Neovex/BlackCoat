using System;
using SFML.Graphics;
using SFML.Window;
using SFML.Audio;
using BlackCoat.Properties;
using BlackCoat.Entities;

namespace BlackCoat
{
    public class BlackCoatIntro : BaseGamestate
    {
        private BaseGamestate _NextState;
        private Graphic _Bg;
        private Texture _BgTex;
        private SoundBuffer _SoundBuffer;
        private Sound _Sound;
        private Boolean _Done = false;

        public BlackCoatIntro(Core core, BaseGamestate nextState):base(core, "BlackCoatIntro")
        {
            _NextState = nextState;
            if (_NextState == null)
            {
                Log.Fatal("Engine Intro initialized without follow up state");
            }
        }

        /// <summary>
        /// Loads the required data for this state.
        /// </summary>
        /// <returns>True on success.</returns>
        protected override bool Load()
        {
            _BgTex = TextureLoader.Load("bg", Resources.Loader);
            _SoundBuffer = SfxLoader.Load("snd", Resources.BCPad);
            _Sound = new Sound(_SoundBuffer);

            if (_BgTex == null || _SoundBuffer == null) return false;

            _Bg = new Graphic(_Core)
            {
                Texture = _BgTex,
                Alpha = 0,
            };
            Layer_BG.AddChild(_Bg);
            _Bg.Scale /= 2; // TODO : double check scale
            _Bg.Position = _Core.DeviceSize.ToVector2f() / 2 - _Bg.Texture.Size.ToVector2f() * _Bg.Scale.X / 2;

            _Core.Input.KeyPressed += HandleKeyPressed;
            _Core.AnimationManager.Wait(1, Start);

            return true;
        }

        private void Start()
        {
            if (_Done) return;
            _Sound.Play();
            _Core.AnimationManager.Run(0, 1, 4.5f, v => _Bg.Alpha = v);
            _Core.AnimationManager.Wait(6, () => _Core.AnimationManager.Run(1, 0, 1f, v => _Bg.Alpha = v, () => _Done = true));
        }

        private void HandleKeyPressed(Keyboard.Key key)
        {
            _Done = true;
        }

        protected override void Update(float deltaT)
        {
            if (_Done)
            {
                _Sound.Stop();
                _Core.StateManager.ChangeState(_NextState);
            }
        }

        protected override void Destroy()
        {
            _Core.Input.KeyPressed -= HandleKeyPressed;
            Layer_BG.RemoveChild(_Bg);
            TextureLoader.Release("bg");
            SfxLoader.Release("snd");
        }
    }
}