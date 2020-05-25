using System.IO;
using System.Threading.Tasks;

namespace Bones3Rebuilt.Database.ChunkData
{
    /// <summary>
    /// Used to maintain the saving and loading of block containers.
    /// </summary>
    public class ChunkDataHandler : IFileHandler<ChunkData>
    {
        /// <inheritdoc cref="IFileHandler{T}"/>
        public IFileLoadTask<ChunkData> Load(string rootFolder, ChunkData obj = null)
        {
            if (obj == null)
                throw new System.ArgumentNullException("Obj cannot be null!");

            return new ChunkDataLoadTask(rootFolder, obj);
        }

        /// <inheritdoc cref="IFileHandler{T}"/>
        public IFileSaveTask<ChunkData> Save(string rootFolder, ChunkData obj)
        {
            if (obj == null)
                throw new System.ArgumentNullException("Obj cannot be null!");

            return new ChunkDataSaveTask(rootFolder, obj);
        }
    }

    /// <summary>
    /// A background task for saving block containers.
    /// </summary>
    public class ChunkDataSaveTask : IFileSaveTask<ChunkData>
    {
        /// <summary>
        /// An identifier for handling file versioning, to aid in future-proofing.
        /// </summary>
        private const int CHUNK_FILE_VERSION = 1;

        private readonly Task m_Task;
        private readonly ChunkData m_ChunkData;
        private readonly string m_File;

        public ChunkDataSaveTask(string rootFolder, ChunkData chunkData)
        {
            m_File = $"{rootFolder}/ChunkData/{chunkData.Position.X}-{chunkData.Position.Y}-{chunkData.Position.Z}.dat";
            m_ChunkData = chunkData;
            m_Task = Task.Run(Run);
        }

        /// <summary>
        /// Attempts to save the block container data to the region file.
        /// </summary>
        /// <exception cref="System.IOException">
        /// If an error occurs while saving the file.
        /// </exception>
        private void Run()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(m_File));

            var fileStream = File.Open(m_File, FileMode.OpenOrCreate);
            using (var writer = new BinaryWriter(fileStream))
            {
                writer.Write(CHUNK_FILE_VERSION);
                writer.Write(m_ChunkData.Size.IntBits);

                var blocks = m_ChunkData.Blocks;
                for (int i = 0; i < blocks.Length; i++)
                    writer.Write(blocks[i]);
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

    public class ChunkDataLoadTask : IFileLoadTask<ChunkData>
    {
        private readonly string m_File;
        private readonly Task m_Task;
        private readonly ChunkData m_ChunkData;

        public ChunkDataLoadTask(string rootFolder, ChunkData chunkData)
        {
            m_File = $"{rootFolder}/ChunkData/{chunkData.Position.X}-{chunkData.Position.Y}-{chunkData.Position.Z}.dat";
            m_ChunkData = chunkData;
            m_Task = Task.Run(Run);
        }

        /// <summary>
        /// Attempts to load the chunk file.
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
                        LoadChunkDataVersion1(reader);
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
        private void LoadChunkDataVersion1(BinaryReader reader)
        {
            var chunkSize = reader.ReadInt32();
            if (chunkSize != m_ChunkData.Size.IntBits)
                throw new InvalidDataException("Chunk size does not match expected!");

            var blocks = m_ChunkData.Blocks;
            for (int i = 0; i < blocks.Length; i++)
                blocks[i] = reader.ReadUInt16();
        }

        /// <inheritdoc cref="IFileLoadTask{T}"/>
        public FileLoadStatus FinishTask()
        {
            try
            {
                m_Task.Wait();
                return new FileLoadStatus(m_ChunkData, true, null);
            }
            catch (System.Exception e)
            {
                return new FileLoadStatus(null, false, e);
            }
        }
    }
}
