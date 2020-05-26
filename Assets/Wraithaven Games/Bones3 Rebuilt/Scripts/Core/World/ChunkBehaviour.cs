using UnityEngine;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// Represents a temporary chunk pointer within a block world.
    /// </summary>
    internal class BlockChunk : MonoBehaviour
    {
        /// <summary>
        /// Get the position of this chunk within the world.
        /// </summary>
        /// <value>The position of the chunk in chunk coordinates.</value>
        internal ChunkPosition Position { get; set; }
    }
}
