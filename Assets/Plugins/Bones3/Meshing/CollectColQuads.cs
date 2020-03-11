using Unity.Collections;
using Unity.Jobs;
using WraithavenGames.Bones3.BlockProperties;

namespace WraithavenGames.Bones3.Meshing
{
    /// <summary>
    /// This job is designed to iterate over all the blocks in a chunk and generate the collision
    /// quads which would appear as a result.
    /// </summary>
    public struct CollectColQuads : IJob
    {
        /// <summary>
        /// An array of block ids currently in the chunk. The length of the array is exactly 4069,
        /// or 16x16x16. The block at (x, y, z) is located at index x * 16 * 16 + y * 16 + z. At
        /// each position, the value provided is the id value of the block, with 0 being air.
        /// </summary>
        [ReadOnly]
        public NativeArray<ushort> blocks;

        /// <summary>
        /// A list of block properties for how blocks should be handled in the mesher. All blocks
        /// contained within the block array have a block id object in this list.
        /// </summary>
        [ReadOnly]
        public NativeArray<BlockID> blockProperties;

        /// <summary>
        /// The output list of quads which were collected by this job. Each quad a boolean value,
        /// where quads[(x * 16 * 16 + y * 16 + z) * 6 + j] represents whether the quad is active
        /// or not.
        /// </summary>
        public NativeArray<byte> quads;

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

                default:
                    throw new System.Exception();
            }
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
                        byte nextCol = 0;

                        if (t != end)
                            nextCol = GetBlockProperties(blocks[GetIndex(j % 2 == 0 ? t + 1 : t - 1, a, b, j)]).hasCollision;

                        quads[GetIndex(t, a, b, j) * 6 + j] = (byte)(block.hasCollision & (1 - nextCol));
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