namespace Bones3Rebuilt
{
    /// <summary>
    /// Thrown when the received file version cannot be parsed.
    /// </summary>
    public class UnknownFileVersionException : System.Exception
    {
        /// <summary>
        /// Creates a new unknown file version exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        public UnknownFileVersionException(string message)
            : base(message)
        {

        }
    }
}