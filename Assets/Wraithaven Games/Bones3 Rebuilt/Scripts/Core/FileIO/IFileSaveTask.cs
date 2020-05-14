using System;

namespace Bones3Rebuilt
{
    /// <summary>
    /// A multithreaded task which saves an object to the disk.
    /// </summary>
    /// <typeparam name="T">The type of object being saved.</typeparam>
    public interface IFileSaveTask<T>
    {
        /// <summary>
        /// Waits for this task to finish before continuing.
        /// </summary>
        /// <returns>Details about the file saving operation.</returns>
        FileSaveStatus FinishTask();
    }

    /// <summary>
    /// The return state for a file saving task.
    /// </summary>
    public struct FileSaveStatus
    {
        /// <summary>
        /// Gets whether or not the data was correctly saved.
        /// </summary>
        /// <value>
        /// True if the data was saved normally. False if an error occurred.
        /// </value>
        public bool SuccessfullySaved { get; }

        /// <summary>
        /// Gets the error which was thrown while saving this file, if any.
        /// </summary>
        /// <value>The exception, or null if no error was thrown.</value>
        public Exception Error { get; }

        /// <summary>
        /// Creates a new file save status object.
        /// </summary>
        /// <param name="success">Whether or not the data was successfully loaded.</param>
        /// <param name="error">The error thrown while loading this object, if any.</param>
        public FileSaveStatus(bool success, Exception error)
        {
            if (error is AggregateException)
                error = (error as AggregateException).InnerException;

            SuccessfullySaved = success;
            Error = error;
        }
    }
}