namespace WraithavenGames.Bones3
{
    /// <summary>
    /// Called when a chunk has been remeshed.
    /// </summary>
    public class ChunkRemeshEvent : IBlockWorldEvent
    {
        /// <summary>
        /// Gets the remesh task stack which contains information about the remesh.
        /// </summary>
        public RemeshTaskStack Task { get; }

        /// <summary>
        /// Creates a new chunk remesh event.
        /// </summary>
        /// <param name="task">The task information.</param>
        internal ChunkRemeshEvent(RemeshTaskStack task) => Task = task;
    }
}
