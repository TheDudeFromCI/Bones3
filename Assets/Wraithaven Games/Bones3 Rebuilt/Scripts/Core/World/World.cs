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
        private readonly List<IChunkLoadHandler> m_ChunkLoadHandlers = new List<IChunkLoadHandler>();

        /// <summary>
        /// The number of blocks in a chunk along a single axis.
        /// </summary>
        /// <value>The size of a chunk.</value>
        internal GridSize ChunkSize { get; }

        /// <summary>
        /// Gets this world's GUID value.
        /// </summary>
        /// <value>The world ID.</value>
        internal Guid ID { get; set; }

        /// <summary>
        /// Creates a new world object.
        /// </summary>
        /// <param name="chunkSize">The chunk size.</param>
        internal World(GridSize chunkSize) => ChunkSize = chunkSize;

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
            foreach (var chunk in m_Chunks)
                if (chunk.Position.Equals(pos))
                    return chunk;

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

            foreach (var handler in m_ChunkLoadHandlers)
                handler.OnChunkLoad(newChunk);

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
        /// Adds a new chunk load handler to this world.
        /// </summary>
        /// <param name="loadHandler">The handler.</param>
        internal void AddChunkLoadHandler(IChunkLoadHandler loadHandler) => m_ChunkLoadHandlers.Add(loadHandler);

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
