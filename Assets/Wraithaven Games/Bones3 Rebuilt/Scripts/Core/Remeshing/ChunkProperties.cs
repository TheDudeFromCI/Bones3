using System.Collections.Generic;

namespace Bones3Rebuilt
{
    /// <summary>
    /// Provides a thread-safe method of storing chunk properties for remeshing.
    /// </summary>
    public class ChunkProperties : IChunkProperties
    {
        private readonly Dictionary<ushort, BlockType> m_BlockProperties = new Dictionary<ushort, BlockType>();
        private readonly ushort[] m_Blocks;
        private readonly ushort[] m_NeighborBlocks;

        /// <inheritdoc cref="IChunkProperties"/>
        public GridSize ChunkSize { get; private set; }

        /// <inheritdoc cref="IChunkProperties"/>
        public ChunkPosition ChunkPosition { get; private set; }

        /// <summary>
        /// Creates a new chunk properties container for the given chunk.
        /// </summary>
        /// <param name="containerProvider">The provider to retrieve needed chunks from.</param>
        /// <param name="containerPosition">The position of the container to analyze.</param>
        /// <param name="blockTypes">The list of block types to reference.</param>
        public ChunkProperties(IBlockContainerProvider containerProvider, ChunkPosition containerPosition, IBlockTypeList blockTypes)
        {
            ChunkSize = containerProvider.ContainerSize;
            ChunkPosition = containerPosition;

            var analyzer = new ChunkAnalyzer(containerProvider, containerPosition);
            m_Blocks = analyzer.Blocks;
            m_NeighborBlocks = analyzer.NeighborBlocks;

            AddBlockProperties(blockTypes, m_Blocks);
            AddBlockProperties(blockTypes, m_NeighborBlocks);
        }

        /// <summary>
        /// Retrieves the required block properties for the given block array.
        /// </summary>
        /// <param name="blockTypes">The list of block types.</param>
        /// <param name="array">The block array to iterate over.</param>
        private void AddBlockProperties(IBlockTypeList blockTypes, ushort[] array)
        {
            int last = -1; // A slight optimization to reduce dictionary lookups.
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == last)
                    continue;

                if (m_BlockProperties.ContainsKey(array[i]))
                    continue;

                var blockType = blockTypes.GetBlockType(array[i]);
                m_BlockProperties[array[i]] = blockType;
                last = array[i];
            }
        }

        /// <inheritdoc cref="IChunkProperties"/>
        public BlockType GetBlock(BlockPosition pos)
        {
            return m_BlockProperties[m_Blocks[pos.Index(ChunkSize)]];
        }

        /// <inheritdoc cref="IChunkProperties"/>
        public BlockType GetNextBlock(BlockPosition pos, int j)
        {
            pos = pos.ShiftAlongDirection(j);

            if (pos.IsWithinGrid(ChunkSize))
                return m_BlockProperties[m_Blocks[pos.Index(ChunkSize)]];

            switch (j)
            {
                case 0:
                case 1:
                    pos = new BlockPosition(j, pos.Y, pos.Z);
                    break;

                case 2:
                case 3:
                    pos = new BlockPosition(j, pos.X, pos.Z);
                    break;

                case 4:
                case 5:
                    pos = new BlockPosition(j, pos.X, pos.Y);
                    break;
            }

            return m_BlockProperties[m_NeighborBlocks[pos.Index(ChunkSize)]];
        }
    }
}
