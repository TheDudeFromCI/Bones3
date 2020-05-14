namespace Bones3Rebuilt
{
    /// <summary>
    /// An event which is called when a block container is modified in any way.
    /// </summary>
    public class BlockContainerModifiedEvent
    {
        /// <summary>
        /// The block container being modified.
        /// </summary>
        /// <value>The block container.</value>
        public IBlockContainer BlockContainer { get; }

        /// <summary>
        /// The position of the block being modified.
        /// </summary>
        /// <value>The block position.</value>
        public BlockPosition BlockPos { get; }

        /// <summary>
        /// The old block index, before the event.
        /// </summary>
        /// <value>The block index before the event.</value>
        public ushort OldBlock { get => BlockContainer.GetBlockID(BlockPos); }

        /// <summary>
        /// The new block index which will be placed.
        /// </summary>
        /// <value>The new block index after the event.</value>
        public ushort NewBlock { get; }

        /// <summary>
        /// Creates a new event.
        /// </summary>
        /// <param name="container">The block container being edited.</param>
        /// <param name="blockPos">The block position.</param>
        /// <param name="newBlock">The new block index.</param>
        public BlockContainerModifiedEvent(Chunk container, BlockPosition blockPos, ushort newBlock)
        {
            BlockContainer = container;
            BlockPos = blockPos;
            NewBlock = newBlock;
        }
    }

    /// <summary>
    /// A callback for block container modified events.
    /// </summary>
    /// <param name="ev">The event.</param>
    public delegate void BlockContainerModifiedCallback(BlockContainerModifiedEvent ev);
}
