using System.Collections.Generic;

namespace Bones3Rebuilt
{
    /// <summary>
    /// Holds a set of remesh distrbutors in order to generate a new mesh for a chunk.
    /// </summary>
    public class RemeshHandler : IRemeshHandler
    {
        private readonly List<IRemeshDistributor> m_Distributors = new List<IRemeshDistributor>();
        private readonly List<RemeshTaskStack> m_ActiveTasks = new List<RemeshTaskStack>();

        /// <inheritdoc cref="IRemeshHandler"/>
        public event RemeshFinishCallback OnRemeshFinish;

        /// <inheritdoc cref="IRemeshHandler"/>
        public void RemeshChunk(IChunkProperties properties)
        {
            var taskStack = new RemeshTaskStack(properties.ChunkPosition);
            m_ActiveTasks.Add(taskStack);

            foreach (var dis in m_Distributors)
                dis.CreateTasks(properties, taskStack);
        }

        /// <inheritdoc cref="IRemeshHandler"/>
        public void FinishTasks()
        {
            while (m_ActiveTasks.Count > 0)
            {
                var task = m_ActiveTasks[0];
                m_ActiveTasks.RemoveAt(0);

                var report = task.ToReport();

                OnRemeshFinish?.Invoke(new RemeshFinishEvent(report));
            }
        }

        /// <summary>
        /// Adds a remesh distributor to this handler.
        /// </summary>
        /// <param name="distributor">The distributor to add.</param>
        public void AddDistributor(IRemeshDistributor distributor)
        {
            if (distributor == null)
                return;

            if (m_Distributors.Contains(distributor))
                return;

            m_Distributors.Add(distributor);
        }

        /// <summary>
        /// Removes a remesh distributor from this handler.
        /// </summary>
        /// <param name="distributor">The distributor to remove.</param>
        public void RemoveDistributor(IRemeshDistributor distributor)
        {
            if (distributor == null)
                return;

            m_Distributors.Remove(distributor);
        }
    }
}
