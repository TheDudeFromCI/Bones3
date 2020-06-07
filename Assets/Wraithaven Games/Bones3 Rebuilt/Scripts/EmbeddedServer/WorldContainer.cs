using System.Collections.Generic;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// A container which wraps the World API.
    /// </summary>
    /// <remarks>
    /// All API calls to this container must be preformed within the world server thread.
    /// </remarks>
    public class WorldContainer
    {
        private readonly List<ChunkPosition> m_DirtyChunks = new List<ChunkPosition>();

        /// <summary>
        /// Gets the event queue for this world container.
        /// </summary>
        internal EventQueue EventQueue { get; }

        /// <summary>
        /// Gets the chunk loader being used by this container.
        /// </summary>
        internal ChunkLoader ChunkLoader { get; }

        /// <summary>
        /// Gets the remesh handler being used by this container.
        /// </summary>
        internal RemeshHandler RemeshHandler { get; }

        /// <summary>
        /// Gets the world this container is managing.
        /// </summary>
        internal World World { get; }

        /// <summary>
        /// Gets the block list being managed by this world container.
        /// </summary>
        internal ServerBlockList BlockList { get; }

        /// <summary>
        /// Creates a new world container for the given world.
        /// </summary>
        /// <param name="world">The world.</param>
        /// <param name="world">The world.</param>
        internal WorldContainer(WorldProperties worldProperties)
        {
            World = new World(worldProperties.ChunkSize, worldProperties.ID);
            BlockList = new ServerBlockList();

            EventQueue = new EventQueue();

            ChunkLoader = new ChunkLoader();
            ChunkLoader.AddChunkLoadHandler(new WorldLoader(World.ID));
            ChunkLoader.AddChunkLoadHandler(worldProperties.WorldGenerator);

            RemeshHandler = new RemeshHandler();
            RemeshHandler.AddDistributor(new StandardDistributor(World.ChunkSize, BlockList));
        }

        /// <summary>
        /// Sets the block at the given position.
        /// </summary>
        /// <param name="blockPos">The block position.</param>
        /// <param name="blockID">The block ID to assign.</param>
        public void SetBlock(BlockPosition blockPos, ushort blockID)
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
        public ushort GetBlock(BlockPosition blockPos, bool createChunk = false)
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
            bool remesh = ChunkLoader.Load(chunk);

            if (remesh)
            {
                MarkForRemesh(chunkPos);
                MarkForRemesh(chunkPos.ShiftAlongDirection(0));
                MarkForRemesh(chunkPos.ShiftAlongDirection(1));
                MarkForRemesh(chunkPos.ShiftAlongDirection(2));
                MarkForRemesh(chunkPos.ShiftAlongDirection(3));
                MarkForRemesh(chunkPos.ShiftAlongDirection(4));
                MarkForRemesh(chunkPos.ShiftAlongDirection(5));
                RemeshDirtyChunks();
            }

            return chunk;
        }

        /// <summary>
        /// Determines which chunks should be remeshed based on the given block position.
        /// </summary>
        /// <param name="blockPos">The local block position.</param>
        /// <param name="chunkPos">The chunk position.</param>
        private void RemeshEffectedChunks(BlockPosition blockPos, ChunkPosition chunkPos)
        {
            MarkForRemesh(chunkPos);

            if (blockPos.X == 0)
                MarkForRemesh(chunkPos.ShiftAlongDirection(1));

            if (blockPos.X == World.ChunkSize.Mask)
                MarkForRemesh(chunkPos.ShiftAlongDirection(0));

            if (blockPos.Y == 0)
                MarkForRemesh(chunkPos.ShiftAlongDirection(3));

            if (blockPos.Y == World.ChunkSize.Mask)
                MarkForRemesh(chunkPos.ShiftAlongDirection(2));

            if (blockPos.Z == 0)
                MarkForRemesh(chunkPos.ShiftAlongDirection(5));

            if (blockPos.Z == World.ChunkSize.Mask)
                MarkForRemesh(chunkPos.ShiftAlongDirection(4));

            RemeshDirtyChunks();
        }

        /// <summary>
        /// Marks a given chunk to be remeshed.
        /// </summary>
        /// <param name="chunkPos">The position of the chunk to remesh.</param>
        private void MarkForRemesh(ChunkPosition chunkPos)
        {
            if (!World.DoesChunkExist(chunkPos))
                return;

            if (m_DirtyChunks.Contains(chunkPos))
                return;

            m_DirtyChunks.Add(chunkPos);
        }

        /// <summary>
        /// Remeshes all chunks which are marked as dirty.
        /// </summary>
        public void RemeshDirtyChunks()
        {
            if (m_DirtyChunks.Count == 0)
                return;

            List<RemeshTaskStack> tasks = new List<RemeshTaskStack>();

            for (int i = 0; i < m_DirtyChunks.Count; i++)
            {
                var task = RemeshHandler.RemeshChunk(this, m_DirtyChunks[i]);
                tasks.Add(task);
            }

            for (int i = 0; i < tasks.Count; i++)
            {
                tasks[i].Finish();
                AddEvent(new ChunkRemeshEvent(tasks[i]));
            }

            m_DirtyChunks.Clear();
        }

        /// <summary>
        /// Checks if the given chunk exists or not.
        /// </summary>
        /// <returns>True if the chunk exists, false otherwise.</returns>
        public bool DoesChunkExist(ChunkPosition chunkPos) => World.DoesChunkExist(chunkPos);

        /// <summary>
        /// Causes the target chunk to be loaded, if not already loaded.
        /// </summary>
        public void LoadChunk(ChunkPosition chunkPos) => GetChunk(chunkPos, true);

        /// <summary>
        /// Adds an event to the event queue.
        /// </summary>
        public void AddEvent(IBlockWorldEvent ev) => EventQueue.AddEvent(ev);
    }
}
