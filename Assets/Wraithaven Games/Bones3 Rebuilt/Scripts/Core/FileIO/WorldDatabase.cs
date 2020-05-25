using System.Collections.Generic;
using System.IO;
using System;

namespace Bones3Rebuilt.Database
{
    /// <summary>
    /// A file storage directory for handling saving and loading voxel worlds.
    /// </summary>
    public partial class WorldDatabase : IWorldDatabase
    {
        private readonly Dictionary<Type, object> m_FileHandlers = new Dictionary<Type, object>();

        /// <inheritdoc cref="IWorldDatabase"/>
        public string RootFolder { get; }

        /// <summary>
        /// Created a new world database for the given root folder.
        /// </summary>
        /// <param name="rootFolder">The root folder for the world.</param>
        /// <remarks>
        /// Creating this database will automatically create the world database
        /// directory if it does not currently exist.
        /// </remarks>
        public WorldDatabase(string rootFolder)
        {
            RootFolder = rootFolder;

            if (!File.Exists(RootFolder))
                Directory.CreateDirectory(RootFolder);
        }

        /// <summary>
        /// Registers the given file handler to the provided object type, replacing
        /// the previous handler if necessary.
        /// </summary>
        /// <param name="fileHandler">The file handler.</param>
        /// <typeparam name="T">The object type.</typeparam>
        public void RegisterFileHandler<T>(IFileHandler<T> fileHandler)
        {
            m_FileHandlers[typeof(T)] = fileHandler;
        }

        /// <summary>
        /// Creates a save task for the given object type.
        /// </summary>
        /// <param name="obj">The object to save.</param>
        /// <typeparam name="T">The object type.</typeparam>
        /// <returns>The save operation task.</returns>
        /// <exception cref="ArgumentException">
        /// If there is no registered file handler for the given object type.
        /// </exception>
        public IFileSaveTask<T> SaveObject<T>(T obj)
        {
            return GetHandler<T>().Save(RootFolder, obj);
        }

        /// <summary>
        /// Creates a load task for the given object type.
        /// </summary>
        /// <param name="obj">The object to load.</param>
        /// <typeparam name="T">The object type.</typeparam>
        /// <returns>The load operation task.</returns>
        /// <exception cref="ArgumentException">
        /// If there is no registered file handler for the given object type.
        /// </exception>
        public IFileLoadTask<T> LoadObject<T>(T obj = default)
        {
            return GetHandler<T>().Load(RootFolder, obj);
        }

        /// <summary>
        /// Gets the correct file handler for the given object type.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <returns>The file handler</returns>
        /// <exception cref="ArgumentException">
        /// If there is no registered file handler for the given object type.
        /// </exception>
        private IFileHandler<T> GetHandler<T>()
        {
            var type = typeof(T);
            if (!m_FileHandlers.ContainsKey(type))
                throw new ArgumentException("File handler not present!", "obj");

            return (IFileHandler<T>)m_FileHandlers[type];
        }
    }
}
