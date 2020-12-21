namespace BlackCoat.ParticleSystem
{
    /// <summary>
    /// Common interface for all emitter that can be triggered.
    /// </summary>
    public interface ITriggerEmitter
    {
        // Methods #########################################################################
        /// <summary>
        /// Triggers the emitter. Causing it to start emitting particles.
        /// </summary>
        void Trigger();
    }
}