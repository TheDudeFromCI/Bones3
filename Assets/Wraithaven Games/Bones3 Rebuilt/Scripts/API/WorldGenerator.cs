using UnityEngine;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// A simple abstract behaviour for generating terrain on block worlds.
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class WorldGenerator : MonoBehaviour, IChunkLoadHandler
    {
        /// <inheritdoc cref="IChunkLoadHandler"/>
        bool IChunkLoadHandler.OnChunkLoad(Chunk chunk, bool alreadyModified)
        {
            if (alreadyModified)
                return false;

            // Fill chunk with air, first.
            for (int i = 0; i < chunk.Blocks.Length; i++)
                chunk.Blocks[i] = 1;
            chunk.IsModified = false;

            // The generate terrain on top.
            GenerateChunk(chunk);
            return chunk.IsModified;
        }

        /// <summary>
        /// Generate the chunk terrain. This is not called on the main thread, so this method
        /// should not call any Unity APIs or any other code.
        /// </summary>
        /// <param name="chunk">The chunk.</param>
        protected abstract void GenerateChunk(Chunk chunk);
    }
}