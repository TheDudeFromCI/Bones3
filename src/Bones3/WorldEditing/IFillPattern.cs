namespace Bones3Rebuilt
{
    /// <summary>
    /// A fill pattern is used to determine which block should be placed at a
    /// given point during an edit.
    /// </summary>
    public interface IFillPattern
    {
        /// <summary>
        /// Gets the block id which should be placed at the given point.
        /// </summary>
        /// <param name="pos">The block position.</param>
        /// <returns>The block id.</returns>
        ushort GetBlockID(BlockPosition pos);
    }
}
