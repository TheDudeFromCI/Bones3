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
        private readonly List<ChunkProperties> m_PendingRemesh = new List<ChunkProperties>();

        /// <summary>
        /// Gets the number of active tasks currently being run.
        /// </summary>
        internal int ActiveTasks => m_ActiveTasks.Count;

        /// <summary>
        /// Analyses the given chunk and starts a set of remesh tasks for handling that chunk.
        /// If the chunk was scheduled to be remeshed later, it will be immediately be remeshed.
        /// If the chunk is already being remeshed, this method does nothing.
        /// </summary>
        /// <param name="properties">The chunk properties to analyze.</param>
        internal void RemeshChunk(ChunkProperties properties, bool pendingTask = false)
        {
            foreach (var task in m_ActiveTasks)
                if (task.ChunkPosition.Equals(properties.ChunkPosition))
                    return;

            for (int i = 0; i < m_PendingRemesh.Count; i++)
            {
                if (m_PendingRemesh[i].ChunkPosition.Equals(properties.ChunkPosition))
                {
                    m_PendingRemesh.RemoveAt(i);
                    break;
                }
            }

            var taskStack = new RemeshTaskStack(properties.ChunkPosition);
            m_ActiveTasks.Add(taskStack);

            if (pendingTask)
                taskStack.IsPendingTask = true;

            foreach (var dis in m_Distributors)
                dis.CreateTasks(properties, taskStack);
        }

        /// <summary>
        /// Schedules the chunk to be remeshed at a later point in time, after all primary
        /// remesh tasks are finished.
        /// </summary>
        /// <param name="properties">The chunk properties to analyze</param>
        /// <see cref="RemeshChunk(ChunkProperties)"/>
        internal void RemeshChunkLater(ChunkProperties properties)
        {
            foreach (var task in m_ActiveTasks)
                if (task.ChunkPosition.Equals(properties.ChunkPosition))
                    return;

            foreach (var task in m_PendingRemesh)
                if (task.ChunkPosition.Equals(properties.ChunkPosition))
                    return;

            m_PendingRemesh.Add(properties);
        }

        /// <summary>
        /// Waits for all current tasks to finish executing before continuing.
        /// </summary>
        /// <param name="finishedTasks">The list to write the finished task stacks to.</param>
        internal void FinishTasks(List<RemeshTaskStack> finishedTasks)
        {
            CheckPendingTasks();

            int skipped = 0;
            while (m_ActiveTasks.Count > skipped)
            {
                var task = m_ActiveTasks[skipped];
                if (task.IsPendingTask && !task.IsFinished)
                {
                    skipped++;
                    continue;
                }

                m_ActiveTasks.RemoveAt(skipped);

                task.Finish();
                finishedTasks.Add(task);
            }
        }

        /// <summary>
        /// Checks if any pending tasks are present which can be moved to an active task.
        /// </summary>
        private void CheckPendingTasks()
        {
            if (m_ActiveTasks.Count > 0)
                return;

            if (m_PendingRemesh.Count == 0)
                return;

            RemeshChunk(m_PendingRemesh[0], true);
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
