using System.Collections.Generic;
using UnityEngine;

namespace WraithavenGames.Bones3
{
    internal class UnityWorldBuilder
    {
        private readonly List<BlockChunk> m_Chunks = new List<BlockChunk>();
        private readonly ChunkCreator m_ChunkCreator;
        private readonly ChunkMeshBuilder m_ChunkMeshBuilder;
        private readonly WorldSaver m_WorldSaver;
        private readonly BlockListManager m_BlockList;

        /// <summary>
        /// Gets the chunk size for this world.
        /// </summary>
        internal GridSize ChunkSize { get; }

        /// <summary>
        /// The unique ID value of this world.
        /// </summary>
        internal string ID { get; }

        /// <summary>
        /// Gets the world container for this behaviour.
        /// </summary>
        internal WorldContainer WorldContainer { get; private set; }

        /// <summary>
        /// Creates a new Unity world builder object.
        /// </summary>
        /// <param name="transform">The transform to add chunk gameobjects to.</param>
        /// <param name="blockList">The block list to read from.</param>
        /// <param name="chunkSize">The chunk size of the world.</param>
        /// <param name="id">The ID value of the world.</param>
        internal UnityWorldBuilder(Transform transform, BlockListManager blockList, GridSize chunkSize, string id)
        {
            ChunkSize = chunkSize;
            ID = id;

            m_BlockList = blockList;

            var world = new World(ChunkSize, ID);
            WorldContainer = new WorldContainer(world, blockList);
            m_WorldSaver = new WorldSaver(world);

            m_ChunkCreator = new ChunkCreator(transform, chunkSize);
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
            foreach (var block in editBatch())
            {
                if (block.BlockID >= m_BlockList.BlockCount)
                    throw new System.ArgumentOutOfRangeException("editBatch", $"Invalid block type '{block.BlockID}'!");

                WorldContainer.SetBlock(block.Position, block.BlockID);
            }

            WorldContainer.RemeshDirtyChunks();
        }

        /// <summary>
        /// Sets a world in the world to a given ID.
        /// </summary>
        /// <param name="blockPos">The block position.</param>
        /// <param name="blockID">The ID of the block to place.</param>
        internal void SetBlock(BlockPosition blockPos, ushort blockID)
        {
            if (blockID >= m_BlockList.BlockCount)
                throw new System.ArgumentOutOfRangeException("blockID", $"Invalid block type '{blockID}'!");

            WorldContainer.SetBlock(blockPos, blockID);
            WorldContainer.RemeshDirtyChunks();
        }

        /// <summary>
        /// Gets the block type at the given world position.
        /// 
        /// For ungenerated or unloaded chunks, the Ungenerated block type is return.
        /// </summary>
        /// <param name="blockPos">The position of the block.</param>
        /// <param name="createChunk">Whether or not to create (or load) the chunk if it doesn't currently exist.</param>
        /// <returns>The block type.</returns>
        internal BlockType GetBlock(BlockPosition blockPos, bool createChunk = false)
        {
            ushort blockID = WorldContainer.GetBlock(blockPos, createChunk);
            return m_BlockList.GetBlockType(blockID);
        }

        /// <summary>
        /// Called each frame to pull remesh tasks from the remesh handler.
        /// </summary>
        internal void Update()
        {
            WorldContainer.FinishRemeshTasks(BuildChunkMesh);
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
        internal void LoadChunkRegion(Vector3Int center, Vector3Int extents)
        {
            var min = center - extents;
            var max = center + extents;

            for (int x = min.x; x <= max.x; x++)
                for (int y = min.y; y <= max.y; y++)
                    for (int z = min.z; z <= max.z; z++)
                    {
                        var blockPos = new BlockPosition(x, y, z) * ChunkSize.Value;
                        WorldContainer.GetBlock(blockPos, true);
                    }
        }

        /// <summary>
        /// Requests the chunk at the given position to start loading in the background.
        /// </summary>
        /// <param name="chunkPos">The chunk position.</param>
        /// <returns>True if the operation was started. False if the chunk is already loaded.</returns>
        internal bool LoadChunkAsync(ChunkPosition chunkPos) => WorldContainer.LoadChunkAsync(chunkPos);
    }
}