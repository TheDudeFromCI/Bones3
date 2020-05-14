using System.Collections.Generic;

namespace Bones3Rebuilt
{
    /// <summary>
    /// Represents a series of remesh tasks being executed for a chunk.
    /// </summary>
    public class RemeshTaskStack
    {
        private readonly List<IRemeshTask> m_CollisionRemesh = new List<IRemeshTask>();
        private readonly List<IRemeshTask> m_VisualRemesh = new List<IRemeshTask>();
        private readonly ChunkPosition m_ChunkPosition;

        /// <summary>
        /// Creates a new remesh task stack.
        /// </summary>
        /// <param name="chunkPosition">The position of the chunk being remeshed.</param>
        public RemeshTaskStack(ChunkPosition chunkPosition)
        {
            m_ChunkPosition = chunkPosition;
        }

        /// <summary>
        /// Waits for all tasks to finish and converts this remesh task stack into a remesh report.
        /// </summary>
        /// <returns>The remesh report.</returns>
        public RemeshReport ToReport()
        {
            var collisionMesh = CompressTasks(m_CollisionRemesh);
            var visualMesh = CompressTasks(m_VisualRemesh);

            return new RemeshReport(m_ChunkPosition, collisionMesh, visualMesh);
        }

        /// <summary>
        /// Compress a list of remesh tasks into a single layered proc mesh.
        /// </summary>
        /// <param name="tasks">The list of tasks.</param>
        /// <returns>The layered proc mesh.</returns>
        private LayeredProcMesh CompressTasks(List<IRemeshTask> tasks)
        {
            var mesh = new LayeredProcMesh();

            for (int i = 0; i < tasks.Count; i++)
            {
                tasks[i].Finish();
                mesh.GetLayer(i).AddData(tasks[i].Mesh);
            }

            return mesh;
        }

        /// <summary>
        /// Adds a collision remesh task to this stack.
        /// </summary>
        /// <param name="task">The task to add.</param>
        public void AddCollisionTask(IRemeshTask task)
        {
            m_CollisionRemesh.Add(task);
        }

        /// <summary>
        /// Adds a visual remesh task to this stack.
        /// </summary>
        /// <param name="task">The task to add.</param>
        public void AddVisualTask(IRemeshTask task)
        {
            m_VisualRemesh.Add(task);
        }
    }
}