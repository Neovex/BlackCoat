namespace BlackCoat.ParticleSystem
{
    public interface ITriggerEmitter
    {
        /// <summary>
        /// Triggers the emitter. Causing it to start emitting particles.
        /// </summary>
        void Trigger();
    }
}