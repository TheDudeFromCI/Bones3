using UnityEngine;
using System.Collections.Generic;

namespace Bones3
{
    public class World : MonoBehaviour
    {
        [SerializeField, HideInInspector] private List<Region> regions = new List<Region>();

        /// <summary>
        /// The number of chunks in this world.
        /// </summary>
        /// <value>The chunk count.</value>
        public int ChunkCount
        {
            get
            {
                int total = 0;

                foreach (var region in regions)
                    total += region.ChunkCount;

                return total;
            }
        }

        /// <summary>
        /// Gets the chunk at the specified location within the world.
        /// </summary>
        /// <param name="x">The relative X position.</param>
        /// <param name="y">The relative Y position.</param>
        /// <param name="z">The relative Z position.</param>
        /// <param name="create">Whether a chunk should be created if it doesn't currently exist.</param>
        /// <returns>The chunk.</returns>
        public Chunk GetChunk(int x, int y, int z, bool create)
        {
            int rX = x >> Region.REGION_BITS;
            int rY = y >> Region.REGION_BITS;
            int rZ = z >> Region.REGION_BITS;

            foreach (var region in regions)
                if (region.X == rX && region.Y == rY && region.Z == rZ)
                    return region.GetChunk(x, y, z, create);

            if (create)
                return CreateRegion(rX, rX, rZ).GetChunk(x, y, z, true);

            return null;
        }

        /// <summary>
        /// Creates a new region at the given location.
        /// </summary>
        /// <param name="x">The relative X position.</param>
        /// <param name="y">The relative Y position.</param>
        /// <param name="z">The relative Z position.</param>
        /// <returns>The newly created chunk.</returns>
        private Region CreateRegion(int x, int y, int z)
        {
            GameObject go = new GameObject($"Region ({x}, {y}, {z})");
            go.transform.SetParent(transform);
            go.transform.localPosition = new Vector3(x, y, z) * Region.REGION_SIZE * Chunk.CHUNK_SIZE;

            Region region = go.AddComponent<Region>();
            region.SetCoords(x, y, z);

            regions.Add(region);
            return region;
        }

        public void Clear()
        {
            regions.Clear();

            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                if (Application.isPlaying)
                    Object.Destroy(transform.GetChild(i).gameObject);
                else
                    Object.DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
    }
}