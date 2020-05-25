namespace Bones3Rebuilt.Database.ChunkData
{
    /// <summary>
    /// Represents a block of chunk data which can be saved and loaded. This file
    /// should not be modified while the chunk data save/load task is running.
    /// </summary>
    public class ChunkData
    {
        /// <summary>
        /// Gets an array of all the block data stored within the chunk.
        /// </summary>
        /// <value>The block ID data.</value>
        public ushort[] Blocks { get; }

        /// <summary>
        /// Gets the size of this chunk.
        /// </summary>
        /// <value>The chunk size.</value>
        public GridSize Size { get; }

        /// <summary>
        /// Gets the position of the chunk within the world.
        /// </summary>
        /// <value>The chunk possition.</value>
        public ChunkPosition Position { get; set; }

        /// <summary>
        /// Creates a new chunk data block for the given chunk size.
        /// </summary>
        /// <param name="size">The size of the chunk.</param>
        public ChunkData(GridSize size)
        {
            Size = size;
            Blocks = new ushort[size.Volume];
        }
    }
}