namespace Bones3Rebuilt
{
    /// <summary>
    /// A simple fill pattern which flood fills the area with a single block type.
    /// </summary>
    public class FloodFill : IFillPattern
    {
        private readonly ushort m_BlockType;

        /// <summary>
        /// Creates a new flood fill pattern.
        /// </summary>
        /// <param name="blockType">The block type to place.</param>
        public FloodFill(ushort blockType) => m_BlockType = blockType;

        /// <inheritdoc cref="IFillPattern"/>
        public ushort GetBlockID(BlockPosition pos) => m_BlockType;
    }
}
