using System.Collections.Generic;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// Holds a set of remesh distrbutors in order to generate a new mesh for a chunk.
    /// </summary>
    internal class RemeshHandler
    {
        private readonly List<IRemeshDistributor> m_Distributors = new List<IRemeshDistributor>();
        private readonly List<RemeshTaskStack> m_ActiveTasks = new List<RemeshTaskStack>();

        /// <summary>
        /// Gets the number of active tasks currently being run.
        /// </summary>
        internal int ActiveTasks => m_ActiveTasks.Count;

        /// <summary>
        /// Analyses the given chunk and starts a set of remesh tasks for handling that chunk.
        /// </summary>
        /// <param name="properties">The chunk properties to analyze.</param>
        internal void RemeshChunk(ChunkProperties properties)
        {
            var taskStack = new RemeshTaskStack(properties.ChunkPosition);
            m_ActiveTasks.Add(taskStack);

            foreach (var dis in m_Distributors)
                dis.CreateTasks(properties, taskStack);
        }

        /// <summary>
        /// Waits for all current tasks to finish executing before continuing.
        /// </summary>
        /// <param name="finishedTasks">The list to write the finished task stacks to.</param>
        internal void FinishTasks(List<RemeshTaskStack> finishedTasks)
        {
            while (m_ActiveTasks.Count > 0)
            {
                var task = m_ActiveTasks[0];
                m_ActiveTasks.RemoveAt(0);

                task.Finish();
                finishedTasks.Add(task);
            }
        }

        /// <summary>
        /// Adds a remesh distributor to this handler.
        /// </summary>
        /// <param name="distributor">The distributor to add.</param>
        internal void AddDistributor(IRemeshDistributor distributor)
        {
            if (distributor == null)
                return;

            if (m_Distributors.Contains(distributor))
                return;

            m_Distributors.Add(distributor);
        }
    }
}
