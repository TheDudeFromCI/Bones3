namespace WraithavenGames.Bones3
{
    /// <summary>
    /// A collection of blocks for quick lookups while remeshing a chunk.
    /// </summary>
    public class ChunkGroup
    {
        private readonly Chunk[] m_Chunks = new Chunk[27];
        private readonly ServerBlockList m_BlockList;

        /// <summary>
        /// Gets the size of the chunk.
        /// </summary>
        public GridSize ChunkSize { get; }

        /// <summary>
        /// A direct reference to the block IDs within the middle chunk.
        /// </summary>
        internal ushort[] Blocks => m_Chunks[13].Blocks;

        /// <summary>
        /// Creates a new chunk group for remeshing.
        /// </summary>
        /// <param name="container">The world container being remeshed.</param>
        /// <param name="chunkPos">The position of the chunk to remesh.</param>
        internal ChunkGroup(WorldContainer container, ChunkPosition chunkPos)
        {
            for (int x = 0; x < 3; x++)
                for (int y = 0; y < 3; y++)
                    for (int z = 0; z < 3; z++)
                        m_Chunks[x * 3 * 3 + y * 3 + z] = container.World.GetChunk(new ChunkPosition(x - 1, y - 1, z - 1) + chunkPos);

            m_BlockList = container.BlockList;
            ChunkSize = container.World.ChunkSize;
        }

        /// <summary>
        /// Gets the block type at the given relative world position.
        /// </summary>
        /// <param name="blockPos">The block position.</param>
        /// <returns>The block type.</returns>
        public ServerBlockType GetBlock(BlockPosition blockPos)
        {
            var local = blockPos & ChunkSize.Mask;
            var chunkIndex = ((blockPos.X >> ChunkSize.IntBits) + 1) * 3 * 3
                           + ((blockPos.Y >> ChunkSize.IntBits) + 1) * 3
                           + ((blockPos.Z >> ChunkSize.IntBits) + 1);

            var id = m_Chunks[chunkIndex]?.GetBlockID(local) ?? 0;
            return m_BlockList.GetBlockType(id);
        }
    }
}