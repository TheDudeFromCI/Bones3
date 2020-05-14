using System.Collections.Generic;

namespace Bones3Rebuilt
{
    /// <summary>
    /// An edit batch can be used to edit multiple blocks at once, efficiently.
    /// </summary>
    /// <remarks>
    /// Using edit batches is very fast for editing large number of blocks at
    /// once. It does not trigger a chunk remesh until after each block is
    /// placed.
    /// </remarks>
    public interface IEditBatch
    {
        /// <summary>
        /// Enumerates over all blocks which need to be placed.
        /// </summary>
        /// <returns>The blocks to place.</returns>
        IEnumerable<BlockPlacement> GetBlocks();
    }

    /// <summary>
    /// Details for what block should be placed and where within an edit batch.
    /// </summary>
    public struct BlockPlacement
    {
        /// <summary>
        /// Gets the position of the block within the world.
        /// </summary>
        /// <value>The block position.</value>
        public BlockPosition Position { get; set; }

        /// <summary>
        /// Gets the block ID to place.
        /// </summary>
        /// <value>The block ID.</value>
        public ushort BlockID { get; set; }
    }
}