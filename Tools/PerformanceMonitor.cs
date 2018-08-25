using System;
using SFML.System;
using SFML.Graphics;
using BlackCoat.Entities;
using BlackCoat.Animation;
using BlackCoat.ParticleSystem;
using BlackCoat.Entities.Shapes;

namespace BlackCoat.Tools
{
    /// <summary>
    /// Internal Helperclass that renders performance and engine information onto the scene
    /// </summary>
    internal class PerformanceMonitor : Container
    {
        // Variables #######################################################################
        private static Single _Runtime = 0;
        private TextItem _InfoDisplay;
        private Single _LastUpdate = 0;

#if AVERAGE_FPS
        private Queue<Single> _FPS = new Queue<float>();
#endif

        private const String TimeString = "FPS: {0}" + Constants.NEW_LINE +
                                          "FrT: {1}" + Constants.NEW_LINE +
                                          "Sum: {2}" + Constants.NEW_LINE +
                                          "DPF: {3}" + Constants.NEW_LINE +
                                          "APC: {4}" + Constants.NEW_LINE + 
                                          "AAC: {5}";


        // CTOR ############################################################################
        /// <summary>
        /// Creates a new instance of the performance monitor class
        /// </summary>
        /// <param name="core">Engine Core</param>
        internal PerformanceMonitor(Core core) : base(core)
        {
            AddChild(new Rectangle(_Core)
            {
                Size = new Vector2f(115, 80),
                Color = Color.Black,
                Alpha = 0.5f
            });

            AddChild(_InfoDisplay = new TextItem(_Core)
            {
                Position = new Vector2f(5, 2),
                Color = Color.Cyan,
                CharacterSize = 10
            });

            View = _Core.DefaultView;
        }


        // Methods #########################################################################
        /// <summary>
        /// Updates the FPS display
        /// </summary>
        /// <param name="gameTime">Current gametime</param>
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


            _InfoDisplay.Text = String.Format(TimeString,
# if !AVERAGE_FPS
                                              1 / deltaT,
#endif                                        
#if AVERAGE_FPS                               
                                              _FPS.Sum() / _FPS.Count,
#endif                                        
                                              deltaT,
                                              _Runtime,
                                              Core.DRAW_CALLS,
                                              //OldEmitter.ACTIVE_PARTICLES,
                                              ParticleBase._PARTICLES,
                                              AnimationManager.ACTIVE_ANIMATIONS);
        }
    }
}