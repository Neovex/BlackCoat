using BlackCoat.Entities;
using BlackCoat.ParticleSystem;
using SFML.Graphics;
using SFML.System;
using System;
using System.Linq;
using System.Collections.Generic;
using SFML.Window;

namespace BlackCoat.Tools
{
    internal class PerformanceMonitor : TextItem
    {
        private Single _LastUpdate = 0;
        private Single _Runtime = 0;
#if AVERAGE_FPS
        private Queue<Single> _FPS = new Queue<float>();
#endif

        private const String TimeString = "FPS:   {0}\n" +
                                          "FTime: {1}\n" +
                                          "Total: {2}\n" +
                                          "APC:   {3}";


        public PerformanceMonitor(Core core)
            : base(core)
        {
            View = _Core.DefaultView;
            Font = _Core.DefaultFont;
            CharacterSize = 13;
            Color = SFML.Graphics.Color.Yellow;
            
            _Core.Device.Resized += Device_Resized;
        }

        private void Device_Resized(object sender, SizeEventArgs e)
        {
            View.Size = new Vector2f(e.Width, e.Height);
            View.Center = new Vector2f(e.Width / 2f, e.Height / 2f);
        }


        public override void Update(Single deltaT)
        {
            _LastUpdate += deltaT;
            _Runtime += deltaT;

#if AVERAGE_FPS
            _FPS.Enqueue(1 / deltaT);
            if (_FPS.Count > 10000) _FPS.Dequeue();
#endif

            if (_LastUpdate < 0.25) return;
            _LastUpdate = 0;


            DisplayedString = String.Format(TimeString,
# if !AVERAGE_FPS
                                            1 / deltaT,
#endif
#if AVERAGE_FPS
                                            _FPS.Sum() / _FPS.Count,
#endif
                                            deltaT,
                                            _Runtime,
                                            Emitter.ACTIVE_PARTICLES);
        }
    }
}