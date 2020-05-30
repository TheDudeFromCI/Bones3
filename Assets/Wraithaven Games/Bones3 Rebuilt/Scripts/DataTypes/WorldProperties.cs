namespace WraithavenGames.Bones3
{
    /// <summary>
    /// A collection of properties to use when creating a world object.
    /// </summary>
    public struct WorldProperties
    {
        /// <summary>
        /// The name of the world.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The ID of the world.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// The size of chunks of the world.
        /// </summary>
        public GridSize ChunkSize { get; set; }
    }
}