namespace WraithavenGames.Bones3
{
    /// <summary>
    /// Provides a method of storing chunk properties for remeshing.
    /// </summary>
    public class ChunkProperties
    {
        private readonly BlockType[] m_Blocks = new BlockType[0];

        /// <summary>
        /// Gets the size of the chunk being handled.
        /// </summary>
        /// <value>The chunk size.</value>
        public GridSize ChunkSize { get; }

        /// <summary>
        /// Gets the position of the chunk.
        /// </summary>
        /// <value>The chunk position.</value>
        public ChunkPosition ChunkPosition { get; internal set; }

        internal ChunkProperties(GridSize chunkSize)
        {
            ChunkSize = chunkSize;

            int blockCount = chunkSize.Value + 2;
            blockCount *= blockCount * blockCount;
            m_Blocks = new BlockType[blockCount];
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
