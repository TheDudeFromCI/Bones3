namespace Bones3Rebuilt
{
    /// <summary>
    /// Called when a block container is marked for deletion.
    /// </summary>
    public class BlockContainerDestroyedEvent
    {
        /// <summary>
        /// Gets the block container to be deleted.
        /// </summary>
        /// <value>The block container.</value>
        public IBlockContainer BlockContainer { get; }

        /// <summary>
        /// Gets the block container provider which owns the container.
        /// </summary>
        /// <value>The container provider.</value>
        public IBlockContainerProvider ContainerProvider { get; }

        /// <summary>
        /// Creates a new event.
        /// </summary>
        /// <param name="container">The block container.</param>
        /// <param name="provider">The block container provider.</param>
        public BlockContainerDestroyedEvent(IBlockContainer container, IBlockContainerProvider provider)
        {
            BlockContainer = container;
            ContainerProvider = provider;
        }
    }

    /// <summary>
    /// A callback for block container destroyed events.
    /// </summary>
    /// <param name="ev">The event.</param>
    public delegate void BlockContainerDestroyedCallback(BlockContainerDestroyedEvent ev);
}
