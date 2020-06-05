namespace WraithavenGames.Bones3
{
    /// <summary>
    /// Triggers the world to remesh all modified chunks.
    /// </summary>
    internal class RemeshChunkTask : IWorldTask
    {
        /// <inheritdoc cref="IWorldTask" />
        public void RunWorldTask(WorldContainer world)
        {
            world.RemeshDirtyChunks();
        }
    }
}
