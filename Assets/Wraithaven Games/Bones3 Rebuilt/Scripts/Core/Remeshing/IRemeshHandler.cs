namespace Bones3Rebuilt
{
    /// <summary>
    /// An object which is in charge of handling remesh operations on block containers.
    /// </summary>
    public interface IRemeshHandler
    {
        /// <summary>
        /// Called when a remesh task has finished.
        /// </summary>
        event RemeshFinishCallback OnRemeshFinish;

        /// <summary>
        /// Analyses the given chunk and starts a set of remesh tasks for handling that chunk.
        /// </summary>
        /// <param name="properties">The chunk properties to analyze.</param>
        void RemeshChunk(IChunkProperties properties);

        /// <summary>
        /// Waits for all current tasks to finish executing before continuing.
        /// </summary>
        void FinishTasks();
    }
}
