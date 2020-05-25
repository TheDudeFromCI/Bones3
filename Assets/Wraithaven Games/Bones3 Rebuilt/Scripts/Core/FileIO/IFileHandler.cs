namespace Bones3Rebuilt.Database
{
    /// <summary>
    /// This handler is used to read and write a specific object type to the world database folder.
    /// </summary>
    /// <typeparam name="T">The object type this file handler works with.</typeparam>
    public interface IFileHandler<T>
    {
        /// <summary>
        /// Saves the given object to the database.
        /// </summary>
        /// <param name="rootFolder">The root folder of the world database.</param>
        /// <param name="obj">The object to save.</param>
        /// <returns>The task associated with this operation.</returns>
        IFileSaveTask<T> Save(string rootFolder, T obj);

        /// <summary>
        /// Loads the given object from the database.
        /// </summary>
        /// <param name="rootFolder">The root folder of the world database.</param>
        /// <param name="obj">The object to load, if needed.</param>
        /// <returns>The task associated with this operation.</returns>
        IFileLoadTask<T> Load(string rootFolder, T obj = default);
    }
}
