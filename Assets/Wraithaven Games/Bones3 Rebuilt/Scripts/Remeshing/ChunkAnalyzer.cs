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
        internal static void LoadProperties(ChunkProperties properties, ServerBlockList blockList, World world, ChunkPosition chunkPos)
        {
            GetChunkGrid(world, chunkPos);
            properties.ChunkPosition = chunkPos;

            int size = world.ChunkSize.Value;
            int intBits = world.ChunkSize.IntBits;
            int extendedSize = size + 2;

            for (int x = -1; x <= size; x++)
                for (int y = -1; y <= size; y++)
                    for (int z = -1; z <= size; z++)
                    {
                        var local = (x & world.ChunkSize.Mask) * size * size
                                  + (y & world.ChunkSize.Mask) * size
                                  + (z & world.ChunkSize.Mask);

                        var chunkIndex = ((x >> intBits) + 1) * 3 * 3
                                       + ((y >> intBits) + 1) * 3
                                       + ((z >> intBits) + 1);

                        var index = (x + 1) * extendedSize * extendedSize
                                  + (y + 1) * extendedSize
                                  + (z + 1);

                        var blockId = m_ChunkBuffer[chunkIndex]?.Blocks[local] ?? 0;
                        var blockType = blockList.GetBlockType(blockId);
                        properties.Blocks[index] = blockType;
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
    }
}