using UnityEngine;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// A simple abstract behaviour for generating terrain on block worlds.
    /// </summary>
    [ExecuteAlways, DisallowMultipleComponent, RequireComponent(typeof(BlockWorld))]
    public abstract class WorldGenerator : MonoBehaviour, IChunkLoadHandler
    {
        private bool m_AlreadyEnabled;

        /// <summary>
        /// Called during the startup phase to atttach this chunk loader to the block world.
        /// </summary>
        protected void OnEnable()
        {
            if (m_AlreadyEnabled)
                return;

            m_AlreadyEnabled = true;
            var blockWorld = GetComponent<BlockWorld>();
            blockWorld.WorldContainer.ChunkLoader.AddChunkLoadHandler(this);
        }

        /// <inheritdoc cref="IChunkLoadHandler"/>
        bool IChunkLoadHandler.OnChunkLoad(Chunk chunk, bool alreadyModified)
        {
            if (alreadyModified)
                return false;

            // Fill chunk with air, first.
            for (int i = 0; i < chunk.Blocks.Length; i++)
                chunk.Blocks[i] = 1;
            chunk.IsModified = true;

            // The generate terrain on top.
            GenerateChunk(chunk);
            return true;
        }

        /// <summary>
        /// Generate the chunk terrain. This is not called on the main thread, so this method
        /// should not call any Unity APIs or any other code.
        /// </summary>
        /// <param name="chunk">The chunk.</param>
        protected abstract void GenerateChunk(Chunk chunk);
    }
}