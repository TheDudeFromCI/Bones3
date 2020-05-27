using UnityEngine;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// Handles loading the world.
    /// </summary>
    internal class WorldLoader : IChunkLoadHandler
    {
        private readonly World m_World;
        private readonly string m_ChunkFolder;

        /// <summary>
        /// Creates a new world loader.
        /// </summary>
        /// <param name="world">The world this loader is handling.</param>
        internal WorldLoader(World world)
        {
            m_World = world;

#if UNITY_EDITOR
            m_ChunkFolder = $"{Application.dataPath}/../Bones3/Worlds/{m_World.ID}/Chunks";
#else
            m_ChunkFolder = $"{Application.persistentDataPath}/Worlds/{m_World.ID}/Chunks";
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