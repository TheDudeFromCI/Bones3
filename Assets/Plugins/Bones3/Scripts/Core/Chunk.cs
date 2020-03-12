using UnityEngine;

namespace Bones3
{
    /// <summary>
    /// A chunk is a 16x16x16 grid of blocks representing a small piece of a voxel world.
    /// </summary>
    public class Chunk : MonoBehaviour
    {
        private Vector3Int pos;
        private ushort[] blocks = new ushort[16 * 16 * 16];

        /// <summary>
        /// The X coordinate of this chunk.
        /// </summary>
        /// <value>The X coord.</value>
        public int X { get => pos.x; }

        /// <summary>
        /// The Y coordinate of this chunk.
        /// </summary>
        /// <value>The Y coord.</value>
        public int Y { get => pos.y; }

        /// <summary>
        /// The Z coordinate of this chunk.
        /// </summary>
        /// <value>The Z coord.</value>
        public int Z { get => pos.z; }

        /// <summary>
        /// The number of non-air blocks in this chunk.
        /// </summary>
        /// <value>The block count.</value>
        public int BlockCount { get; private set; } = 0;

        /// <summary>
        /// Sets the coordinates of this chunk.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="z">The Z coordinate.</param>
        void SetCoords(int x, int y, int z)
        {
            pos.x = x;
            pos.y = y;
            pos.z = z;
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
        public bool SetBlock(int x, int y, int z, ushort block)
        {
            int index = Index(x, y, z);

            if (blocks[index] == block)
                return false;

            if (blocks[index] == 0)
                BlockCount++;
            else if (block == 0)
                BlockCount--;

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
        private int Index(int x, int y, int z)
        {
            if (x < 0 || y < 0 || z < 0 || x > 15 || y > 15 || z > 15)
                throw new Bones3Exception($"Block index out of range! ({x},{y},{z})");

            return x * 16 * 16 + y * 16 + z;
        }
    }
}