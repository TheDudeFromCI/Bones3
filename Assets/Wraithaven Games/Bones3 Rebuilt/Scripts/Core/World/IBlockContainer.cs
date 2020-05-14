using System;

namespace Bones3Rebuilt
{
    /// <summary>
    /// A container which holds a collection of block ids within a cube of a given size.
    /// </summary>    
    public interface IBlockContainer
    {
        /// <summary>
        /// Gets the size of the block container.
        /// </summary>
        /// <value>The block container size.</value>
        GridSize Size { get; }

        /// <summary>
        /// Gets the position of the block container relative to other block containers.
        /// </summary>
        /// <value>The block container position.</value>
        ChunkPosition Position { get; }

        /// <summary>
        /// Called when this chunk is modified.
        /// </summary>
        event BlockContainerModifiedCallback OnBlockContainerModified;

        /// <summary>
        /// Gets the block ID at the given local block position within the container.
        /// </summary>
        /// <param name="pos">The local block position.</param>
        /// <returns>The block ID.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the block position is not within the container.
        /// </exception>
        ushort GetBlockID(BlockPosition pos);

        /// <summary>
        /// Sets the block ID at the given local block position within the container.
        /// </summary>
        /// <param name="pos">The local block position.</param>
        /// <param name="id">The block ID to assign.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the block position is not within the container.
        /// </exception>
        void SetBlockID(BlockPosition pos, ushort id);
    }
}
