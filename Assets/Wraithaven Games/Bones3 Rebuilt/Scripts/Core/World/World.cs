using System;
using System.Collections.Generic;

namespace Bones3Rebuilt
{
    /// <summary>
    /// A container for the storage of all world data involved a voxel world representation.
    /// </summary>
    public class World : IBlockContainerProvider
    {
        private readonly List<IBlockContainer> m_Chunks = new List<IBlockContainer>();

        /// <summary>
        /// The number of blocks in a chunk along a single axis.
        /// </summary>
        /// <value>The size of a chunk.</value>
        public GridSize ContainerSize { get; }

        /// <inheritdoc cref="IBlockContainerProvider"/>
        public event BlockContainerCreatedCallback OnBlockContainerCreated;

        /// <inheritdoc cref="IBlockContainerProvider"/>
        public event BlockContainerDestroyedCallback OnBlockContainerDestroyed;

        /// <summary>
        /// Creates a new world object.
        /// </summary>
        /// <param name="chunkSize">The chunk size.</param>
        public World(GridSize chunkSize)
        {
            ContainerSize = chunkSize;
        }

        /// <inheritdoc cref="IBlockContainerProvider"/>
        public IBlockContainer GetContainer(ChunkPosition pos, bool create)
        {
            foreach (var chunk in m_Chunks)
                if (chunk.Position.Equals(pos))
                    return chunk;

            if (!create)
                return null;

            var newChunk = new Chunk(ContainerSize, pos);
            m_Chunks.Add(newChunk);

            OnBlockContainerCreated?.Invoke(new BlockContainerCreatedEvent(newChunk, this));

            return newChunk;
        }

        /// <inheritdoc cref="IBlockContainerProvider"/>
        public void DestroyContainer(ChunkPosition pos)
        {
            var chunk = GetContainer(pos, false);
            if (chunk == null)
                return;

            OnBlockContainerDestroyed?.Invoke(new BlockContainerDestroyedEvent(chunk, this));
            m_Chunks.Remove(chunk);
        }
    }
}
