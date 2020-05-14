namespace Bones3Rebuilt
{
    /// <summary>
    /// An event which is called when a chunk is first created.
    /// </summary>
    public class BlockContainerCreatedEvent
    {
        /// <summary>
        /// Gets the block container which was created.
        /// </summary>
        /// <value>The newly created block container.</value>
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
        public BlockContainerCreatedEvent(IBlockContainer container, IBlockContainerProvider provider)
        {
            BlockContainer = container;
            ContainerProvider = provider;
        }
    }

    /// <summary>
    /// A callback for block container creation events.
    /// </summary>
    /// <param name="ev">The event.</param>
    public delegate void BlockContainerCreatedCallback(BlockContainerCreatedEvent ev);
}
