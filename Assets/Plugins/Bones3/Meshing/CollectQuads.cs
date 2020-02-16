using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using WraithavenGames.Bones3.BlockProperties;

namespace WraithavenGames.Bones3.Meshing
{
    /// <summary>
    /// This job is designed to iterate over all the blocks in a chunk, for a given material, and
    /// generate the quads which would appear as a result.
    /// </summary>
    public struct CollectQuads : IJob
    {
        /// <summary>
        /// An array of block ids currently in the chunk. The length of the array is exactly 4069,
        /// or 16x16x16. The block at (x, y, z) is located at index x * 16 * 16 + y * 16 + z. At
        /// each position, the value provided is the id value of the block, with 0 being air.
        /// </summary>
        [ReadOnly]
        public NativeArray<ushort> blocks;

        /// <summary>
        /// An array of blocks which surround the outside of this chunk. Used for the purpose of
        /// determining how outter faces should be handled.
        /// </summary>
        [ReadOnly]
        public NativeArray<ushort> nearbyBlocks;

        /// <summary>
        /// A list of block properties for how blocks should be handled in the mesher. All blocks
        /// contained within the block array have a block id object in this list.
        /// </summary>
        [ReadOnly]
        public NativeArray<BlockID> blockProperties;

        /// <summary>
        /// The output list of quads which were collected by this job. Each quad a boolean value,
        /// where quads[x * 16 * 16 * 6 + y * 16 * 6 + z * 6 + j] represents whether the quad is
        /// active or not.
        /// </summary>
        public NativeArray<byte> quads;

        /// <summary>
        /// The block this job is looking for.
        /// </summary>
        public int targetBlock;

        public void Execute()
        {
            BuildFaces(0);
            BuildFaces(1);
            BuildFaces(2);
            BuildFaces(3);
            BuildFaces(4);
            BuildFaces(5);
        }

        private int GetIndex(int t, int a, int b, int j)
        {
            switch (j)
            {
                case 0:
                case 1:
                    return t * 16 * 16 + a * 16 + b;

                case 2:
                case 3:
                    return a * 16 * 16 + t * 16 + b;

                case 4:
                case 5:
                    return a * 16 * 16 + b * 16 + t;
            }

            return -1;
        }

        private bool HasQuad(BlockID block, BlockID next)
        {
            bool quad = block.id == targetBlock;
            quad &= next.transparent > 0;

            if (block.id == next.id)
                quad &= next.viewInsides > 0;

            return quad;
        }

        private void BuildFaces(int j)
        {
            int nearbyOffset = j * 16 * 16;
            int end = j % 2 == 0 ? 15 : 0;

            for (int t = 0; t < 16; t++)
            {
                for (int a = 0; a < 16; a++)
                {
                    for (int b = 0; b < 16; b++)
                    {
                        BlockID block = GetBlockProperties(blocks[GetIndex(t, a, b, j)]);
                        BlockID next;

                        if (t == end)
                            next = GetBlockProperties(nearbyBlocks[a * 16 + b + nearbyOffset]);
                        else
                            next = GetBlockProperties(blocks[GetIndex(j % 2 == 0 ? t + 1 : t - 1, a, b, j)]);

                        quads[GetIndex(t, a, b, j) * 6 + j] = HasQuad(block, next) ? (byte)1 : (byte)0;
                    }
                }
            }
        }

        private BlockID GetBlockProperties(ushort blockId)
        {
            for (int i = 0; i < blockProperties.Length; i++)
                if (blockProperties[i].id == blockId)
                    return blockProperties[i];

            return default;
        }
    }
}