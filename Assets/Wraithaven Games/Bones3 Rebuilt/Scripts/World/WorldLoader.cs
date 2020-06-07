using System.IO;
using UnityEngine;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// Handles loading the world.
    /// </summary>
    internal class WorldLoader : IChunkLoadHandler
    {
        private readonly string m_ChunkFolder;

        /// <summary>
        /// Creates a new world loader.
        /// </summary>
        /// <param name="worldID">The ID of the world to load from.</param>
        internal WorldLoader(string worldID)
        {
#if UNITY_EDITOR
            m_ChunkFolder = $"{Application.dataPath}/../Bones3/Worlds/{worldID}/Chunks";
#else
            m_ChunkFolder = $"{Application.persistentDataPath}/Worlds/{worldID}/Chunks";
#endif
        }

        /// <inheritdoc cref="IChunkLoadHandler"/>
        public bool OnChunkLoad(Chunk chunk, bool alreadyModified)
        {
            try
            {
                Run(chunk);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Attempts to load the chunk file.
        /// </summary>
        /// <param name="chunk">The chunk to load.</param>
        /// <exception cref="System.IOException">
        /// If an error occurs while loading the file.
        /// </exception>
        /// <exception cref="UnknownFileVersionException">
        /// If the received file version cannot be parsed.
        /// </exception>
        internal void Run(Chunk chunk)
        {
            var file = $"{m_ChunkFolder}/{chunk.Position.X}-{chunk.Position.Y}-{chunk.Position.Z}.dat";

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
        private void LoadChunkDataVersion1(BinaryReader reader, Chunk chunk)
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