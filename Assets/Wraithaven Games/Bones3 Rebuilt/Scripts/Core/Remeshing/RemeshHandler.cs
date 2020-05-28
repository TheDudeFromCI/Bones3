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
        private readonly List<ChunkPosition> m_PendingRemesh = new List<ChunkPosition>();
        private readonly ChunkPropertiesPool m_ChunkPropertiesPool;
        private readonly BlockWorld m_BlockWorld;

        /// <summary>
        /// Gets the number of active tasks currently being run.
        /// </summary>
        internal int ActiveTasks => m_ActiveTasks.Count;

        internal RemeshHandler(BlockWorld blockWorld)
        {
            m_BlockWorld = blockWorld;
            m_ChunkPropertiesPool = new ChunkPropertiesPool(m_BlockWorld.ChunkSize);
        }

        /// <summary>
        /// Analyses the given chunk and starts a set of remesh tasks for handling that chunk.
        /// If the chunk was scheduled to be remeshed later, it will be immediately be remeshed.
        /// If the chunk is already being remeshed, this method does nothing.
        /// </summary>
        /// <param name="chunkPos">The chunk target position.</param>
        /// <param name="pendingTask">
        /// If true, this task is allowed to task any number of frames to finish. Otherwise, the task
        /// is required to finish the next frame.
        /// </param>
        internal void RemeshChunk(ChunkPosition chunkPos, bool pendingTask = false)
        {
            for (int i = 0; i < m_ActiveTasks.Count; i++)
            {
                if (m_ActiveTasks[i].ChunkPosition.Equals(chunkPos))
                {
                    m_ActiveTasks[i].IsPendingTask = false;
                    return;
                }
            }

            for (int i = 0; i < m_PendingRemesh.Count; i++)
            {
                if (m_PendingRemesh[i].Equals(chunkPos))
                {
                    m_PendingRemesh.RemoveAt(i);
                    break;
                }
            }

            var properties = m_ChunkPropertiesPool.Pull();
            var blockList = m_BlockWorld.BlockList;
            var world = m_BlockWorld.WorldContainer.World;
            ChunkAnalyzer.LoadProperties(properties, blockList, world, chunkPos);

            var taskStack = new RemeshTaskStack(properties);
            m_ActiveTasks.Add(taskStack);
            taskStack.IsPendingTask = pendingTask;

            foreach (var dis in m_Distributors)
                dis.CreateTasks(properties, taskStack);
        }

        /// <summary>
        /// Schedules the chunk to be remeshed at a later point in time, after all primary
        /// remesh tasks are finished.
        /// </summary>
        /// <param name="chunkPos">The target chunk position.</param>
        /// <see cref="RemeshChunk(ChunkProperties)"/>
        internal void RemeshChunkLater(ChunkPosition chunkPos)
        {
            foreach (var task in m_ActiveTasks)
                if (task.ChunkPosition.Equals(chunkPos))
                    return;

            foreach (var task in m_PendingRemesh)
                if (task.Equals(chunkPos))
                    return;

            m_PendingRemesh.Add(chunkPos);
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
                var properties = task.Finish();
                m_ChunkPropertiesPool.Return(properties);

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
