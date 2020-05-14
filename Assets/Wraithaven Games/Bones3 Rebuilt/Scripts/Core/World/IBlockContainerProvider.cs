namespace Bones3Rebuilt
{
    /// <summary>
    /// Contains a collection of block containers which can be retrieved later.
    /// </summary>
    public interface IBlockContainerProvider
    {
        /// <summary>
        /// Gets the size of each container in this provider.
        /// </summary>
        /// <value>The container size.</value>
        GridSize ContainerSize { get; }

        /// <summary>
        /// Called when a new block container is created.
        /// </summary>
        event BlockContainerCreatedCallback OnBlockContainerCreated;

        /// <summary>
        /// Called when a block container is destroyed.
        /// </summary>
        event BlockContainerDestroyedCallback OnBlockContainerDestroyed;

        /// <summary>
        /// Gets the block container at the given chunk coordinates.
        /// </summary>
        /// <param name="pos">The position of the block container.</param>
        /// <param name="create">Whether or not to create the container if it doesn't exist.</param>
        /// <returns>The block container, or null if it doesn't exist.</returns>
        IBlockContainer GetContainer(ChunkPosition pos, bool create);

        /// <summary>
        /// Destroys the block container at the given chunk coordinates if it exists.
        /// </summary>
        /// <param name="pos">The position of the block container.</param>
        void DestroyContainer(ChunkPosition pos);
    }
}
