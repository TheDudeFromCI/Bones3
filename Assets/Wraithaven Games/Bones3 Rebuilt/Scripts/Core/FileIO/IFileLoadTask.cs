using System;

namespace Bones3Rebuilt.Database
{
    /// <summary>
    /// A multithreaded task which load an object from the disk.
    /// </summary>
    /// <typeparam name="T">The type of object being loaded.</typeparam>
    public interface IFileLoadTask<T>
    {
        /// <summary>
        /// Waits for this task to finish before continuing.
        /// </summary>
        /// <returns>Details about the file loading operation.</returns>
        FileLoadStatus FinishTask();
    }

    /// <summary>
    /// The return state for a file loading task.
    /// </summary>
    public struct FileLoadStatus
    {
        /// <summary>
        /// Gets the data returned from the file loading operation.
        /// </summary>
        /// <value>The loaded data.</value>
        public object Data { get; }

        /// <summary>
        /// Gets whether or not the data was correctly loaded.
        /// </summary>
        /// <value>
        /// True if the data was loaded normally. False if an error occurred.
        /// </value>
        public bool SuccessfullyLoaded { get; }

        /// <summary>
        /// Gets the error which was thrown while loading this file, if any.
        /// </summary>
        /// <value>The exception, or null if no error was thrown.</value>
        public Exception Error { get; }

        /// <summary>
        /// Creates a new file load status object.
        /// </summary>
        /// <param name="data">The data which was loaded, or null.</param>
        /// <param name="success">Whether or not the data was successfully loaded.</param>
        /// <param name="error">The error thrown while loading this object, if any.</param>
        public FileLoadStatus(object data, bool success, Exception error)
        {
            if (error is AggregateException)
                error = (error as AggregateException).InnerException;

            Data = data;
            SuccessfullyLoaded = success;
            Error = error;
        }
    }
}