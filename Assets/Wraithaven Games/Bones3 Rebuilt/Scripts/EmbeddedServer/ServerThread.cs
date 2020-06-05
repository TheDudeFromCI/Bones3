using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// A dedicated thread for executing tasks on a block world object.
    /// </summary>
    internal class ServerThread
    {
        private readonly BlockingCollection<IWorldTask> m_TaskList = new BlockingCollection<IWorldTask>();
        private readonly BlockingCollection<IWorldTask> m_FinishedTasks = new BlockingCollection<IWorldTask>();
        private readonly WorldContainer m_WorldContainer;
        private volatile bool m_Running = true;
        private volatile int m_ActiveTasks = 0;

        /// <summary>
        /// Gets number of active tasks being run.
        /// </summary>
        internal int ActiveTasks => m_ActiveTasks;

        /// <summary>
        /// Creates a new server thread for the given world.
        /// </summary>
        /// <param name="world">The world.</param>
        internal ServerThread(WorldContainer world)
        {
            m_WorldContainer = world;
            Task.Run(Run);
        }

        /// <summary>
        /// Stops this server thread. All currently executing tasks are allowed to finish.
        /// The remaining tasks are executed on the main thread immediately. This method
        /// does nothing if the server thread has already been stopped.
        /// </summary>
        internal void Stop()
        {
            if (!m_Running)
                return;

            m_Running = false;
            m_TaskList.CompleteAdding();
            while (!m_FinishedTasks.IsAddingCompleted && m_FinishedTasks.TryTake(out _, -1)) ;
        }

        /// <summary>
        /// Runs the given task on the world thread.
        /// </summary>
        /// <param name="task">The task to run.</param>
        /// <exception cref="ObjectDisposedException">
        /// If the server thread has already been stopped.
        /// </exception>
        internal void RunTask(IWorldTask task)
        {
            if (!m_Running)
                throw new ObjectDisposedException("World thread already disposed!");

            Interlocked.Increment(ref m_ActiveTasks);
            m_TaskList.Add(task);
        }

        /// <summary>
        /// Runs a task on the world thread, but blocks the main thread until this
        /// task has finished executing. Any tasks which were scheduled before this
        /// task are completed first.
        /// </summary>
        internal void RunTaskSync(IWorldTask task)
        {
            RunTask(task);

            while (true)
            {
                if (!m_FinishedTasks.TryTake(out IWorldTask t, -1))
                    throw new ApplicationException("Failed to retreive task!");

                m_WorldContainer.EventQueue.RunEvents();
                Interlocked.Decrement(ref m_ActiveTasks);

                if (task == t)
                    break;
            }
        }

        /// <summary>
        /// Called each frame by the main thread to poll and cleanup finished tasks.
        /// This method does nothing if the server thread has already been stopped.
        /// </summary>
        internal void Update()
        {
            if (!m_Running)
                return;

            while (m_FinishedTasks.TryTake(out _))
                Interlocked.Decrement(ref m_ActiveTasks);

            m_WorldContainer.EventQueue.RunEvents();
        }

        /// <summary>
        /// The server thread logic.
        /// </summary>
        private void Run()
        {
            while (!m_TaskList.IsAddingCompleted)
            {
                if (!m_TaskList.TryTake(out IWorldTask task, -1))
                    break;

                task.RunWorldTask(m_WorldContainer);
                m_FinishedTasks.Add(task);
            }

            m_FinishedTasks.CompleteAdding();
        }
    }
}
