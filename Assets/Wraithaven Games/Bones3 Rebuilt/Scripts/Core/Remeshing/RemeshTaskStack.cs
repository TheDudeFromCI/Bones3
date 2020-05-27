using System.Collections.Generic;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// Represents a series of remesh tasks being executed for a chunk.
    /// </summary>
    public class RemeshTaskStack
    {
        private readonly List<IRemeshTask> m_Tasks = new List<IRemeshTask>();

        /// <summary>
        /// Gets the position of the chunk this task stack is targeting.
        /// </summary>
        /// <value>The chunk position.</value>
        public ChunkPosition ChunkPosition { get; }

        /// <summary>
        /// Gets the number of tasks in this task stack.
        /// </summary>
        public int TaskCount => m_Tasks.Count;

        /// <summary>
        /// Gets whether or not this task was marked as a pending stack. A pending
        /// stack is allowed to take as long as needed to finish.
        /// </summary>
        internal bool IsPendingTask { get; set; } = false;

        /// <summary>
        /// Gets whether or not this task stack has finished.
        /// </summary>
        internal bool IsFinished
        {
            get
            {
                foreach (var task in m_Tasks)
                    if (!task.IsFinished)
                        return false;

                return true;
            }
        }

        /// <summary>
        /// Creates a new remesh task stack.
        /// </summary>
        /// <param name="chunkPosition">The position of the chunk being remeshed.</param>
        internal RemeshTaskStack(ChunkPosition chunkPosition) => ChunkPosition = chunkPosition;

        /// <summary>
        /// Adds a remesh task to this stack.
        /// </summary>
        /// <param name="task">The task to add.</param>
        public void AddTask(IRemeshTask task) => m_Tasks.Add(task);

        /// <summary>
        /// Gets a remesh task from this stack.
        /// </summary>
        /// <param name="index">The index of the stack.</param>
        /// <returns>The task.</returns>
        public IRemeshTask GetTask(int index) => m_Tasks[index];

        /// <summary>
        /// Waits for all tasks to finish before returning.
        /// </summary>
        internal void Finish()
        {
            foreach (var task in m_Tasks)
                task.Finish();
        }
    }
}
