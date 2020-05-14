using System.Collections.Generic;

namespace Bones3Rebuilt
{
    /// <summary>
    /// A utility class for analyzing a chunk and nearby chunks to retrieve all required block types for remeshing.
    /// </summary>
    public class ChunkAnalyzer
    {
        /// <summary>
        /// A structure for passing around data for iterating over neighbor blocks of a chunk.
        /// </summary>
        struct NeighborBlockPosition
        {
            public int PlaneX { get; set; }
            public int PlaneY { get; set; }
            public int Side { get; set; }
            public int Offset { get; set; }
            public IBlockContainer Chunk { get; set; }
        }

        /// <summary>
        /// Gets the block array data for the chunk.
        /// </summary>
        /// <value>The block data.</value>
        public ushort[] Blocks { get; }

        /// <summary>
        /// Gets the block array data for neighboring chunks.
        /// </summary>
        /// <value>The block data.</value>
        public ushort[] NeighborBlocks { get; }

        /// <summary>
        /// Creates a new chunk analyzer.
        /// </summary>
        /// <param name="containerProvider">The provider to retrieve needed chunks from.</param>
        /// <param name="containerPosition">The position of the container to analyze.</param>
        public ChunkAnalyzer(IBlockContainerProvider containerProvider, ChunkPosition containerPosition)
        {
            int chunkSize = containerProvider.ContainerSize.Value;
            Blocks = new ushort[chunkSize * chunkSize * chunkSize];
            NeighborBlocks = new ushort[chunkSize * chunkSize * 6];

            GetBlocks(containerProvider, containerPosition);
            GetNeighborBlocks(containerProvider, containerPosition);
        }

        /// <summary>
        /// Retrieves all of the blocks from the container and stores them.
        /// </summary>
        /// <param name="containerProvider">The provider to retrieve needed chunks from.</param>
        /// <param name="containerPosition">The position of the container to analyze.</param>
        private void GetBlocks(IBlockContainerProvider containerProvider, ChunkPosition containerPosition)
        {
            var chunk = containerProvider.GetContainer(containerPosition, true);
            int chunkSize = containerProvider.ContainerSize.Value;

            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    for (int z = 0; z < chunkSize; z++)
                    {
                        var pos = new BlockPosition(x, y, z);
                        Blocks[pos.Index(containerProvider.ContainerSize)] = chunk.GetBlockID(pos);
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves all of the required blocks from nearby containers and stores them.
        /// </summary>
        /// <param name="containerProvider">The provider to retrieve needed chunks from.</param>
        /// <param name="containerPosition">The position of the center container.</param>
        private void GetNeighborBlocks(IBlockContainerProvider containerProvider, ChunkPosition containerPosition)
        {
            int chunkSize = containerProvider.ContainerSize.Value;
            foreach (var pos in NeighborBlockIterator(containerProvider, containerPosition))
            {
                if (pos.Chunk == null)
                {
                    int index = pos.PlaneX * chunkSize + pos.PlaneY + pos.Offset;
                    NeighborBlocks[index] = BlockType.UNGENERATED;
                    continue;
                }

                RetrieveBlockFromNeighbor(chunkSize, pos);
            }
        }

        /// <summary>
        /// Gets an iterator which returns all neighbor blocks around the target chunk.
        /// </summary>
        /// <param name="containerProvider">The provider to retrieve needed chunks from.</param>
        /// <param name="containerPosition">The position of the center container.</param>
        /// <returns>The iterator.</returns>
        private IEnumerable<NeighborBlockPosition> NeighborBlockIterator(IBlockContainerProvider containerProvider, ChunkPosition containerPosition)
        {
            int chunkSize = containerProvider.ContainerSize.Value;

            for (int j = 0; j < 6; j++)
            {
                var chunkPos = containerPosition.ShiftAlongDirection(j);
                var chunk = containerProvider.GetContainer(chunkPos, false);

                for (int a = 0; a < chunkSize; a++)
                    for (int b = 0; b < chunkSize; b++)
                        yield return new NeighborBlockPosition
                        {
                            Side = j,
                            PlaneX = a,
                            PlaneY = b,
                            Offset = j * chunkSize * chunkSize,
                            Chunk = chunk,
                        };
            }
        }

        /// <summary>
        /// Pulls the block from a neighbor chunk and stores it based on the block position data.
        /// </summary>
        /// <param name="chunkSize">The size of the chunk.</param>
        /// <param name="blockPos">The block position data to sample from.</param>
        private void RetrieveBlockFromNeighbor(int chunkSize, NeighborBlockPosition blockPos)
        {
            int index = blockPos.PlaneX * chunkSize + blockPos.PlaneY + blockPos.Offset;
            int planeZ = blockPos.Side % 2 == 0 ? 0 : chunkSize - 1;

            switch (blockPos.Side)
            {
                case 0:
                case 1:
                    NeighborBlocks[index] = blockPos.Chunk.GetBlockID(
                        new BlockPosition(planeZ, blockPos.PlaneX, blockPos.PlaneY));
                    break;

                case 2:
                case 3:
                    NeighborBlocks[index] = blockPos.Chunk.GetBlockID(
                        new BlockPosition(blockPos.PlaneX, planeZ, blockPos.PlaneY));
                    break;

                case 4:
                case 5:
                    NeighborBlocks[index] = blockPos.Chunk.GetBlockID(
                        new BlockPosition(blockPos.PlaneX, blockPos.PlaneY, planeZ));
                    break;
            }
        }
    }
}