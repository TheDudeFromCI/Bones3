namespace Bones3Rebuilt.Remeshing
{
    // TODO Add locking mechanisms to ensure thread safety to Reset() and SetBlock() methods.

    /// <summary>
    /// Provides a method of storing chunk properties for remeshing.
    /// </summary>
    public class ChunkProperties
    {
        private IMeshBlockDetails[] m_Blocks = new IMeshBlockDetails[0];

        /// <summary>
        /// Gets the size of the chunk being handled.
        /// </summary>
        /// <value>The chunk size.</value>
        public GridSize ChunkSize { get; private set; }

        /// <summary>
        /// Gets the position of the chunk.
        /// </summary>
        /// <value>The chunk position.</value>
        public ChunkPosition ChunkPosition { get; private set; }

        /// <summary>
        /// Prepares this chunk properties for a chunk with the given size and position. This
        /// should only be called by the main thread when tasks are not actively using it. This
        /// method will also clear all block data currently stored.
        /// </summary>
        /// <param name="chunkPos">The chunk position.</param>
        /// <param name="chunkSize">The chunk size.</param>
        /// <remarks>
        /// If the chunk size is less than the allocated memory size, then a new array is allocated.
        /// If analyzing many chunks of different sizes, it minimizes garbage creation by scanning
        /// the largest chunk first.
        /// </remarks>
        public void Reset(ChunkPosition chunkPos, GridSize chunkSize)
        {
            ChunkPosition = chunkPos;
            ChunkSize = chunkSize;

            int blockCount = chunkSize.Volume;
            blockCount += chunkSize.Value * chunkSize.Value * 6; // Neighbor chunks

            if (m_Blocks.Length < blockCount)
                m_Blocks = new IMeshBlockDetails[blockCount];
            else
            {
                for (int i = 0; i < m_Blocks.Length; i++)
                    m_Blocks[i] = null;
            }
        }

        /// <summary>
        /// Sets a block type at the position within this chunk. The block may be up to
        /// one block outside of the chunk bounds. This should only be called from the
        /// main thread, and should not be called when this chunk properties object is
        /// being used by the remesh tasks.
        /// </summary>
        /// <param name="pos">The position of the block.</param>
        /// <returns>The block type.</returns>
        public void SetBlock(BlockPosition pos, IMeshBlockDetails details)
        {
            m_Blocks[BlockIndex(pos)] = details;
        }

        /// <summary>
        /// Gets a block type at the position within this chunk. The block may be up to
        /// one block outside of the chunk bounds.
        /// </summary>
        /// <param name="pos">The position of the block.</param>
        /// <returns>The block type.</returns>
        public IMeshBlockDetails GetBlock(BlockPosition pos)
        {
            return m_Blocks[BlockIndex(pos)];
        }

        /// <summary>
        /// Gets the block list index of the given block position.
        /// </summary>
        /// <param name="pos">The block position.</param>
        /// <returns>The block index.</returns>
        private int BlockIndex(BlockPosition pos)
        {
            if (IsCorners(pos) || IsOutOfBounds(pos))
                throw new System.ArgumentException("Block position out of range!", "pos");

            int j = GetChunkSide(pos);

            if (j == -1)
                return pos.Index(ChunkSize);

            return GetNextBlock(pos, j);
        }

        /// <summary>
        /// Checks if the block position is on the corner of 3 or more relevant chunks.
        /// </summary>
        /// <param name="pos">The block position.</param>
        /// <returns>True if the block position is touching 3 or more relevant chunks.</returns>
        private bool IsCorners(BlockPosition pos)
        {
            int n = 0;

            if (pos.X < 0 || pos.X >= ChunkSize.Value) n++;
            if (pos.Y < 0 || pos.Y >= ChunkSize.Value) n++;
            if (pos.Z < 0 || pos.Z >= ChunkSize.Value) n++;

            return n > 1;
        }

        /// <summary>
        /// Checks if the position is more than one block away from the check.
        /// </summary>
        /// <param name="pos">The block position.</param>
        /// <returns>True if the block position is too far from the chunk.</returns>
        private bool IsOutOfBounds(BlockPosition pos)
        {
            if (pos.X < -1 || pos.X > ChunkSize.Value) return true;
            if (pos.Y < -1 || pos.Y > ChunkSize.Value) return true;
            if (pos.Z < -1 || pos.Z > ChunkSize.Value) return true;

            return false;
        }

        /// <summary>
        /// Gets the side of the chunk the block position is.
        /// </summary>
        /// <param name="pos">The block position.</param>
        /// <returns>The chunk side, or -1 if the position is within the chunk.</returns>
        private int GetChunkSide(BlockPosition pos)
        {
            if (pos.X < 0)
                return 0;

            if (pos.X >= ChunkSize.Value)
                return 1;

            if (pos.Y < 0)
                return 2;

            if (pos.Y >= ChunkSize.Value)
                return 3;

            if (pos.Z < 0)
                return 4;

            if (pos.Z >= ChunkSize.Value)
                return 5;

            return -1;
        }

        /// <summary>
        /// Gets the index of the neighbor block based on the given block position and chunk edge.
        /// </summary>
        /// <param name="pos">The block position.</param>
        /// <param name="j">The chunk edge.</param>
        /// <returns>The block index.</returns>
        private int GetNextBlock(BlockPosition pos, int j)
        {
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

            return pos.Index(ChunkSize) + ChunkSize.Volume;
        }
    }
}
