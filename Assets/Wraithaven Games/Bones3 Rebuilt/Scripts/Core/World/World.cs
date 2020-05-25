using System.Collections.Generic;

namespace Bones3Rebuilt.World
{
    /// <summary>
    /// A container for the storage of all world data involved a voxel world representation.
    /// </summary>
    public class World
    {
        private readonly List<Chunk> m_Chunks = new List<Chunk>();

        /// <summary>
        /// The number of blocks in a chunk along a single axis.
        /// </summary>
        /// <value>The size of a chunk.</value>
        public GridSize ChunkSize { get; }

        /// <summary>
        /// Creates a new world object.
        /// </summary>
        /// <param name="chunkSize">The chunk size.</param>
        public World(GridSize chunkSize) => ChunkSize = chunkSize;

        /// <summary>
        /// Gets whether or not the target chunk exists.
        /// </summary>
        /// <param name="pos">The position of the chunk.</param>
        /// <returns>True if the chunk exists, false otherwise.</returns>
        public bool DoesChunkExist(ChunkPosition pos) => GetChunk(pos) != null;

        /// <summary>
        /// Gets the chunk at the given chunk position.
        /// </summary>
        /// <param name="pos">The position of the chunk.</param>
        /// <returns>The block container, or null if it doesn't exist.</returns>
        public Chunk GetChunk(ChunkPosition pos)
        {
            foreach (var chunk in m_Chunks)
                if (chunk.Position.Equals(pos))
                    return chunk;

            return null;
        }

        /// <summary>
        /// Creates a new chunk at the given chunk position.
        /// </summary>
        /// <param name="pos">The chunk position.</param>
        /// <returns>
        /// The newly created chunk, or the current chunk if if already exists.
        /// </returns>
        public Chunk CreateChunk(ChunkPosition pos)
        {
            var oldChunk = GetChunk(pos);
            if (oldChunk != null)
                return oldChunk;

            var newChunk = new Chunk(ChunkSize, pos);
            m_Chunks.Add(newChunk);

            return newChunk;
        }

        /// <summary>
        /// Destroys the chunk at the given chunk coordinates if it exists.
        /// </summary>
        /// <param name="pos">The chunk position.</param>
        public void DestroyChunk(ChunkPosition pos)
        {
            var chunk = GetChunk(pos);
            if (chunk != null)
                m_Chunks.Remove(chunk);
        }
    }
}
