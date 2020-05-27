namespace WraithavenGames.Bones3
{
    /// <summary>
    /// A utility for analyzing a chunk region and converting it into a chunk properties object.
    /// This class is not thread safe.
    /// </summary>
    internal static class ChunkAnalyzer
    {
        private static readonly Chunk[] m_ChunkBuffer = new Chunk[27];

        /// <summary>
        /// Loads the block properties from the world into the chunk properties object.
        /// </summary>
        /// <param name="properties">The chunk properties to write to.</param>
        /// <param name="blockList">The block list to reference.</param>
        /// <param name="world">The world to load from.</param>
        /// <param name="chunkPos">The target chunk.</param>
        internal static void LoadProperties(ChunkProperties properties, BlockList blockList, World world, ChunkPosition chunkPos)
        {
            GetChunkGrid(world, chunkPos);
            properties.ChunkPosition = chunkPos;

            int size = world.ChunkSize.Value;
            for (int x = -1; x <= size; x++)
                for (int y = -1; y <= size; y++)
                    for (int z = -1; z <= size; z++)
                    {
                        var blockPos = new BlockPosition(x, y, z);
                        var blockId = GetBlock(blockPos, world.ChunkSize);
                        var blockType = blockList.GetBlockType(blockId);
                        properties.SetBlock(blockPos, blockType);
                    }
        }

        /// <summary>
        /// Loads all nearby chunks into the chunk buffer grid.
        /// </summary>
        /// <param name="world">The world to load from.</param>
        /// <param name="chunkPos">The center chunk.</param>
        private static void GetChunkGrid(World world, ChunkPosition chunkPos)
        {
            for (int x = 0; x < 3; x++)
                for (int y = 0; y < 3; y++)
                    for (int z = 0; z < 3; z++)
                        m_ChunkBuffer[x * 3 * 3 + y * 3 + z] = world.GetChunk(new ChunkPosition(x - 1, y - 1, z - 1) + chunkPos);
        }

        /// <summary>
        /// Gets the block within the chunk grid based on the given block position.
        /// </summary>
        /// <param name="blockPos">The block position.</param>
        /// <param name="chunkSize">The chunk size.</param>
        /// <returns>The block ID.</returns>
        private static ushort GetBlock(BlockPosition blockPos, GridSize chunkSize)
        {
            var local = blockPos & chunkSize.Mask;
            var chunkIndex = ((blockPos.X >> chunkSize.IntBits) + 1) * 3 * 3
                           + ((blockPos.Y >> chunkSize.IntBits) + 1) * 3
                           + ((blockPos.Z >> chunkSize.IntBits) + 1);

            return m_ChunkBuffer[chunkIndex]?.GetBlockID(local) ?? 0;
        }
    }
}