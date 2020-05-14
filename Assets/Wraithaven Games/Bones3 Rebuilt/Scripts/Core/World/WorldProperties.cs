namespace Bones3Rebuilt
{
    /// <summary>
    /// A series of properties which are used to define a voxel world.
    /// </summary>
    public struct WorldProperties
    {
        /// <summary>
        /// Gets the size of chunks in this world.
        /// </summary>
        /// <value>The chunk size.</value>
        public GridSize ChunkSize { get; set; }

        /// <summary>
        /// Gets the name of this world.
        /// </summary>
        /// <value>The world name.</value>
        public string WorldName { get; set; }
    }
}
