using System.Collections.Generic;

namespace Bones3Rebuilt
{
    /// <summary>
    /// A wrapper object for handling a world object, remesh handler, and file IO.
    /// </summary>
    public class WorldContainer
    {
        private readonly List<ChunkPosition> m_ToRemesh = new List<ChunkPosition>();

        /// <summary>
        /// Gets the block container provider for this world container.
        /// </summary>
        /// <value>The block container provider.</value>
        public IBlockContainerProvider BlockContainerProvider { get; }

        /// <summary>
        /// Gets the remesh handler for this world container.
        /// </summary>
        /// <value>The remesh handler.</value>
        public IRemeshHandler RemeshHandler { get; }

        /// <summary>
        /// Gets the block list for this world container.
        /// </summary>
        /// <value>The block list.</value>
        public IBlockTypeList BlockList { get; }

        /// <summary>
        /// Gets the world database file manager for this container.
        /// </summary>
        /// <value>The world database.</value>
        public IWorldDatabase Database { get; }

        /// <summary>
        /// Creates a new world container.
        /// </summary>
        /// <param name="containerProvider">The block container provider.</param>
        /// <param name="remeshHandler">The remesh handler.</param>
        /// <param name="blockList">The block list.</param>
        /// <param name="database">The world database.</param>
        public WorldContainer(IBlockContainerProvider containerProvider, IRemeshHandler remeshHandler, IBlockTypeList blockList, IWorldDatabase database)
        {
            BlockContainerProvider = containerProvider;
            RemeshHandler = remeshHandler;
            BlockList = blockList;
            Database = database;
        }

        /// <summary>
        /// Gets the block type at the specified location in the world.
        /// </summary>
        /// <param name="pos">The position of the block.</param>
        /// <param name="create">
        /// Whether or not to create (or load) the chunk if it doesn't currently
        /// exist in memory.
        /// </param>
        /// <returns>The block type at the given position.</returns>
        public BlockType GetBlock(BlockPosition pos, bool create)
        {
            var chunkPos = pos.ToChunkPosition(BlockContainerProvider.ContainerSize);
            var blockPos = pos & BlockContainerProvider.ContainerSize.Mask;

            var container = BlockContainerProvider.GetContainer(chunkPos, create);
            if (container == null)
                return BlockList.GetBlockType(0);

            var blockId = container.GetBlockID(blockPos);
            return BlockList.GetBlockType(blockId);
        }

        /// <summary>
        /// Sets a block in the world to a given type.
        /// </summary>
        /// <param name="pos">The position of the block.</param>
        /// <param name="block">The block type to assign.</param>
        public void SetBlock(BlockPosition pos, BlockType block)
        {
            SetBlockInternal(pos, block.ID);
            RemeshQueued();
        }

        /// <summary>
        /// Sets a group of blocks in the world at once. More efficient than multiple
        /// calls to SetBlock(...).
        /// </summary>
        /// <param name="editBatch">The edit batch to iterator over.</param>
        public void SetBlocks(IEditBatch editBatch)
        {
            foreach (var block in editBatch.GetBlocks())
                SetBlockInternal(block.Position, block.BlockID);

            RemeshQueued();
        }

        /// <summary>
        /// Sets a block at the given position and queues chunks to remesh as needed.
        /// </summary>
        /// <param name="pos">The position of the block.</param>
        /// <param name="blockId">The block ID to assign.</param>
        private void SetBlockInternal(BlockPosition pos, ushort blockId)
        {
            var chunkPos = pos.ToChunkPosition(BlockContainerProvider.ContainerSize);
            var blockPos = pos & BlockContainerProvider.ContainerSize.Mask;

            var container = BlockContainerProvider.GetContainer(chunkPos, true);
            ushort oldBlock = container.GetBlockID(blockPos);

            if (oldBlock == blockId)
                return;

            container.SetBlockID(blockPos, blockId);
            RemeshChunkAt(chunkPos);

            if (blockPos.X == BlockContainerProvider.ContainerSize.Mask)
                RemeshChunkAt(chunkPos.ShiftAlongDirection(0));
            else if (blockPos.X == 0)
                RemeshChunkAt(chunkPos.ShiftAlongDirection(1));

            if (blockPos.Y == BlockContainerProvider.ContainerSize.Mask)
                RemeshChunkAt(chunkPos.ShiftAlongDirection(2));
            else if (blockPos.Y == 0)
                RemeshChunkAt(chunkPos.ShiftAlongDirection(3));

            if (blockPos.Z == BlockContainerProvider.ContainerSize.Mask)
                RemeshChunkAt(chunkPos.ShiftAlongDirection(4));
            else if (blockPos.Z == 0)
                RemeshChunkAt(chunkPos.ShiftAlongDirection(5));
        }

        /// <summary>
        /// Marks the chunk at the given chunk position to be remeshed.
        /// </summary>
        /// <param name="pos">The position of the chunk.</param>
        private void RemeshChunkAt(ChunkPosition pos)
        {
            if (!m_ToRemesh.Contains(pos))
                m_ToRemesh.Add(pos);
        }

        /// <summary>
        /// Sends all queued chunks to the remesh handler.
        /// </summary>
        private void RemeshQueued()
        {
            foreach (var chunk in m_ToRemesh)
            {
                if (BlockContainerProvider.GetContainer(chunk, false) == null)
                    continue;

                var props = new ChunkProperties(BlockContainerProvider, chunk, BlockList);
                RemeshHandler.RemeshChunk(props);
            };

            m_ToRemesh.Clear();
        }
    }
}
