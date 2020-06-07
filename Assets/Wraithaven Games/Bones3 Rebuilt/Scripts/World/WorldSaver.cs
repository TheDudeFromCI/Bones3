using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// The WorldSaver class can be used to save a world from file.
    /// </summary>
    internal class WorldSaver
    {
        /// <summary>
        /// An identifier for handling file versioning, to aid in future-proofing.
        /// </summary>
        private const int CHUNK_FILE_VERSION = 1;

        private readonly string m_ChunkFolder;

        /// <summary>
        /// Creates a new world saver object.
        /// </summary>
        /// <param name="world">The world to manage.</param>
        internal WorldSaver(string worldID)
        {
#if UNITY_EDITOR
            m_ChunkFolder = $"{UnityEngine.Application.dataPath}/../Bones3/Worlds/{worldID}/Chunks";
#else
            m_ChunkFolder = $"{UnityEngine.Application.persistentDataPath}/Worlds/{worldID}/Chunks";
#endif
        }

        /// <summary>
        /// Saves all chunks in the world.
        /// </summary>
        internal void SaveWorld(WorldContainer container)
        {
            Directory.CreateDirectory(m_ChunkFolder);

            List<Task> tasks = new List<Task>();
            foreach (var chunk in container.World.ChunkIterator())
            {
                if (chunk.IsModified)
                {
                    var task = Task.Run(() => SaveChunk(chunk));
                    tasks.Add(task);
                }
            }

            for (int i = 0; i < tasks.Count; i++)
            {
                try
                {
                    tasks[i].Wait();
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }
        }

        /// <summary>
        /// Saves a chunk to it's correct file.
        /// </summary>
        /// <param name="chunk">The chunk to save.</param>
        private void SaveChunk(Chunk chunk)
        {
            var file = $"{m_ChunkFolder}/{chunk.Position.X}-{chunk.Position.Y}-{chunk.Position.Z}.dat";
            var fileStream = File.Open(file, FileMode.OpenOrCreate);
            using (var writer = new BinaryWriter(fileStream))
            {
                writer.Write(CHUNK_FILE_VERSION);
                writer.Write(chunk.Size.IntBits);

                var blocks = chunk.Blocks;
                for (int i = 0; i < blocks.Length; i++)
                    writer.Write(blocks[i]);
            }
        }

    }
}
