﻿using System;
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

        private const String TimeString = "FPS:   {0}" + Constants.NEW_LINE +
                                          "FTime: {1}" + Constants.NEW_LINE +
                                          "Total: {2}" + Constants.NEW_LINE +
                                          "APC:   {3}" + Constants.NEW_LINE + 
                                          "AAC:   {4}";


        // CTOR ############################################################################
        /// <summary>
        /// Creates a new instance of the performance monitor class
        /// </summary>
        /// <param name="core">Engine Core</param>
        internal PerformanceMonitor(Core core) : base(core)
        {
            var backGround = new Rectangle(_Core);
            backGround.Size = new Vector2f(110, 75);
            backGround.Color = Color.Black;
            backGround.Alpha = 0.5f;
            AddChild(backGround);

            _InfoDisplay = new TextItem(_Core);
            _InfoDisplay.Position = new Vector2f(5, 2);
            _InfoDisplay.Color = Color.Yellow;
            _InfoDisplay.CharacterSize = 12;
            AddChild(_InfoDisplay);

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
                                            Emitter.ACTIVE_PARTICLES,
                                            AnimationManager.ACTIVE_ANIMATIONS);
        }
    }
}