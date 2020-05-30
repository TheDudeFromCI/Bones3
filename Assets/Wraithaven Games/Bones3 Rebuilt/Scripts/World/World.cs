using System;
using System.Collections.Generic;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// A container for the storage of all world data involved a voxel world representation.
    /// </summary>
    internal class World
    {
        private readonly List<Chunk> m_Chunks = new List<Chunk>();

        /// <summary>
        /// The number of blocks in a chunk along a single axis.
        /// </summary>
        /// <value>The size of a chunk.</value>
        internal GridSize ChunkSize { get; }

        /// <summary>
        /// Gets this world's GUID value.
        /// </summary>
        /// <value>The world ID.</value>
        internal string ID { get; }

        /// <summary>
        /// Creates a new world object.
        /// </summary>
        /// <param name="chunkSize">The chunk size.</param>
        /// <param name="id">The ID for this world.</param>
        internal World(GridSize chunkSize, string id)
        {
            ChunkSize = chunkSize;
            ID = id;
        }

        /// <summary>
        /// Gets whether or not the target chunk exists.
        /// </summary>
        /// <param name="pos">The position of the chunk.</param>
        /// <returns>True if the chunk exists, false otherwise.</returns>
        internal bool DoesChunkExist(ChunkPosition pos) => GetChunk(pos) != null;

        /// <summary>
        /// Gets the chunk at the given chunk position.
        /// </summary>
        /// <param name="pos">The position of the chunk.</param>
        /// <returns>The block container, or null if it doesn't exist.</returns>
        internal Chunk GetChunk(ChunkPosition pos)
        {
            for (int i = 0; i < m_Chunks.Count; i++)
            {
                if (m_Chunks[i].Position.Equals(pos))
                    return m_Chunks[i];
            }

            return null;
        }

        /// <summary>
        /// Creates a new chunk at the given chunk position if it doesn't currently exist.
        /// </summary>
        /// <param name="pos">The chunk position.</param>
        /// <returns>
        /// The newly created chunk, or the current chunk if if already exists.
        /// </returns>
        internal Chunk CreateChunk(ChunkPosition pos)
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
        internal void DestroyChunk(ChunkPosition pos)
        {
            var chunk = GetChunk(pos);
            if (chunk != null)
                m_Chunks.Remove(chunk);
        }

        /// <summary>
        /// Iterates over all chunks in this world.
        /// </summary>
        /// <returns>The chunk iterator.</returns>
        internal IEnumerable<Chunk> ChunkIterator()
        {
            foreach (var chunk in m_Chunks)
                yield return chunk;
        }
    }
}
