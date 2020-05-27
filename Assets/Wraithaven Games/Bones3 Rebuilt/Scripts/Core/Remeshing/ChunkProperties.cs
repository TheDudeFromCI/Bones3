namespace WraithavenGames.Bones3
{
    // TODO Add locking mechanisms to ensure thread safety to Reset() and SetBlock() methods.

    /// <summary>
    /// Provides a method of storing chunk properties for remeshing.
    /// </summary>
    public class ChunkProperties
    {
        private BlockType[] m_Blocks = new BlockType[0];

        /// <summary>
        /// Gets the size of the chunk being handled.
        /// </summary>
        /// <value>The chunk size.</value>
        public GridSize ChunkSize { get; private set; }

        /// <summary>
        /// Gets the position of the chunk.
        /// </summary>
        /// <value>The chunk position.</value>
        public ChunkPosition ChunkPosition { get; private set; }

        /// <summary>
        /// Prepares this chunk properties for a chunk with the given size and position. This
        /// should only be called by the main thread when tasks are not actively using it. This
        /// method will also clear all block data currently stored.
        /// </summary>
        /// <param name="chunkPos">The chunk position.</param>
        /// <param name="chunkSize">The chunk size.</param>
        /// <remarks>
        /// If the chunk size is less than the allocated memory size, then a new array is allocated.
        /// If analyzing many chunks of different sizes, it minimizes garbage creation by scanning
        /// the largest chunk first.
        /// </remarks>
        public void Reset(ChunkPosition chunkPos, GridSize chunkSize)
        {
            ChunkPosition = chunkPos;
            ChunkSize = chunkSize;

            int blockCount = chunkSize.Value + 2;
            blockCount *= blockCount * blockCount;

            if (m_Blocks.Length < blockCount)
                m_Blocks = new BlockType[blockCount];
            else
            {
                for (int i = 0; i < m_Blocks.Length; i++)
                    m_Blocks[i] = null;
            }
        }

        /// <summary>
        /// Sets a block type at the position within this chunk. The block may be up to
        /// one block outside of the chunk bounds. This should only be called from the
        /// main thread, and should not be called when this chunk properties object is
        /// being used by the remesh tasks.
        /// </summary>
        /// <param name="pos">The position of the block.</param>
        /// <returns>The block type.</returns>
        public void SetBlock(BlockPosition pos, BlockType details)
        {
            m_Blocks[BlockIndex(pos)] = details;
        }

        /// <summary>
        /// Gets a block type at the position within this chunk. The block may be up to
        /// one block outside of the chunk bounds.
        /// </summary>
        /// <param name="pos">The position of the block.</param>
        /// <returns>The block type.</returns>
        public BlockType GetBlock(BlockPosition pos)
        {
            return m_Blocks[BlockIndex(pos)];
        }

        /// <summary>
        /// Gets the block list index of the given block position.
        /// </summary>
        /// <param name="pos">The block position.</param>
        /// <returns>The block index.</returns>
        private int BlockIndex(BlockPosition pos)
        {
            int size = ChunkSize.Value + 2;
            return (pos.X + 1) * size * size + (pos.Y + 1) * size + (pos.Z + 1);
        }
    }
}
