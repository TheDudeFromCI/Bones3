using System.IO;
using System.Threading.Tasks;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// Used to maintain the saving and loading of block containers.
    /// </summary>
    internal class ChunkLoadOperation
    {
        private readonly Task m_Task;

        /// <summary>
        /// Gets the file being targeted by this operation.
        /// </summary>
        /// <value>The file.</value>
        internal string OutputFile { get; }

        /// <summary>
        /// Gets the chunk being targeted by this operation.
        /// </summary>
        /// <value>The chunk.</value>
        internal Chunk Chunk { get; }

        /// <summary>
        /// Creates and starts a new chunk load operation for the given chunk.
        /// </summary>
        /// <param name="folder">The chunk data folder.</param>
        /// <param name="chunk">The chunk to load.</param>
        internal ChunkLoadOperation(string folder, Chunk chunk)
        {
            OutputFile = $"{folder}/{chunk.Position.X}-{chunk.Position.Y}-{chunk.Position.Z}.dat";
            Chunk = chunk;

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
            var fileStream = File.Open(OutputFile, FileMode.Open);
            using(var reader = new BinaryReader(fileStream))
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
            if (chunkSize != Chunk.Size.IntBits)
                throw new InvalidDataException("Chunk size does not match expected!");

            var blocks = Chunk.Blocks;
            for (int i = 0; i < blocks.Length; i++)
                blocks[i] = reader.ReadUInt16();
        }

        /// <summary>
        /// Waits for this load operation to complete before continuing.
        /// </summary>
        /// <exception cref="System.AggregateException">
        /// If an error occurs while loading this file.
        /// </exception>
        public void FinishTask()
        {
            m_Task.Wait();
            Chunk.IsModified = false;
        }
    }
}
