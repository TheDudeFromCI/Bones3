using System.Collections.Generic;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// Holds a set of remesh distrbutors in order to generate a new mesh for a chunk.
    /// </summary>
    internal class RemeshHandler
    {
        private readonly List<IRemeshDistributor> m_Distributors = new List<IRemeshDistributor>();

        /// <summary>
        /// Analyses the given chunk and starts a set of remesh tasks for handling that chunk.
        /// This method blocks until all remesh tasks have been completed.
        /// </summary>
        /// <param name="worldContainer">The world to operate on.</param>
        /// <param name="chunkPos">The chunk target position.</param>
        internal void RemeshChunk(WorldContainer worldContainer, ChunkPosition chunkPos)
        {
            var taskStack = new RemeshTaskStack(chunkPos);
            var chunkGroup = new ChunkGroup(worldContainer, chunkPos);

            foreach (var dis in m_Distributors)
                dis.CreateTasks(chunkGroup, taskStack);

            taskStack.Finish();
            worldContainer.AddEvent(new ChunkRemeshEvent(taskStack));
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
