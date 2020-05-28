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
                ChunkLoadOperation.Run(m_ChunkFolder, chunk);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }
}