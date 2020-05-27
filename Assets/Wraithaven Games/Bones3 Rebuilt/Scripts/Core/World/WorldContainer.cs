using System;
using System.Collections.Generic;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// A container which wraps the World API and RemeshHandler API.
    /// </summary>
    internal class WorldContainer
    {
        private readonly List<RemeshTaskStack> taskStacks = new List<RemeshTaskStack>();
        private readonly BlockWorld m_BlockWorld;

        /// <summary>
        /// Gets the chunk loader being used by this container.
        /// </summary>
        /// <value>The chunk loader.</value>
        internal AsyncChunkLoader ChunkLoader { get; }

        /// <summary>
        /// Gets the remesh handler being used by this container.
        /// </summary>
        /// <value>The remesh handler.</value>
        internal RemeshHandler RemeshHandler { get; }

        /// <summary>
        /// Gets the world this container is managing.
        /// </summary>
        /// <value></value>
        internal World World { get; }

        /// <summary>
        /// Creates a new world container for the given world.
        /// </summary>
        /// <param name="world">The world.</param>
        internal WorldContainer(World world, BlockWorld blockWorld)
        {
            World = world;
            m_BlockWorld = blockWorld;

            ChunkLoader = new AsyncChunkLoader();
            ChunkLoader.AddChunkLoadHandler(new WorldLoader(world));

            RemeshHandler = new RemeshHandler(blockWorld);
            RemeshHandler.AddDistributor(new StandardDistributor(World.ChunkSize));
        }

        /// <summary>
        /// Sets the block at the given position.
        /// </summary>
        /// <param name="blockPos">The block position.</param>
        /// <param name="blockID">The block ID to assign.</param>
        internal void SetBlock(BlockPosition blockPos, ushort blockID)
        {
            var chunkPos = blockPos.ToChunkPosition(World.ChunkSize);
            blockPos &= World.ChunkSize.Mask;

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
            var chunkPos = blockPos.ToChunkPosition(World.ChunkSize);
            blockPos &= World.ChunkSize.Mask;

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
            var chunk = World.GetChunk(chunkPos);
            if (chunk != null || !create)
                return chunk;

            chunk = World.CreateChunk(chunkPos);
            bool remesh = ChunkLoader.LoadSync(chunk);

            if (remesh)
                RemeshAllNeighbors(chunkPos, false);

            return chunk;
        }

        /// <summary>
        /// Remeshes the chunk at the given position and all chunks touching it.
        /// </summary>
        /// <param name="chunkPos">The chunk position.</param>
        private void RemeshAllNeighbors(ChunkPosition chunkPos, bool later)
        {
            RemeshChunkAt(chunkPos, later);
            RemeshChunkAt(chunkPos.ShiftAlongDirection(0), later);
            RemeshChunkAt(chunkPos.ShiftAlongDirection(1), later);
            RemeshChunkAt(chunkPos.ShiftAlongDirection(2), later);
            RemeshChunkAt(chunkPos.ShiftAlongDirection(3), later);
            RemeshChunkAt(chunkPos.ShiftAlongDirection(4), later);
            RemeshChunkAt(chunkPos.ShiftAlongDirection(5), later);
        }

        /// <summary>
        /// Determines which chunks should be remeshed based on the given block position.
        /// </summary>
        /// <param name="blockPos">The local block position.</param>
        /// <param name="toRemesh">The chunk position.</param>
        private void RemeshEffectedChunks(BlockPosition blockPos, ChunkPosition chunkPos)
        {
            RemeshChunkAt(chunkPos, false);

            int max = World.ChunkSize.Mask;

            if (blockPos.X == 0)
                RemeshChunkAt(chunkPos.ShiftAlongDirection(1), false);

            if (blockPos.X == max)
                RemeshChunkAt(chunkPos.ShiftAlongDirection(0), false);

            if (blockPos.Y == 0)
                RemeshChunkAt(chunkPos.ShiftAlongDirection(3), false);

            if (blockPos.Y == max)
                RemeshChunkAt(chunkPos.ShiftAlongDirection(2), false);

            if (blockPos.Z == 0)
                RemeshChunkAt(chunkPos.ShiftAlongDirection(5), false);

            if (blockPos.Z == max)
                RemeshChunkAt(chunkPos.ShiftAlongDirection(4), false);
        }

        /// <summary>
        /// Triggers the given at the given position to be remeshed, if not already in the list.
        /// Preforms no action if the chunk does not exist.
        /// </summary>
        /// <param name="chunkPos">The chunk position.</param>
        /// <param name="later">Whether to remesh the chunk now or later.</param>
        private void RemeshChunkAt(ChunkPosition chunkPos, bool later)
        {
            if (!World.DoesChunkExist(chunkPos))
                return;

            if (later)
                RemeshHandler.RemeshChunkLater(chunkPos);
            else
                RemeshHandler.RemeshChunk(chunkPos);
        }

        /// <summary>
        /// Finishes any active remesh tasks.
        /// </summary>
        /// <param name="action">The action to preform on each task.</param>
        internal void FinishRemeshTasks(Action<RemeshTaskStack> action)
        {
            CheckAsyncWorldLoader();

            RemeshHandler.FinishTasks(taskStacks);

            foreach (var taskStack in taskStacks)
                action(taskStack);

            taskStacks.Clear();
        }

        /// <summary>
        /// Called each frame to handle any finished async world loading operations.
        /// </summary>
        private void CheckAsyncWorldLoader()
        {
            var requiresRemesh = ChunkLoader.Update(out Chunk chunk);

            if (requiresRemesh)
                RemeshAllNeighbors(chunk.Position, true);
        }

        /// <summary>
        /// Requests the chunk at the given position to start loading in the background.
        /// </summary>
        /// <param name="chunkPos">The chunk position.</param>
        /// <returns>True if the operation was started. False if the chunk is already loaded.</returns>
        public bool LoadChunkAsync(ChunkPosition chunkPos)
        {
            var chunk = World.GetChunk(chunkPos);
            if (chunk != null)
                return false;

            chunk = World.CreateChunk(chunkPos);
            ChunkLoader.LoadAsync(chunk);
            return true;
        }
    }
}
