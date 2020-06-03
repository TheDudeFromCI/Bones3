namespace WraithavenGames.Bones3
{
    /// <summary>
    /// A world task which assigns the block ID at the given position.
    /// </summary>
    internal class LoadChunkTask : IWorldTask
    {
        private readonly ChunkPosition m_ChunkPosition;

        /// <summary>
        /// True if the chunk was just loaded, or false if the chunk was already loaded.
        /// </summary>
        public bool WasJustLoaded { get; private set; }

        /// <summary>
        /// Creates a new load-chunk task.
        /// </summary>
        /// <param name="chunkPos">The chunk position to load.</param>
        internal LoadChunkTask(ChunkPosition chunkPos)
        {
            m_ChunkPosition = chunkPos;
        }

        /// <inheritdoc cref="IWorldTask"/>
        public void FinishWorldTask()
        {
            // Nothing to do.
        }

        /// <inheritdoc cref="IWorldTask"/>
        public void RunWorldTask(WorldContainer world)
        {
            if (world.DoesChunkExist(m_ChunkPosition))
                return;

            world.LoadChunk(m_ChunkPosition);
            WasJustLoaded = true;
        }
    }
}