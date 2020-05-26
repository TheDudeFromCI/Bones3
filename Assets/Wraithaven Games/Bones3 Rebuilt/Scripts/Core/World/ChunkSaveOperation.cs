using System.IO;
using System.Threading.Tasks;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// Used to maintain the saving and loading of block containers.
    /// </summary>
    internal class ChunkSaveOperation
    {
        /// <summary>
        /// An identifier for handling file versioning, to aid in future-proofing.
        /// </summary>
        private const int CHUNK_FILE_VERSION = 1;

        private readonly Task m_Task;
        private readonly string m_File;
        private readonly Chunk m_Chunk;

        /// <summary>
        /// Creates and starts a new chunk save operation for the given chunk.
        /// </summary>
        /// <param name="folder">The chunk data folder.</param>
        /// <param name="chunk">The chunk to save.</param>
        internal ChunkSaveOperation(string folder, Chunk chunk)
        {
            m_File = $"{folder}/{chunk.Position.X}-{chunk.Position.Y}-{chunk.Position.Z}.dat";
            m_Chunk = chunk;

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
            using(var writer = new BinaryWriter(fileStream))
            {
                writer.Write(CHUNK_FILE_VERSION);
                writer.Write(m_Chunk.Size.IntBits);

                var blocks = m_Chunk.Blocks;
                for (int i = 0; i < blocks.Length; i++)
                    writer.Write(blocks[i]);
            }
        }

        /// <summary>
        /// Waits for this save operation to complete before continuing.
        /// </summary>
        /// <exception cref="System.AggregateException">
        /// If an error occurs while saving this file.
        /// </exception>
        public void FinishTask()
        {
            m_Task.Wait();
            m_Chunk.IsModified = false;
        }
    }
}
