using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlackCoat.Properties;
using BlackCoat.Tools;
using SFML.Graphics;
using SFML.Audio;
using BlackCoat.Entities;
using BlackCoat.Animation;
using SFML.Window;
using SFML.System;

namespace BlackCoat
{
    public class BlackCoatIntro : BaseGamestate
    {
        private BaseGamestate _NextState;
        private Graphic _Bg;
        private Texture _BgTex;
        private SoundBuffer _Sound;
        private Boolean _Done = false;

        public BlackCoatIntro(Core core, BaseGamestate nextState):base(core, "BlackCoatIntro")
        {
            _NextState = nextState;
            if (_NextState == null)
            {
                Log.Fatal("Engine Intro initialized without followup state");
            }
        }

        /// <summary>
        /// Loads the recuired data for this state.
        /// </summary>
        /// <returns>True on success.</returns>
        protected override bool Load()
        {
            _BgTex = TextureLoader.Load("bg", Resources.Loader);
            _Sound = SfxLoader.Load("snd", Resources.BCPad);

            _Bg = new Graphic(_Core);
            _Bg.Texture = _BgTex;
            _Bg.Scale = new Vector2f(0.5f, 0.5f);
            Layer_BG.AddChild(_Bg);

            Input.KeyPressed += HandleKeyPressed;
            _Core.AnimationManager.Run(0, 1, 6, v => _Bg.Alpha = v, InterpolationType.Linear, a => _Done = true);
            // TODO : fix timing & scale
            var snd = new Sound(_Sound);
            snd.Play();

            return _BgTex != null && _Sound != null;
        }

        private void HandleKeyPressed(Keyboard.Key key)
        {
            _Done = true;
        }

        protected override void Update(float deltaT)
        {
            if (_Done) _Core.StateManager.ChangeState(_NextState);
        }

        protected override void Destroy()
        {
            Input.KeyPressed -= HandleKeyPressed;
            Layer_BG.RemoveChild(_Bg);
            TextureLoader.Release("bg");
            SfxLoader.Release("snd");
        }
    }
}
