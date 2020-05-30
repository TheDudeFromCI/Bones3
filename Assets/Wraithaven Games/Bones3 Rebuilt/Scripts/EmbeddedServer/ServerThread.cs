using System.Collections.Concurrent;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// A dedicated thread for executing tasks on a block world object.
    /// </summary>
    internal class ServerThread
    {
        private readonly BlockingCollection<IWorldTask> m_TaskList = new BlockingCollection<IWorldTask>();
        private readonly BlockingCollection<IWorldTask> m_FinishedTasks = new BlockingCollection<IWorldTask>();
        private readonly World m_World;
        private volatile bool m_Running = true;

        /// <summary>
        /// Creates a new server thread for the given world.
        /// </summary>
        /// <param name="world">The world.</param>
        internal ServerThread(World world)
        {
            m_World = world;
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

            while (m_FinishedTasks.TryTake(out IWorldTask task, -1))
                task.FinishWorldTask();
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

                t.FinishWorldTask();

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

            if (m_FinishedTasks.TryTake(out IWorldTask task))
                task.FinishWorldTask();
        }

        /// <summary>
        /// The server thread logic.
        /// </summary>
        private void Run()
        {
            while (m_Running && !m_TaskList.IsCompleted)
            {
                if (!m_TaskList.TryTake(out IWorldTask task, -1))
                    break;

                task.RunWorldTask(m_World);
                m_FinishedTasks.Add(task);
            }

            m_FinishedTasks.CompleteAdding();
        }
    }
}