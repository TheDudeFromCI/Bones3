using System;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// A cubic collection of block IDs.
    /// </summary>
    public class Chunk
    {
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

        /// <summary>
        /// Gets whether or not this chunk has been modified since the last save.
        /// </summary>
        /// <value>True if the chunk has been modified. False otherwise.</value>
        internal bool IsModified { get; set; } = true;

        /// <summary>
        /// Gets the array of blocks being stored in this chunk. (This should not be modified.)
        /// </summary>
        /// <value>The block data.</value>
        internal ushort[] Blocks { get; }

        /// <summary>
        /// Creates a new chunk object.
        /// </summary>
        /// <param name="chunkSize">The chunk size.</param>
        /// <param name="position">The size of this chunk in the world.</param>
        internal Chunk(GridSize chunkSize, ChunkPosition position)
        {
            Size = chunkSize;
            Position = position;
            Blocks = new ushort[Size.Volume];
        }

        /// <summary>
        /// Gets the block ID at the given local block position within the container.
        /// </summary>
        /// <param name="pos">The local block position.</param>
        /// <returns>The block ID.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the block position is not within the container.
        /// </exception>
        public ushort GetBlockID(BlockPosition pos)
        {
            return Blocks[pos.Index(Size)];
        }

        /// <summary>
        /// Sets the block ID at the given local block position within the container.
        /// </summary>
        /// <param name="pos">The local block position.</param>
        /// <param name="id">The block ID to assign.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the block position is not within the container.
        /// </exception>
        public void SetBlockID(BlockPosition pos, ushort id)
        {
            int index = pos.Index(Size);

            if (Blocks[index] == id)
                return;

            Blocks[index] = id;
            IsModified = true;
        }
    }
}
