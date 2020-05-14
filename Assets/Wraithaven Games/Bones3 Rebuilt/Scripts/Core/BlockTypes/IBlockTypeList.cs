namespace Bones3Rebuilt
{
    public interface IBlockTypeList
    {
        /// <summary>
        /// The number of block types currently in this list.
        /// </summary>
        /// <value>The number of block types.</value>
        int Count { get; }

        /// <summary>
        /// Gets the next available block ID within this list.
        /// </summary>
        /// <returns>The next available block ID.</returns>
        ushort NextBlockID { get; }

        /// <summary>
        /// Gets the block type with the given ID.
        /// </summary>
        /// <param name="id">The block id.</param>
        BlockType GetBlockType(ushort id);

        /// <summary>
        /// Adds a new block type to this list.
        /// </summary>
        /// <param name="blockType">The block type to add.</param>
        /// <exception cref="System.ArgumentException">
        /// If the block count is currently at MAX_BLOCK_TYPES.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// If the block's ID does not match the NextAvailableID property
        /// of this list.
        /// </exception>
        void AddBlockType(BlockType blockType);

        /// <summary>
        /// Removes a block type from this list.
        /// </summary>
        /// <param name="blockType">The block type to remove.</param>
        /// <exception cref="System.AccessViolationException">
        /// If the block type is protected.
        /// </exception>
        void RemoveBlockType(BlockType blockType);
    }
}
