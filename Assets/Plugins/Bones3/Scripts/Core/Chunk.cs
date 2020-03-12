using UnityEngine;

namespace Bones3
{
    /// <summary>
    /// A chunk is a 16x16x16 grid of blocks representing a small piece of a voxel world.
    /// </summary>
    public class Chunk : MonoBehaviour
    {
        /// <summary>
        /// The number of blocks within one axis of a chunk.
        /// </summary>
        public const int CHUNK_SIZE = 16;

        private ushort[] blocks = new ushort[CHUNK_SIZE * CHUNK_SIZE * CHUNK_SIZE];
        private Vector3Int position;
        private int blockCount;

        /// <summary>
        /// The X coordinate of this chunk.
        /// </summary>
        /// <value>The X coord.</value>
        public int X { get => position.x; }

        /// <summary>
        /// The Y coordinate of this chunk.
        /// </summary>
        /// <value>The Y coord.</value>
        public int Y { get => position.y; }

        /// <summary>
        /// The Z coordinate of this chunk.
        /// </summary>
        /// <value>The Z coord.</value>
        public int Z { get => position.z; }

        /// <summary>
        /// The number of non-air blocks in this chunk.
        /// </summary>
        /// <value>The block count.</value>
        public int BlockCount { get => blockCount; }

        /// <summary>
        /// Sets the coordinates of this chunk.
        /// </summary>
        /// <remarks>
        /// This method should only be called when creating the chunk by the
        /// chunk container object. It is assumed that all chunks within a world
        /// have a unquie coordinate position along a grid.
        /// 
        /// A chunk's coordinate location is equal to `a` >> 4, where `a` is the
        /// world coordinates of the block in this chunk at relative position
        /// (0, 0, 0).
        /// </remarks>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="z">The Z coordinate.</param>
        public void SetCoords(int x, int y, int z)
        {
            position = new Vector3Int(x, y, z);
        }

        /// <summary>
        /// Gets the Block ID at the specified location within the chunk.
        /// </summary>
        /// <param name="x">The relative X position.</param>
        /// <param name="y">The relative Y position.</param>
        /// <param name="z">The relative Z position.</param>
        /// <returns>The Block ID.</returns>
        /// <exception cref="Bones3Exception">If the position is outside of this chunk.</exception>
        public ushort GetBlock(int x, int y, int z)
        {
            return blocks[Index(x, y, z)];
        }

        /// <summary>
        /// Sets the Block ID at the specified location within the chunk.
        /// </summary>
        /// <param name="x">The relative X position.</param>
        /// <param name="y">The relative Y position.</param>
        /// <param name="z">The relative Z position.</param>
        /// <param name="block">The Block ID to set.</param>
        /// <returns>True if the block was updated. False if a block with a matching ID was already at the given location.</returns>
        /// <exception cref="Bones3Exception">If the position is outside of this chunk.</exception>
        public bool SetBlock(int x, int y, int z, ushort block)
        {
            int index = Index(x, y, z);

            if (blocks[index] == block)
                return false;

            if (blocks[index] == 0)
                blockCount++;
            else if (block == 0)
                blockCount--;

            blocks[index] = block;
            return true;
        }

        /// <summary>
        /// Gets the index of a block within the block array based on it's position.
        /// </summary>
        /// <param name="x">The relative X position.</param>
        /// <param name="y">The relative Y position.</param>
        /// <param name="z">The relative Z position.</param>
        /// <returns>The block index within the blocks array.</returns>
        /// <exception cref="Bones3Exception">If the position is outside of this chunk.</exception>
        private int Index(int x, int y, int z)
        {
            if (x < 0 || y < 0 || z < 0 || x >= CHUNK_SIZE || y >= CHUNK_SIZE || z >= CHUNK_SIZE)
                throw new Bones3Exception($"Block index out of range! ({x},{y},{z})");

            return x * CHUNK_SIZE * CHUNK_SIZE + y * CHUNK_SIZE + z;
        }
    }
}