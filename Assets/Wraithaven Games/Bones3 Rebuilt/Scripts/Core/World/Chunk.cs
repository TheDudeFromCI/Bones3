using System;

namespace Bones3Rebuilt
{
    /// <summary>
    /// A cubic collection of block IDs.
    /// </summary>
    public class Chunk : IBlockContainer
    {
        private readonly ushort[] m_Blocks;

        /// <summary>
        /// The number of blocks in this chunk along a single axis.
        /// </summary>
        /// <value>The size of this chunk.</value>
        public GridSize Size { get; }

        /// <summary>
        /// Gets the position of this chunk within the world, in chunk coordinates.
        /// </summary>
        /// <value>The chunk position.</value>
        public ChunkPosition Position { get; }

        /// <inheritdoc cref="IBlockContainer"/>
        public event BlockContainerModifiedCallback OnBlockContainerModified;

        /// <summary>
        /// Creates a new chunk object.
        /// </summary>
        /// <param name="chunkSize">The chunk size.</param>
        /// <param name="position">The size of this chunk in the world.</param>
        public Chunk(GridSize chunkSize, ChunkPosition position)
        {
            Size = chunkSize;
            Position = position;

            m_Blocks = new ushort[Size.Value * Size.Value * Size.Value];
        }

        /// <inheritdoc cref="IBlockContainer"/>
        public ushort GetBlockID(BlockPosition pos)
        {
            return m_Blocks[pos.Index(Size)];
        }

        /// <inheritdoc cref="IBlockContainer"/>
        public void SetBlockID(BlockPosition pos, ushort id)
        {
            OnBlockContainerModified?.Invoke(new BlockContainerModifiedEvent(this, pos, id));
            m_Blocks[pos.Index(Size)] = id;
        }
    }
}
