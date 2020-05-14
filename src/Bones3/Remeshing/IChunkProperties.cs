using System;

namespace Bones3Rebuilt
{
    /// <summary>
    /// A thread-safe method of reading chunk data.
    /// </summary>
    public interface IChunkProperties
    {
        /// <summary>
        /// Gets the size of the chunk being handled.
        /// </summary>
        /// <value>The chunk size.</value>
        GridSize ChunkSize { get; }

        /// <summary>
        /// Gets the position of the chunk.
        /// </summary>
        /// <value>The chunk position.</value>
        ChunkPosition ChunkPosition { get; }

        /// <summary>
        /// Gets a block type at the position within this chunk.
        /// </summary>
        /// <param name="pos">The position of the block.</param>
        /// <returns>The block type.</returns>
        BlockType GetBlock(BlockPosition pos);

        /// <summary>
        /// Gets a block type at the position relative to the target block,
        /// within this chunk or in the chunks surrounding it.
        /// </summary>
        /// <param name="pos">The position of the block.</param>
        /// <param name="side">The side of the block being handled.</param>
        /// <returns>The block type of the neighboring block on the given side.</returns>
        BlockType GetNextBlock(BlockPosition pos, int side);
    }
}