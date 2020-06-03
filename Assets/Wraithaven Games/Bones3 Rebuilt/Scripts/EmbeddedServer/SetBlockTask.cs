namespace WraithavenGames.Bones3
{
    /// <summary>
    /// A world task which assigns the block ID at the given position.
    /// </summary>
    internal class SetBlockTask : IWorldTask
    {
        private readonly BlockPosition m_BlockPosition;
        private readonly ushort m_BlockID;

        /// <summary>
        /// Creates a new set-block task.
        /// </summary>
        /// <param name="blockPos">The block position.</param>
        /// <param name="blockId">The block to place.</param>
        internal SetBlockTask(BlockPosition blockPos, ushort blockId)
        {
            m_BlockPosition = blockPos;
            m_BlockID = blockId;
        }

        /// <inheritdoc cref="IWorldTask"/>
        public void FinishWorldTask()
        {
            // Nothing to do.
        }

        /// <inheritdoc cref="IWorldTask"/>
        public void RunWorldTask(WorldContainer world)
        {
            world.SetBlock(m_BlockPosition, m_BlockID);
        }
    }
}