namespace WraithavenGames.Bones3
{
    /// <summary>
    /// A task which was executed to generate a single mesh layer of a chunk
    /// based on the given properties of that chunk.
    /// </summary>
    public interface IRemeshTask
    {
        /// <summary>
        /// Gets whether or not this task is finished.
        /// </summary>
        bool IsFinished { get; }

        /// <summary>
        /// Waits for this task to finish executing before continuing. If task is
        /// already finished, this method simply returns the generated mesh.
        /// </summary>
        /// <returns>The generated mesh.</returns>
        ProcMesh Finish();
    }
}
