using UnityEngine;

namespace Bones3
{
    /// <summary>
    /// A region is a collection of 32 x 32 x 32 chunks.
    /// </summary>
    public class Region : MonoBehaviour
    {
        public const int REGION_SIZE = 32;

        private Chunk[] chunks = new Chunk[REGION_SIZE * REGION_SIZE * REGION_SIZE];
        private Vector3Int position;
        private int chunkCount;

        /// <summary>
        /// The X coordinate of this region.
        /// </summary>
        /// <value>The X coord.</value>
        public int X { get => position.x; }

        /// <summary>
        /// The Y coordinate of this region.
        /// </summary>
        /// <value>The Y coord.</value>
        public int Y { get => position.y; }

        /// <summary>
        /// The Z coordinate of this region.
        /// </summary>
        /// <value>The Z coord.</value>
        public int Z { get => position.z; }

        /// <summary>
        /// The number of non-empty chunks in this region.
        /// </summary>
        /// <value>The chunk count.</value>
        public int ChunkCount { get => chunkCount; }

        /// <summary>
        /// Sets the coordinates of this region.
        /// </summary>
        /// <remarks>
        /// This method should only be called when creating the region by the
        /// region container object. It is assumed that all region within a world
        /// have a unquie coordinate position along a grid.
        /// 
        /// A regions's coordinate location is equal to `a` >> 4, where `a` is the
        /// world coordinates of the block in this region at relative position
        /// (0, 0, 0).
        /// </remarks>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="z">The Z coordinate.</param>
        void SetCoords(int x, int y, int z)
        {
            position = new Vector3Int(x, y, z);
        }

        /// <summary>
        /// Gets the chunk at the specified location within the region.
        /// </summary>
        /// <param name="x">The relative X position.</param>
        /// <param name="y">The relative Y position.</param>
        /// <param name="z">The relative Z position.</param>
        /// <param name="create">Whether a chunk should be created if it doesn't currently exist.</param>
        /// <returns>The chunk.</returns>
        /// <exception cref="Bones3Exception">If the position is outside of this region.</exception>
        public Chunk GetChunk(int x, int y, int z, bool create)
        {
            var chunk = chunks[Index(x, y, z)];

            if (chunk == null && create)
                chunk = CreateChunk(x, y, z);

            return chunk;
        }

        /// <summary>
        /// Creates a new chunk at the given location.
        /// </summary>
        /// <param name="x">The relative X position.</param>
        /// <param name="y">The relative Y position.</param>
        /// <param name="z">The relative Z position.</param>
        /// <returns>The newly created chunk.</returns>
        /// <exception cref="Bones3Exception">If the position is outside of this region.</exception>
        private Chunk CreateChunk(int x, int y, int z)
        {
            if (chunks[Index(x, y, z)] != null)
                throw new Bones3Exception("Chunk already exists!");


            GameObject go = new GameObject($"Chunk ({z},{y},{z})");
            Chunk chunk = go.AddComponent<Chunk>();
            chunk.SetCoords(x, y, z);

            chunkCount++;

            chunks[Index(x, y, z)] == chunk;
            return chunk;
        }

        /// <summary>
        /// Destroys the chunk at the given location.
        /// </summary>
        /// <param name="x">The relative X position.</param>
        /// <param name="y">The relative Y position.</param>
        /// <param name="z">The relative Z position.</param>
        /// <exception cref="Bones3Exception">If the position is outside of this region.</exception>
        public void DestroyChunk(int x, int y, int z)
        {
            int index = Index(x, y, z);

            if (chunks[index] == null)
                return;

            if (Application.isPlaying)
                Object.Destroy(chunks[index].gameObject);
            else
                Object.DestroyImmediate(chunks[index].gameObject);

            chunks[index] = null;
        }

        /// <summary>
        /// Gets the index of a chunk within the chunk array based on it's position.
        /// </summary>
        /// <param name="x">The relative X position.</param>
        /// <param name="y">The relative Y position.</param>
        /// <param name="z">The relative Z position.</param>
        /// <returns>The chunk index within the chunks array.</returns>
        /// <exception cref="Bones3Exception">If the position is outside of this region.</exception>
        private int Index(int x, int y, int z)
        {
            if (x < 0 || y < 0 || z < 0 || x >= REGION_SIZE || y >= REGION_SIZE || z >= REGION_SIZE)
                throw new Bones3Exception($"Chunk index out of range! ({x},{y},{z})");

            return x * REGION_SIZE * REGION_SIZE + y * REGION_SIZE + z;
        }
    }
}