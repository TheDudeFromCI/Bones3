namespace Bones3Rebuilt
{
    /// <summary>
    /// A file storage directory for handling saving and loading voxel worlds.
    /// </summary>
    public interface IWorldDatabase
    {
        /// <summary>
        /// The root folder this world is saved within.
        /// </summary>
        string RootFolder { get; }

        /// <summary>
        /// Saves the given object to this database.
        /// </summary>
        /// <param name="obj">The object to save.</param>
        /// <typeparam name="T">The object type.</typeparam>
        /// <returns>The object saving task.</returns>
        /// <exception cref="System.ArgumentException">
        /// If there is no file handler for the given argument type.
        /// </exception>
        IFileSaveTask<T> SaveObject<T>(T obj);

        /// <summary>
        /// Loads the given object from the database.
        /// </summary>
        /// <param name="obj">The object to load, if needed.</param>
        /// <typeparam name="T">The object type.</typeparam>
        /// <returns>The object loading task.</returns>
        /// <exception cref="System.ArgumentException">
        /// If there is no file handler for the given argument type.
        /// </exception>
        IFileLoadTask<T> LoadObject<T>(T obj = default);
    }
}
