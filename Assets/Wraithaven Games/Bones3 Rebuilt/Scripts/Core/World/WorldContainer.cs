using System;
using System.Collections.Generic;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// A container which wraps the World API and RemeshHandler API.
    /// </summary>
    internal class WorldContainer
    {
        private readonly List<Chunk> chunksToRemesh = new List<Chunk>();
        private readonly List<RemeshTaskStack> taskStacks = new List<RemeshTaskStack>();
        private readonly World m_World;
        private readonly RemeshHandler m_RemeshHandler;
        private readonly BlockWorld m_BlockWorld;
        private readonly AsyncChunkLoader m_ChunkLoader;

        /// <summary>
        /// Gets the number of chunks currently being loaded in the background.
        /// </summary>
        internal int ChunksBeingLoaded => m_ChunkLoader.ActiveTasks;

        /// <summary>
        /// Creates a new world container for the given world.
        /// </summary>
        /// <param name="world">The world.</param>
        internal WorldContainer(World world, BlockWorld blockWorld)
        {
            m_World = world;
            m_BlockWorld = blockWorld;

            m_ChunkLoader = new AsyncChunkLoader();
            m_ChunkLoader.AddChunkLoadHandler(new WorldLoader(world));

            m_RemeshHandler = new RemeshHandler();
            m_RemeshHandler.AddDistributor(new StandardDistributor());
        }

        /// <summary>
        /// Sets the block at the given position.
        /// </summary>
        /// <param name="blockPos">The block position.</param>
        /// <param name="blockID">The block ID to assign.</param>
        internal void SetBlock(BlockPosition blockPos, ushort blockID)
        {
            var chunkPos = blockPos.ToChunkPosition(m_World.ChunkSize);
            blockPos &= m_World.ChunkSize.Mask;

            var chunk = GetChunk(chunkPos, true);
            if (chunk.GetBlockID(blockPos) == blockID)
                return;

            chunk.SetBlockID(blockPos, blockID);
            RemeshEffectedChunks(blockPos, chunkPos);
        }

        /// <summary>
        /// Gets the block type at the given world position.
        /// 
        /// For ungenerated or unloaded chunks, the Ungenerated block type is return.
        /// </summary>
        /// <param name="blockPosition">The position of the block.</param>
        /// <param name="createChunk">Whether or not to create (or load) the chunk if it doesn't currently exist.</param>
        /// <returns>The block type.</returns>
        internal ushort GetBlock(BlockPosition blockPos, bool createChunk = false)
        {
            var chunkPos = blockPos.ToChunkPosition(m_World.ChunkSize);
            blockPos &= m_World.ChunkSize.Mask;

            return GetChunk(chunkPos, createChunk)?.GetBlockID(blockPos) ?? 0;
        }

        /// <summary>
        /// Gets the chunk at the target chunk position.
        /// </summary>
        /// <param name="chunkPos">The chunk position.</param>
        /// <param name="create">Whether or not to create the chunk if it doesn't currently exist.</param>
        /// <returns>The chunk, or null if it doesn't exist.</returns>
        private Chunk GetChunk(ChunkPosition chunkPos, bool create)
        {
            var chunk = m_World.GetChunk(chunkPos);
            if (chunk != null || !create)
                return chunk;

            chunk = m_World.CreateChunk(chunkPos);
            bool remesh = m_ChunkLoader.LoadSync(chunk);

            if (remesh)
            {
                RemeshAllNeighbors(chunkPos);
                RemeshDirtyChunks();
            }

            return chunk;
        }

        /// <summary>
        /// Remeshes the chunk at the given position and all chunks touching it.
        /// </summary>
        /// <param name="chunkPos">The chunk position.</param>
        private void RemeshAllNeighbors(ChunkPosition chunkPos)
        {
            RemeshChunkAt(chunkPos);
            RemeshChunkAt(chunkPos.ShiftAlongDirection(0));
            RemeshChunkAt(chunkPos.ShiftAlongDirection(1));
            RemeshChunkAt(chunkPos.ShiftAlongDirection(2));
            RemeshChunkAt(chunkPos.ShiftAlongDirection(3));
            RemeshChunkAt(chunkPos.ShiftAlongDirection(4));
            RemeshChunkAt(chunkPos.ShiftAlongDirection(5));
        }

        /// <summary>
        /// Determines which chunks should be remeshed based on the given block position.
        /// </summary>
        /// <param name="blockPos">The local block position.</param>
        /// <param name="toRemesh">The chunk position.</param>
        private void RemeshEffectedChunks(BlockPosition blockPos, ChunkPosition chunkPos)
        {
            RemeshChunkAt(chunkPos);

            int max = m_World.ChunkSize.Mask;

            if (blockPos.X == 0)
                RemeshChunkAt(chunkPos.ShiftAlongDirection(1));

            if (blockPos.X == max)
                RemeshChunkAt(chunkPos.ShiftAlongDirection(0));

            if (blockPos.Y == 0)
                RemeshChunkAt(chunkPos.ShiftAlongDirection(3));

            if (blockPos.Y == max)
                RemeshChunkAt(chunkPos.ShiftAlongDirection(2));

            if (blockPos.Z == 0)
                RemeshChunkAt(chunkPos.ShiftAlongDirection(5));

            if (blockPos.Z == max)
                RemeshChunkAt(chunkPos.ShiftAlongDirection(4));
        }

        /// <summary>
        /// Triggers the given at the given position to be remeshed, if not already in the list.
        /// Preforms no action if the chunk does not exist.
        /// </summary>
        /// <param name="chunkPos">The chunk position.</param>
        private void RemeshChunkAt(ChunkPosition chunkPos)
        {
            var chunk = m_World.GetChunk(chunkPos);
            if (chunk == null)
                return;

            if (!chunksToRemesh.Contains(chunk))
                chunksToRemesh.Add(chunk);
        }

        /// <summary>
        /// Triggers all dirty chunks to be remeshed.
        /// </summary>
        internal void RemeshDirtyChunks()
        {
            foreach (var chunk in chunksToRemesh)
                UpdateChunk(chunk, m_BlockWorld.BlockList);

            chunksToRemesh.Clear();
        }

        /// <summary>
        /// Sends a chunk to the remesh handler.
        /// </summary>
        /// <param name="chunk">The chunk to remesh.</param>
        /// <param name="blockList">The blockList to use when remeshing this chunk.</param>
        private void UpdateChunk(Chunk chunk, BlockList blockList)
        {
            var chunkSize = m_World.ChunkSize;

            var chunkProperties = new ChunkProperties();
            chunkProperties.Reset(chunk.Position, chunkSize);

            // TODO Refactor and Optimize this

            for (int x = -1; x <= chunkSize.Value; x++)
                for (int y = -1; y <= chunkSize.Value; y++)
                    for (int z = -1; z <= chunkSize.Value; z++)
                    {
                        var blockPos = new BlockPosition(x, y, z);
                        if (!chunkProperties.IsValidPosition(blockPos))
                            continue;

                        ushort blockID;
                        if (blockPos.IsWithinGrid(chunkSize))
                            blockID = chunk.GetBlockID(blockPos);
                        else
                        {
                            var worldBlockPos = blockPos.LocalToWorld(chunkSize, chunk.Position);
                            var neighborChunkPos = worldBlockPos.ToChunkPosition(chunkSize);
                            var neighborChunk = m_World.GetChunk(neighborChunkPos);

                            if (neighborChunk == null)
                                blockID = 0;
                            else
                                blockID = neighborChunk.GetBlockID(worldBlockPos & chunkSize.Mask);
                        }

                        var blockType = blockList.GetBlockType(blockID);
                        chunkProperties.SetBlock(blockPos, blockType);
                    }

            m_RemeshHandler.RemeshChunk(chunkProperties);
        }

        /// <summary>
        /// Finishes any active remesh tasks.
        /// </summary>
        /// <param name="action">The action to preform on each task.</param>
        internal void FinishRemeshTasks(Action<RemeshTaskStack> action)
        {
            CheckAsyncWorldLoader();

            m_RemeshHandler.FinishTasks(taskStacks);

            foreach (var taskStack in taskStacks)
                action(taskStack);

            taskStacks.Clear();
        }

        /// <summary>
        /// Called each frame to handle any finished async world loading operations.
        /// </summary>
        private void CheckAsyncWorldLoader()
        {
            var requiresRemesh = m_ChunkLoader.Update(out Chunk chunk);

            if (requiresRemesh)
            {
                RemeshAllNeighbors(chunk.Position);
                RemeshDirtyChunks();
            }
        }

        /// <summary>
        /// Requests the chunk at the given position to start loading in the background.
        /// </summary>
        /// <param name="chunkPos">The chunk position.</param>
        /// <returns>True if the operation was started. False if the chunk is already loaded.</returns>
        public bool LoadChunkAsync(ChunkPosition chunkPos)
        {
            var chunk = m_World.GetChunk(chunkPos);
            if (chunk != null)
                return false;

            chunk = m_World.CreateChunk(chunkPos);
            m_ChunkLoader.LoadAsync(chunk);
            return true;
        }
    }
}
