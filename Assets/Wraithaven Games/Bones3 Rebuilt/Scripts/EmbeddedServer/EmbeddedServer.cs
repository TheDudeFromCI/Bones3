namespace WraithavenGames.Bones3
{
    /// <summary>
    /// The embedded server a dedicated thread for interacting with a block world.
    /// </summary>
    internal static class EmbeddedServer
    {
        /// <summary>
        /// Creates a new server thread from the given world properties.
        /// </summary>
        /// <param name="worldProperties">The world properties.</param>
        /// <returns>The server thread.</returns>
        internal static ServerThread Initialize(WorldProperties worldProperties)
        {
            var world = new World(worldProperties.ChunkSize, worldProperties.ID);
            var blockList = new ServerBlockList();
            var container = new WorldContainer(world, blockList);
            container.ChunkLoader.AddChunkLoadHandler(worldProperties.WorldGenerator);

            return new ServerThread(container);
        }
    }
}
