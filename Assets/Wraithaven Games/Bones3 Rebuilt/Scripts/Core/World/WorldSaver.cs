using System.Collections.Generic;

using UnityEngine;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// The WorldSaver class can be used to save a world from file.
    /// </summary>
    internal class WorldSaver : IChunkLoadHandler
    {
        private readonly World m_World;

        /// <summary>
        /// Creates a new world saver object.
        /// </summary>
        /// <param name="world">The world to manage.</param>
        internal WorldSaver(World world)
        {
            m_World = world;
        }

        /// <inheritdoc cref="IChunkLoadHandler"/>
        public bool OnChunkLoad(Chunk chunk)
        {
#if UNITY_EDITOR
            // In the editor, all world data is in this one folder.
            string folder = $"{Application.dataPath}/../Bones3/Worlds/{m_World.ID}/Chunks";
            return TryLoadChunk(chunk, folder);

#else
            // First try to load the chunk from persistant data path.
            string folder = $"{Application.persistentDataPath}/Worlds/{m_World.ID}/Chunks";
            if (TryLoadChunk(chunk, folder))
                return true;

            // If it doesn't exist, try loading from the streaming assets path.
            folder = $"{Application.streamingAssetsPath}/Worlds/{m_World.ID}/Chunks";
            return TryLoadChunk(chunk, folder);
#endif
        }

        /// <summary>
        /// Attempts to load chunk data from the given folder.
        /// </summary>
        /// <param name="chunk">The chunk to load.</param>
        /// <param name="folder">The chunk data folder.</param>
        /// <returns>True if data was loaded. False otherwise.</returns>
        private bool TryLoadChunk(Chunk chunk, string folder)
        {
            var task = new ChunkLoadOperation(folder, chunk);
            try
            {
                task.FinishTask();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Saves all chunks in the world.
        /// </summary>
        internal void SaveWorld()
        {
            List<ChunkSaveOperation> operations = new List<ChunkSaveOperation>();
#if UNITY_EDITOR
            string folder = $"{Application.dataPath}/../Bones3/Worlds/{m_World.ID}/Chunks";
#else
            string folder = $"{Application.persistentDataPath}/Worlds/{m_World.ID}/Chunks";
#endif

            foreach (var chunk in m_World.ChunkIterator())
            {
                if (chunk.IsModified)
                    operations.Add(new ChunkSaveOperation(folder, chunk));
            }

            List<System.Exception> exceptions = new List<System.Exception>();
            foreach (var op in operations)
            {
                try
                {
                    op.FinishTask();
                }
                catch (System.Exception e)
                {
                    exceptions.Add(e);
                }
            }

            if (exceptions.Count > 0) // We still want to throw all exceptions, but ensure tasks finish first.
                throw new System.AggregateException(exceptions);
        }
    }
}
