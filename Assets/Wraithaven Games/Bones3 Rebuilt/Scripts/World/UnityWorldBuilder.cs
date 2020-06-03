using System.Collections.Generic;

using UnityEngine;

namespace WraithavenGames.Bones3
{
    internal class UnityWorldBuilder
    {
        private readonly List<BlockChunk> m_Chunks = new List<BlockChunk>();
        private readonly ServerThread m_ServerThread;
        private readonly ChunkCreator m_ChunkCreator;
        private readonly ChunkMeshBuilder m_ChunkMeshBuilder;
        private readonly WorldSaver m_WorldSaver;

        /// <summary>
        /// Gets the chunk size for this world.
        /// </summary>
        internal GridSize ChunkSize { get; }

        /// <summary>
        /// Creates a new Unity world builder object.
        /// </summary>
        /// <param name="transform">The transform to add chunk gameobjects to.</param>
        /// <param name="blockList">The block list to read from.</param>
        /// <param name="chunkSize">The chunk size of the world.</param>
        /// <param name="id">The ID value of the world.</param>
        internal UnityWorldBuilder(Transform transform, BlockListManager blockList, WorldProperties worldProperties)
        {
            ChunkSize = worldProperties.ChunkSize;
            m_ServerThread = EmbeddedServer.Initialize(worldProperties);

            m_ChunkCreator = new ChunkCreator(transform, ChunkSize);
            m_ChunkMeshBuilder = new ChunkMeshBuilder(blockList);
        }

        /// <summary>
        /// Applies an edit batch to this world, remeshing chunks as needed.
        /// </summary>
        /// <param name="editBatch">The edit batch to apply.</param>
        internal void SetBlocks(IEditBatch editBatch) => SetBlocks(editBatch.GetBlocks);

        /// <summary>
        /// Applies an edit batch to this world, remeshing chunks as needed.
        /// </summary>
        /// <param name="editBatch">The edit batch to apply.</param>
        internal void SetBlocks(EditBatch editBatch)
        {
            m_ServerThread.RunTask(new SetBlocksTask(editBatch));
        }

        /// <summary>
        /// Sets a world in the world to a given ID.
        /// </summary>
        /// <param name="blockPos">The block position.</param>
        /// <param name="blockID">The ID of the block to place.</param>
        internal void SetBlock(BlockPosition blockPos, ushort blockID)
        {
            m_ServerThread.RunTask(new SetBlockTask(blockPos, blockID));
        }

        /// <summary>
        /// Gets the block type at the given world position.
        /// 
        /// For ungenerated or unloaded chunks, the Ungenerated block type is return.
        /// </summary>
        /// <param name="blockPos">The position of the block.</param>
        /// <param name="createChunk">Whether or not to create (or load) the chunk if it doesn't currently exist.</param>
        /// <returns>The block type.</returns>
        internal ushort GetBlock(BlockPosition blockPos, bool createChunk = false)
        {
            var task = new GetBlockTask(blockPos, createChunk);
            m_ServerThread.RunTaskSync(task);

            return task.BlockID;
        }

        /// <summary>
        /// Called each frame to pull remesh tasks from the remesh handler.
        /// </summary>
        internal void Update()
        {
            m_ServerThread.Update();

            // TODO REIMPLEMENT REMESHING!
            // This relies on the remesh handles being moved entirely to the server
            // and listening for remesh events.
        }

        /// <summary>
        /// Updates a chunk mesh based on the results of a finished task stack.
        /// </summary>
        /// <param name="task">The finished task stack.</param>
        private void BuildChunkMesh(RemeshTaskStack task)
        {
            var chunk = GetChunk(task.ChunkPosition);
            m_ChunkMeshBuilder.UpdateMesh(task, chunk);
        }

        /// <summary>
        /// Gets or creates the chunk at the given position.
        /// </summary>
        /// <param name="chunkPos">The chunk position.</param>
        /// <returns>The block chunk.</returns>
        private BlockChunk GetChunk(ChunkPosition chunkPos)
        {
            foreach (var chunk in m_Chunks)
                if (chunk.Position.Equals(chunkPos))
                    return chunk;

            var c = m_ChunkCreator.LoadChunk(chunkPos);
            m_Chunks.Add(c);

            return c;
        }

        /// <summary>
        /// Saves the world to file.
        /// </summary>
        internal void SaveWorld() => m_WorldSaver.SaveWorld();

        /// <summary>
        /// Clears all loaded chunk data for this world.
        /// </summary>
        internal void ClearWorld()
        {
            foreach (var chunk in m_Chunks)
                m_ChunkCreator.DestroyChunk(chunk);
            m_Chunks.Clear();
        }

        /// <summary>
        /// Force loads all chunks within a given region, if not already loaded.
        /// </summary>
        /// <param name="center">The center of the bounding region.</param>
        /// <param name="extents">The radius of each axis.</param>
        /// <returns>True if any additional chunks were loaded.</returns>
        internal bool LoadChunkRegion(ChunkPosition center, Vector3Int extents)
        {
            // TODO Remove "was loaded" check, to stop tasks from being run synchronously.

            var min = new Vector3Int
            {
                x = center.X - extents.x,
                y = center.Y - extents.y,
                z = center.Z - extents.z,
            };

            var max = new Vector3Int
            {
                x = center.X + extents.x,
                y = center.Y + extents.y,
                z = center.Z + extents.z,
            };

            bool loaded = false;
            for (int x = min.x; x <= max.x; x++)
                for (int y = min.y; y <= max.y; y++)
                    for (int z = min.z; z <= max.z; z++)
                    {
                        var chunkPos = new ChunkPosition(x, y, z);
                        var task = new LoadChunkTask(chunkPos);
                        m_ServerThread.RunTaskSync(task);

                        loaded |= task.WasJustLoaded;
                    }

            return loaded;
        }

        /// <summary>
        /// Requests the chunk at the given position to start loading in the background.
        /// </summary>
        /// <param name="chunkPos">The chunk position.</param>
        internal void LoadChunkAsync(ChunkPosition chunkPos)
        {
            m_ServerThread.RunTask(new LoadChunkTask(chunkPos));
        }
    }
}
