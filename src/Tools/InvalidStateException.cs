using System;

namespace BlackCoat
{
    /// <summary>
    /// Exception class for when an instance is used while in an inappropriate state for the attempted operation.
    /// </summary>
    /// <seealso cref="Exception" />
    public class InvalidStateException : Exception
    {
        // CTOR ############################################################################
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidStateException"/> class.
        /// </summary>
        public InvalidStateException() : base("The instance was in an invalid state for the attempted operation")
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidStateException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidStateException(string message) : base(message)
        { }
    }
}