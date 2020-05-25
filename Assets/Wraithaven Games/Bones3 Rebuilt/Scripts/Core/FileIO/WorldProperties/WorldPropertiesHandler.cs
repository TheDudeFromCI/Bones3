using System.IO;
using System.Threading.Tasks;

namespace Bones3Rebuilt.Database.WorldProps
{
    /// <summary>
    /// Handles file IO for a world properties object.
    /// </summary>
    public class WorldPropertiesHandler : IFileHandler<WorldProperties>
    {
        /// <inheritdoc cref="IFileHandler{T}"/>
        public IFileSaveTask<WorldProperties> Save(string rootFolder, WorldProperties worldProperties) =>
            new WorldPropertiesSaveTask(rootFolder, worldProperties);

        /// <inheritdoc cref="IFileHandler{T}"/>
        public IFileLoadTask<WorldProperties> Load(string rootFolder, WorldProperties worldProperties = default) =>
            new WorldPropertiesLoadTask(rootFolder);
    }

    /// <summary>
    /// Saves a world properties object to the disk using the most recent file version format.
    /// </summary>
    internal class WorldPropertiesSaveTask : IFileSaveTask<WorldProperties>
    {
        /// <summary>
        /// An identifier for handling file versioning, to aid in future-proofing.
        /// </summary>
        private const int WORLD_PROPERTIES_FILE_VERSION = 1;

        private readonly string m_File;
        private readonly WorldProperties m_WorldProperties;
        private readonly Task m_Task;

        /// <summary>
        /// Creates a new world properties save task.
        /// </summary>
        /// <param name="folder">The world database root folder.</param>
        /// <param name="worldProperties">The properties object to save.</param>
        public WorldPropertiesSaveTask(string folder, WorldProperties worldProperties)
        {
            m_File = $"{folder}/properties.dat";
            m_WorldProperties = worldProperties;
            m_Task = Task.Run(Run);
        }

        /// <summary>
        /// Attempts to save the world properties file.
        /// </summary>
        /// <exception cref="System.IOException">
        /// If an error occurs while saving the file.
        /// </exception>
        private void Run()
        {
            var fileStream = File.Open(m_File, FileMode.OpenOrCreate);
            using (var writer = new BinaryWriter(fileStream))
            {
                writer.Write(WORLD_PROPERTIES_FILE_VERSION);

                writer.Write(m_WorldProperties.ChunkSize.IntBits);
                writer.Write(m_WorldProperties.WorldName);
                writer.Write(m_WorldProperties.WorldFileFormat);
            }
        }

        /// <inheritdoc cref="IFileSaveTask{T}"/>
        public FileSaveStatus FinishTask()
        {
            try
            {
                m_Task.Wait();
                return new FileSaveStatus(true, null);
            }
            catch (System.Exception e)
            {
                return new FileSaveStatus(false, e);
            }
        }
    }

    /// <summary>
    /// Loads a world properties object from the disk using all known file version formats.
    /// </summary>
    internal class WorldPropertiesLoadTask : IFileLoadTask<WorldProperties>
    {
        private readonly string m_File;
        private readonly Task m_Task;
        private WorldProperties m_WorldProperties;

        /// <summary>
        /// Creates a new world properties load task.
        /// </summary>
        /// <param name="folder">The world database root folder.</param>
        public WorldPropertiesLoadTask(string folder)
        {
            m_File = $"{folder}/properties.dat";
            m_Task = Task.Run(Run);
        }

        /// <summary>
        /// Attempts to load the world properties file.
        /// </summary>
        /// <exception cref="System.IOException">
        /// If an error occurs while loading the file.
        /// </exception>
        /// <exception cref="UnknownFileVersionException">
        /// If the received file version cannot be parsed.
        /// </exception>
        private void Run()
        {
            var fileStream = File.Open(m_File, FileMode.Open);
            using (var reader = new BinaryReader(fileStream))
            {
                int fileVersion = reader.ReadInt32();

                switch (fileVersion)
                {
                    case 1:
                        LoadWorldPropertiesVersion1(reader);
                        break;

                    default:
                        throw new UnknownFileVersionException($"Unknown file version {fileVersion}!");
                }
            }
        }

        /// <summary>
        /// Loads the world properties for this world using file version 1.
        /// </summary>
        /// <param name="reader">The reading to stream the data from.</param>
        private void LoadWorldPropertiesVersion1(BinaryReader reader)
        {
            var chunkSize = new GridSize(reader.ReadInt32());
            var worldName = reader.ReadString();
            var worldFileFormat = reader.ReadInt32();

            m_WorldProperties = new WorldProperties
            {
                ChunkSize = chunkSize,
                WorldName = worldName,
                WorldFileFormat = worldFileFormat,
            };
        }

        /// <inheritdoc cref="IFileLoadTask{T}"/>
        public FileLoadStatus FinishTask()
        {
            try
            {
                m_Task.Wait();
                return new FileLoadStatus(m_WorldProperties, true, null);
            }
            catch (System.Exception e)
            {
                return new FileLoadStatus(null, false, e);
            }
        }
    }
}
