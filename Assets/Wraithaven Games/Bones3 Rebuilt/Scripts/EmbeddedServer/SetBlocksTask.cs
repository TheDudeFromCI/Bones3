namespace WraithavenGames.Bones3
{
    /// <summary>
    /// A world task which assigns the block ID at the given position.
    /// </summary>
    internal class SetBlocksTask : IWorldTask
    {
        private readonly EditBatch m_EditBatch;

        /// <summary>
        /// Creates a new set-blocks task.
        /// </summary>
        /// <param name="editBatch">The iterator for blocks to place.</param>
        internal SetBlocksTask(EditBatch editBatch)
        {
            m_EditBatch = editBatch;
        }

        /// <inheritdoc cref="IWorldTask"/>
        public void RunWorldTask(WorldContainer world)
        {
            foreach (var block in m_EditBatch())
            {
                world.SetBlock(block.Position, block.BlockID);
            }

            world.RemeshDirtyChunks();
        }
    }
}
