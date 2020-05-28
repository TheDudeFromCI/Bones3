using WraithavenGames.Bones3;
using UnityEngine;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// Generates a flat plane for the world.
    /// </summary>
    [AddComponentMenu("Bones3/World Generators/Flat Terrain")]
    public class FlatTerrainGenerator : WorldGenerator
    {
        [Tooltip("The ID of the block to place on the surface.")]
        [SerializeField] protected ushort m_SurfaceBlockID = 2;

        [Tooltip("The ID of the block to place under the surface.")]
        [SerializeField] protected ushort m_GroundBlockID = 3;

        /// <inheritdoc cref="WorldGenerator"/>
        protected override void GenerateChunk(Chunk chunk)
        {
            int size = chunk.Size.Value;

            for (int x = 0; x < size; x++)
            {
                for (int z = 0; z < size; z++)
                {
                    int h = -chunk.Position.Y * size;

                    for (int y = 0; y <= h; y++)
                    {
                        if (y == size)
                            break;

                        if (y == h)
                            chunk.SetBlockID(new BlockPosition(x, y, z), m_SurfaceBlockID);
                        else
                            chunk.SetBlockID(new BlockPosition(x, y, z), m_GroundBlockID);
                    }
                }
            }
        }
    }
}