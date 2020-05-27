using System.Threading.Tasks;
using System.Collections.Generic;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// A handler for loading chunks sync and async.
    /// </summary>
    internal class AsyncChunkLoader
    {
        private readonly List<BackgroundLoadChunkTask> operations = new List<BackgroundLoadChunkTask>();
        private readonly List<IChunkLoadHandler> m_ChunkLoadHandlers = new List<IChunkLoadHandler>();

        /// <summary>
        /// Triggers a chunk to load in a background task. The chunk should *not* be accessed in
        /// any way until this task is finished. The task can be force finished by calling
        /// `Finish(chunk);`
        /// 
        /// This method does nothing if the chunk is already being handled.
        /// </summary>
        /// <param name="chunk">The chunk to load.</param>
        internal void LoadAsync(Chunk chunk)
        {
            if (GetTask(chunk) != null)
                return;

            operations.Add(new BackgroundLoadChunkTask(chunk, m_ChunkLoadHandlers.ToArray()));
        }

        /// <summary>
        /// Loads the given chunk in the current thread.
        /// </summary>
        /// <param name="chunk">The chunk to load.</param>
        /// <returns>Whether or not the chunk needs to be remeshed.</returns>
        internal bool LoadSync(Chunk chunk)
        {
            var op = new BackgroundLoadChunkTask(chunk, m_ChunkLoadHandlers.ToArray());
            op.Finish();

            return op.RequiresRemesh;
        }

        /// <summary>
        /// Blocks the current thread until the given chunk has finished loading.
        /// This method returns immediately if the task is already finished or was
        /// not being loaded.
        /// </summary>
        /// <param name="chunk">The chunk to wait for.</param>
        /// <returns>Whether or not the chunk needs to be remeshed.</returns>
        internal bool Finish(Chunk chunk)
        {
            return FinishTask(GetTask(chunk));
        }

        /// <summary>
        /// Gets the task which is handling the given chunk.
        /// </summary>
        /// <param name="chunk">The chunk.</param>
        /// <returns>The task.</returns>
        private BackgroundLoadChunkTask GetTask(Chunk chunk)
        {
            foreach (var op in operations)
                if (op.Chunk == chunk)
                    return op;

            return null;
        }

        /// <summary>
        /// Called each frame to retreive finished tasks for remeshing.
        /// </summary>
        /// <param name="chunk">
        /// The chunk which finished loading this frame, or null.
        /// </param>
        /// <returns>Whether or not the chunk (if not null) needs to be remeshed.</returns>
        internal bool Update(out Chunk chunk)
        {
            var op = GetFinishedTask();
            FinishTask(op);

            chunk = op?.Chunk;
            return op.RequiresRemesh;
        }

        /// <summary>
        /// Waits for the given task to finish and removes it from the list.
        /// </summary>
        /// <param name="op">The task to finish</param>
        /// <returns>Whether or not the chunk needs to be remeshed.</returns>
        private bool FinishTask(BackgroundLoadChunkTask op)
        {
            if (op == null)
                return false;

            operations.Remove(op);
            op.Finish();

            return op.RequiresRemesh;
        }

        /// <summary>
        /// Retrieves a finished task from the list if one exists.
        /// </summary>
        /// <returns>A random finished task, or null if there isn't one.</returns>
        private BackgroundLoadChunkTask GetFinishedTask()
        {
            foreach (var op in operations)
                if (op.IsFinished)
                    return op;

            return null;
        }

        /// <summary>
        /// Adds a new chunk load handler to this world. Handlers are executed on
        /// chunks in the order they are added to this loaded.
        /// </summary>
        /// <param name="loadHandler">The handler.</param>
        internal void AddChunkLoadHandler(IChunkLoadHandler loadHandler) => m_ChunkLoadHandlers.Add(loadHandler);
    }

    /// <summary>
    /// Represents a background chunk loading task.
    /// </summary>
    internal class BackgroundLoadChunkTask
    {
        private readonly Task m_Task;
        private readonly IChunkLoadHandler[] m_Handlers;

        /// <summary>
        /// Gets the chunk being loaded.
        /// </summary>
        /// <value>The chunk.</value>
        public Chunk Chunk { get; }

        /// <summary>
        /// Gets whether or not the chunk has finished loading.
        /// </summary>
        /// <value>True if the task is finished.</value>
        public bool IsFinished => m_Task.IsCompleted;

        /// <summary>
        /// Gets whether or not the chunk needs to be remeshed.
        /// </summary>
        /// <value>True if the chunk was modified.</value>
        public bool RequiresRemesh { get; private set; } = false;

        /// <summary>
        /// Creates and starts a new background chunk loading task.
        /// </summary>
        /// <param name="chunk">The chunk to load.</param>
        /// <param name="handlers">The array of handlers to execute.</param>
        internal BackgroundLoadChunkTask(Chunk chunk, IChunkLoadHandler[] handlers)
        {
            Chunk = chunk;
            m_Handlers = handlers;

            m_Task = Task.Run(Run);
        }

        /// <summary>
        /// Runs all handlers on the chunk in order.
        /// </summary>
        private void Run()
        {
            foreach (var handler in m_Handlers)
                RequiresRemesh |= handler.OnChunkLoad(Chunk, RequiresRemesh);
        }

        /// <summary>
        /// Waits for the task to finished.
        /// </summary>
        internal void Finish()
        {
            m_Task.Wait();
        }
    }
}