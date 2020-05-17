using System;

using Bones3Rebuilt;

using UnityEngine;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// Represents a temporary chunk pointer within a block world.
    /// </summary>
    public class BlockChunk : MonoBehaviour
    {
        /// <summary>
        /// Get the position of this chunk within the world.
        /// </summary>
        /// <value>The position of the chunk in chunk coordinates.</value>
        public ChunkPosition Position { get; internal set; }
    }
}
