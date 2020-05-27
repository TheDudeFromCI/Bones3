namespace WraithavenGames.Bones3
{
    /// <summary>
    /// A handler for describing how chunks should be loaded.
    /// </summary>
    internal interface IChunkLoadHandler
    {
        /// <summary>
        /// Called when a new chunk is first created in memory.
        /// </summary>
        /// <param name="chunk">The chunk.</param>
        /// <returns>True if any blocks where changed on this chunk while loading.</returns>
        bool OnChunkLoad(Chunk chunk);
    }
}
