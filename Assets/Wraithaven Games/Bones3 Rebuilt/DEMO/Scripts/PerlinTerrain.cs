using WraithavenGames.Bones3;
using UnityEngine;

namespace WraithavenGames.Bones3Demo
{
    public class PerlinTerrain : WorldGenerator
    {
        protected override void GenerateChunk(Chunk chunk)
        {
            int size = chunk.Size.Value;

            for (int x = 0; x < size; x++)
            {
                for (int z = 0; z < size; z++)
                {
                    float worldX = x + chunk.Position.X * size;
                    float worldZ = z + chunk.Position.Z * size;

                    int h = Mathf.FloorToInt(Mathf.PerlinNoise(worldX, worldZ) * 25);
                    h -= chunk.Position.Y * size;
                    h = Mathf.Min(h, size);

                    for (int y = 0; y < h; y++)
                        chunk.SetBlockID(new BlockPosition(x, y, z), 2);
                }
            }
        }
    }
}