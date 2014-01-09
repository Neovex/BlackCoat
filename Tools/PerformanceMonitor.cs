using System;
using BlackCoat.ParticleSystem;
using SFML.Graphics;

namespace BlackCoat.Tools
{
    internal class PerformanceMonitor : TextItem
    {
        private Single _LastUpdate = 0;
        private Single _Runtime = 0;

        private const String TimeString = "\r\nBlack Coat Engine\r\n\r\n" +
                                              "FPS:   {0}\r\n" +
                                              "FTime: {1}\r\n" +
                                              "Total: {2}\r\n" +
                                              "APC:   {3}";



        public PerformanceMonitor(Core core) : base(core)
        {
            Size = 13;
            Color = SFML.Graphics.Color.Yellow;
        }


        public override void Update(Single deltaT)
        {
            _LastUpdate += deltaT;
            _Runtime += deltaT;

            if (_LastUpdate > 0.17)
            {
                Text = String.Format(TimeString,
                                      1 / deltaT,
                                      deltaT,
                                      _Runtime,
                                      Emitter.ACTIVE_PARTICLES);
                _LastUpdate = 0;
            }
        }
    }
}