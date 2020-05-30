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
        /// <param name="world">The world being accessed.</param>
        void RunWorldTask(World world);

        /// <summary>
        /// Called after the task has finish from the main thread to clean up.
        /// </summary>
        void FinishWorldTask();

        /// <summary>
        /// If this task only reads the world and does not edit it, then a temporary
        /// thread is created to run this task in allowing multiple tasks to read
        /// the world at the same time without blocking.
        /// </summary>
        /// <returns>Whether or not this task modifies the world in any way.</returns>
        bool IsReadOnly();
    }
}