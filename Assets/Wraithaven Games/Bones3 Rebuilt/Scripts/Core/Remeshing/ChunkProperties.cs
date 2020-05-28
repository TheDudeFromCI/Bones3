namespace WraithavenGames.Bones3
{
    /// <summary>
    /// Provides a method of storing chunk properties for remeshing.
    /// </summary>
    public class ChunkProperties
    {
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

        /// <summary>
        /// Gets the block type array backing this chunk properties object.
        /// </summary>
        internal BlockType[] Blocks { get; } = new BlockType[0];

        /// <summary>
        /// Creates a new chunk properties container for the given chunk size.
        /// </summary>
        /// <param name="chunkSize">The chunk size.</param>
        internal ChunkProperties(GridSize chunkSize)
        {
            ChunkSize = chunkSize;

            int blockCount = chunkSize.Value + 2;
            blockCount *= blockCount * blockCount;
            Blocks = new BlockType[blockCount];
        }

        /// <summary>
        /// Gets a block type at the position within this chunk. The block may be up to
        /// one block outside of the chunk bounds.
        /// </summary>
        /// <param name="pos">The position of the block.</param>
        /// <returns>The block type.</returns>
        public BlockType GetBlock(BlockPosition pos)
        {
            int size = ChunkSize.Value + 2;
            return Blocks[(pos.X + 1) * size * size + (pos.Y + 1) * size + (pos.Z + 1)];
        }
    }
}
