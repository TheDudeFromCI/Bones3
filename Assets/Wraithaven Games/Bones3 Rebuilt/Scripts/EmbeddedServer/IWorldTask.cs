namespace WraithavenGames.Bones3
{
    /// <summary>
    /// An operation which is to be preformed on a world from the world thread.
    /// </summary>
    internal interface IWorldTask
    {
        /// <summary>
        /// Runs this task on a world in the world thread.
        /// </summary>
        /// <param name="world">The world data being accessed.</param>
        void RunWorldTask(WorldContainer world);
    }
}
