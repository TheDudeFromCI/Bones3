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
            m_World.AddChunkLoadHandler(this);
        }

        /// <inheritdoc cref="IChunkLoadHandler"/>
        public void OnChunkLoad(Chunk chunk)
        {
#if UNITY_EDITOR
            // In the editor, all world data is in this one folder.
            string folder = $"{Application.dataPath}/Wraithaven Games/Bones3 Rebuilt/StreamingAssets/Worlds/{m_World.ID}/Chunks";
            TryLoadChunk(chunk, folder);

#else
            // First try to load the chunk from persistant data path.
            string folder = $"{Application.persistentDataPath}/Worlds/{m_World.ID}/Chunks";
            if(TryLoadChunk(chunk, folder))
                return;

            // If it doesn't exist, try loading from the streaming assets path.
            folder = $"{Application.streamingAssetsPath}/Worlds/{m_World.ID}/Chunks";
            TryLoadChunk(chunk, folder);
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
            // TODO Load the chunk
            return true;
        }

        /// <summary>
        /// Saves all chunks in the world.
        /// </summary>
        internal void SaveWorld()
        {
            foreach (var chunk in m_World.ChunkIterator())
                SaveChunk(chunk);
        }

        /// <summary>
        /// Saves a given chunk to file.
        /// </summary>
        /// <param name="chunk">The chunk to save.</param>
        private void SaveChunk(Chunk chunk)
        {
#if UNITY_EDITOR
            string folder = $"{Application.dataPath}/Wraithaven Games/Bones3 Rebuilt/StreamingAssets/Worlds/{m_World.ID}/Chunks";
#else
            string folder = $"{Application.persistentDataPath}/Worlds/{m_World.ID}/Chunks";
#endif

            // TODO Save the chunk
        }
    }
}