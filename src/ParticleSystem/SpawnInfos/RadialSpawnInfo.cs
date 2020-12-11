using SFML.System;


namespace BlackCoat.ParticleSystem
{
    class RadialSpawnInfo : ParticleSpawnInfo
    {
        private Core _Core;
        protected float _Angle;

        public float MinAngle { get; set; }
        public float MaxAngle { get; set; }
        public float Distance { get; set; }

        public override Vector2f Offset
        {
            get
            {
                _Angle = _Core.Random.NextFloat(MinAngle, MaxAngle);
                return Create.Vector2fFromAngleLookup(_Angle, Distance);
            }
            set => base.Offset = value;
        }

        public override Vector2f Velocity
        {
            get => Create.Vector2fFromAngleLookup(base.Velocity.Angle() + _Angle, (float)base.Velocity.Length());
            set => base.Velocity = value;
        }

        public override Vector2f Acceleration
        {
            get => Create.Vector2fFromAngleLookup(base.Acceleration.Angle() + _Angle, (float)base.Acceleration.Length());
            set => base.Acceleration = value;
        }

        public RadialSpawnInfo(Core core)
        {
            _Core = core;

            MinAngle = 0;
            MaxAngle = 360;
            Distance = 10;
        }
    }
}