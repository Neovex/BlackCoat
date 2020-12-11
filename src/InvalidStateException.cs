using System;

namespace BlackCoat
{
    public class InvalidStateException : Exception
    {
        public InvalidStateException() : base("Source instance was in an invalid state for the attempted operation")
        {
        }

        public InvalidStateException(string message) : base(message)
        {
        }
    }
}