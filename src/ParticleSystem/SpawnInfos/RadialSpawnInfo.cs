using SFML.System;


namespace BlackCoat.ParticleSystem
{
    /// <summary>
    /// Provides particle spawn information based on a circular spawn pattern
    /// </summary>
    /// <seealso cref="ParticleSpawnInfo" />
    public class RadialSpawnInfo : ParticleSpawnInfo
    {
        // Variables #######################################################################
        private Core _Core;
        protected float _Angle;


        // Properties ######################################################################
        /// <summary>
        /// Gets or sets the spawn areas minimum angle. Default 0.
        /// </summary>
        /// <remarks>
        /// <see cref="MinAngle"/>, <see cref="MaxAngle"/> and <see cref="Radius"/> create the circular spawn area.
        /// </remarks>
        public float MinAngle { get; set; }

        /// <summary>
        /// Gets or sets the spawn areas maximum angle. Default 360.
        /// </summary>
        /// <remarks>
        /// <see cref="MinAngle"/>, <see cref="MaxAngle"/> and <see cref="Radius"/> create the circular spawn area.
        /// </remarks>
        public float MaxAngle { get; set; }

        /// <summary>
        /// Gets or sets the spawn areas radius. Default 10.
        /// </summary>
        /// <remarks>
        /// <see cref="MinAngle"/>, <see cref="MaxAngle"/> and <see cref="Radius"/> create the circular spawn area.
        /// </remarks>
        public float Radius { get; set; }

        /// <summary>
        /// Gets the calculated offset between the particle and its emitter based on 
        /// <see cref="MinAngle"/>, <see cref="MaxAngle"/> and <see cref="Radius"/>.
        /// </summary>
        public override Vector2f Offset
        {
            get
            {
                _Angle = _Core.Random.NextFloat(MinAngle, MaxAngle);
                return Create.Vector2fFromAngleLookup(_Angle, Radius);
            }
            set => base.Offset = value;
        }

        /// <summary>
        /// Gets or sets the particle velocity.
        /// </summary>
        public override Vector2f Velocity
        {
            get => Create.Vector2fFromAngleLookup(base.Velocity.Angle() + _Angle, (float)base.Velocity.Length());
            set => base.Velocity = value;
        }

        /// <summary>
        /// Gets or sets the acceleration which will modify the velocity over time.
        /// </summary>
        public override Vector2f Acceleration
        {
            get => Create.Vector2fFromAngleLookup(base.Acceleration.Angle() + _Angle, (float)base.Acceleration.Length());
            set => base.Acceleration = value;
        }


        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="RadialSpawnInfo"/> class.
        /// </summary>
        /// <param name="core">The engine core.</param>
        public RadialSpawnInfo(Core core)
        {
            _Core = core;
            MinAngle = 0;
            MaxAngle = 360;
            Radius = 10;
        }
    }
}