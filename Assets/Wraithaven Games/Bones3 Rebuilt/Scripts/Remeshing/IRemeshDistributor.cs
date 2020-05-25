namespace Bones3Rebuilt.Remeshing
{
    /// <summary>
    /// Analyzes a chunk when it is queued and generates new remesh tasks.
    /// </summary>
    public interface IRemeshDistributor
    {
        /// <summary>
        /// Looks over a chunk and generates new remesh tasks as needed.
        /// </summary>
        /// <param name="properties">The chunk properties object.</param>
        /// <param name="taskStack">The list of tasks to add tasks to.</param>
        void CreateTasks(ChunkProperties properties, RemeshTaskStack taskStack);
    }
}
