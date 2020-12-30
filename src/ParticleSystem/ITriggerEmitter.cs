namespace BlackCoat.ParticleSystem
{
    /// <summary>
    /// Common interface for all emitter that can be triggered.
    /// </summary>
    public interface ITriggerEmitter
    {
        // Methods #########################################################################
        /// <summary>
        /// Gets or sets a value indicating whether this instance is currently triggered.
        /// </summary>
        bool Triggered { get; set; }
    }
}