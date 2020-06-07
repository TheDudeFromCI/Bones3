namespace WraithavenGames.Bones3
{
    /// <summary>
    /// Triggers the world to save all modified chunks.
    /// </summary>
    internal class SaveWorldTask : IWorldTask
    {
        /// <inheritdoc cref="IWorldTask" />
        public void RunWorldTask(WorldContainer world)
        {
            world.SaveWorld();
        }
    }
}
