using System.Collections.Generic;

using Bones3Rebuilt;

using UnityEngine;

namespace WraithavenGames.Bones3.Demo
{
    public class PerlinTerrain : MonoBehaviour, IEditBatch
    {
        [Header("Dimensions")]
        public int width = 100;
        public int height = 100;

        [Range(1f, 40f)]
        public float amplitude = 20f;

        [Header("Perlin Noise")]
        [Range(1f, 40f)]
        public float smoothing = 20f;

        [Header("Reference")]
        public BlockWorld blockWorld;

        void Start()
        {
            blockWorld.WorldContainer.BlockList.AddBlockType(new BlockBuilder(2)
                .Name("Grass")
                .Build());

            blockWorld.WorldContainer.BlockList.AddBlockType(new BlockBuilder(3)
                .Name("Stone")
                .Build());

            blockWorld.WorldContainer.SetBlocks(this);
        }

        public IEnumerable<BlockPlacement> GetBlocks()
        {
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    float a = x / smoothing;
                    float b = z / smoothing;
                    int h = (int) (Mathf.PerlinNoise(a, b) * amplitude);

                    for (int y = -5; y <= h; y++)
                    {
                        var place = new BlockPlacement
                        {
                            Position = new BlockPosition(x, y, z),
                            BlockID = (ushort) (y == h ? 2 : 3),
                        };

                        yield return place;
                    }

                    for (int y = h + 1; y <= (int) amplitude + 5; y++)
                    {
                        var place = new BlockPlacement
                        {
                            Position = new BlockPosition(x, y, z),
                            BlockID = BlockType.AIR,
                        };

                        yield return place;
                    }
                }
            }
        }
    }
}
