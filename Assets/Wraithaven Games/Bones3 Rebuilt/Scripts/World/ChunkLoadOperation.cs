using System.IO;
using System.Threading.Tasks;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// Used to maintain the saving and loading of block containers.
    /// </summary>
    internal static class ChunkLoadOperation
    {
        /// <summary>
        /// Attempts to load the chunk file.
        /// </summary>
        /// <param name="folder">The chunk data folder.</param>
        /// <param name="chunk">The chunk to load.</param>
        /// <exception cref="System.IOException">
        /// If an error occurs while loading the file.
        /// </exception>
        /// <exception cref="UnknownFileVersionException">
        /// If the received file version cannot be parsed.
        /// </exception>
        internal static void Run(string folder, Chunk chunk)
        {
            var file = $"{folder}/{chunk.Position.X}-{chunk.Position.Y}-{chunk.Position.Z}.dat";

            var fileStream = File.Open(file, FileMode.Open);
            using (var reader = new BinaryReader(fileStream))
            {
                int fileVersion = reader.ReadInt32();

                switch (fileVersion)
                {
                    case 1:
                        LoadChunkDataVersion1(reader, chunk);
                        break;

                    default:
                        throw new UnknownFileVersionException($"Unknown file version {fileVersion}!");
                }
            }

            chunk.IsModified = false;
        }

        /// <summary>
        /// Loads the world properties for this world using file version 1.
        /// </summary>
        /// <param name="reader">The reading to stream the data from.</param>
        private static void LoadChunkDataVersion1(BinaryReader reader, Chunk chunk)
        {
            var chunkSize = reader.ReadInt32();
            if (chunkSize != chunk.Size.IntBits)
                throw new InvalidDataException("Chunk size does not match expected!");

            var blocks = chunk.Blocks;
            for (int i = 0; i < blocks.Length; i++)
                blocks[i] = reader.ReadUInt16();
        }
    }
}
