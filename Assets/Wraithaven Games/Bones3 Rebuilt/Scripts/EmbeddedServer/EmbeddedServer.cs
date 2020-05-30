namespace WraithavenGames.Bones3
{
    /// <summary>
    /// The embedded server a dedicated thread for interacting with a block world.
    /// </summary>
    internal class EmbeddedServer
    {
        private readonly ServerThread m_Server;

        /// <summary>
        /// Creates and starts a new embedded server to the given world properties.
        /// </summary>
        /// <param name="worldProperties">The world properties.</param>
        internal EmbeddedServer(WorldProperties worldProperties)
        {
            var world = new World(worldProperties.ChunkSize, worldProperties.ID);
            m_Server = new ServerThread(world);
        }

        /// <summary>
        /// See <see cref="ServerThread.Update()"/>
        /// </summary>
        internal void Update() => m_Server.Update();

        /// <summary>
        /// See <see cref="ServerThread.Stop()"/>
        /// </summary>
        internal void Stop() => m_Server.Stop();

        /// <summary>
        /// See <see cref="ServerThread.RunTask(IWorldTask)"/>
        /// </summary>
        internal void RunTask(IWorldTask task) => m_Server.RunTask(task);

        /// <summary>
        /// See <see cref="ServerThread.RunTaskSync(IWorldTask)"/>
        /// </summary>
        internal void RunTaskSync(IWorldTask task) => m_Server.RunTaskSync(task);
    }
}
