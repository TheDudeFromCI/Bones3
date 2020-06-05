namespace WraithavenGames.Bones3
{
    /// <summary>
    /// A world task which retrieves the block ID at the given position.
    /// </summary>
    internal class GetBlockTask : IWorldTask
    {
        private readonly BlockPosition m_BlockPosition;
        private readonly bool m_CreateChunk;

        /// <summary>
        /// Gets the block ID at the given position.
        /// </summary>
        public ushort BlockID { get; private set; }

        /// <summary>
        /// Creates a new get-block task.
        /// </summary>
        /// <param name="blockPos">The block position.</param>
        /// <param name="create">Whether or not to create the chunk if non-existant.</param>
        internal GetBlockTask(BlockPosition blockPos, bool create)
        {
            m_BlockPosition = blockPos;
            m_CreateChunk = create;
        }

        /// <inheritdoc cref="IWorldTask"/>
        public void RunWorldTask(WorldContainer world)
        {
            BlockID = world.GetBlock(m_BlockPosition, m_CreateChunk);
        }
    }
}
