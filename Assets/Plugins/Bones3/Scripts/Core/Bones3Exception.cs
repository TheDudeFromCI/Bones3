using System;

namespace Bones3
{
    /// <summary>
    /// Thrown when a error occurs while interacting with the Bones3 API.
    /// </summary>
    public class Bones3Exception : Exception
    {
        /// <summary>
        /// Creates a new exception.
        /// </summary>
        /// <param name="message">The message to attach to this exception.</param>
        public Bones3Exception(String message) : base(message) { }
    }
}