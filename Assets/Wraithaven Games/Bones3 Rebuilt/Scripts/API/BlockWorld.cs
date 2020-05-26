using System.Collections.Generic;
using UnityEngine;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// The main behaviour for containing a voxel block world.
    /// </summary>
    [SelectionBase, ExecuteAlways]
    public class BlockWorld : MonoBehaviour
    {
        [Tooltip("The block properties container to use for this world.")]
        [SerializeField] protected BlockList m_BlockList;

        private readonly List<BlockChunk> m_Chunks = new List<BlockChunk>();
        private WorldContainer m_WorldContainer;
        private ChunkCreator m_ChunkCreator;
        private ChunkMeshBuilder m_ChunkMeshBuilder;
        private WorldSaver m_WorldSaver;

        /// <summary>
        /// Gets the chunk size for this world.
        /// 
        /// If this value is modified, all existing world data should be deleted and recreated.
        /// </summary>
        /// <returns>The chunk size.</returns>
        public GridSize ChunkSize => new GridSize(4);

        /// <summary>
        /// Gets the block list being used by this world.
        /// </summary>
        public BlockList BlockList => m_BlockList;

        /// <summary>
        /// Called when the world object is constructed to initialize data.
        /// </summary>
        protected void Awake()
        {
            var world = new World(ChunkSize);
            m_WorldContainer = new WorldContainer(world);

            m_WorldSaver = new WorldSaver(world);

            m_ChunkCreator = new ChunkCreator(this);
            m_ChunkMeshBuilder = new ChunkMeshBuilder(this);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Called when the world is enabled to subscribe to editor frame updates
        /// and initialize.
        /// </summary>
        protected void OnEnable()
        {
            if (!Application.isPlaying)
            {
                UnityEditor.EditorApplication.update += Update;
                Awake();
            }
        }

        /// <summary>
        /// Called when the world is disabled to unsubscribe from editor frame updates.
        /// </summary>
        protected void OnDisable()
        {
            if (!Application.isPlaying)
            {
                UnityEditor.EditorApplication.update -= Update;

                foreach (var chunk in m_Chunks)
                    m_ChunkCreator.DestroyChunk(chunk);
                m_Chunks.Clear();
            }
        }
#endif

        /// <summary>
        /// Applies an edit batch to this world, remeshing chunks as needed.
        /// </summary>
        /// <param name="editBatch">The edit batch to apply.</param>
        public void SetBlocks(IEditBatch editBatch) => SetBlocks(editBatch.GetBlocks);

        /// <summary>
        /// Applies an edit batch to this world, remeshing chunks as needed.
        /// </summary>
        /// <param name="editBatch">The edit batch to apply.</param>
        public void SetBlocks(EditBatch editBatch)
        {
            foreach (var block in editBatch())
            {
                if (block.BlockID >= m_BlockList.BlockCount)
                {
                    m_WorldContainer.RemeshDirtyChunks(m_BlockList);
                    throw new System.ArgumentOutOfRangeException("editBatch", $"Invalid block type '{block.BlockID}'!");
                }

                m_WorldContainer.SetBlock(block.Position, block.BlockID);
            }

            m_WorldContainer.RemeshDirtyChunks(m_BlockList);
        }

        /// <summary>
        /// Sets a world in the world to a given ID.
        /// </summary>
        /// <param name="blockPos">The block position.</param>
        /// <param name="blockID">The ID of the block to place.</param>
        public void SetBlock(BlockPosition blockPos, ushort blockID)
        {
            if (blockID >= m_BlockList.BlockCount)
                throw new System.ArgumentOutOfRangeException("blockID", $"Invalid block type '{blockID}'!");

            m_WorldContainer.SetBlock(blockPos, blockID);
            m_WorldContainer.RemeshDirtyChunks(m_BlockList);
        }

        /// <summary>
        /// Gets the block type at the given world position.
        /// 
        /// For ungenerated or unloaded chunks, the Ungenerated block type is return.
        /// </summary>
        /// <param name="blockPos">The position of the block.</param>
        /// <param name="createChunk">Whether or not to create (or load) the chunk if it doesn't currently exist.</param>
        /// <returns>The block type.</returns>
        public BlockType GetBlock(BlockPosition blockPos, bool createChunk = false)
        {
            ushort blockID = m_WorldContainer.GetBlock(blockPos, createChunk);
            return m_BlockList.GetBlockType(blockID);
        }

        // A buffer for pulling remesh tasks.

        /// <summary>
        /// Called each frame to pull remesh tasks from the remesh handler.
        /// </summary>
        protected void Update()
        {
            m_WorldContainer.FinishRemeshTasks(task =>
            {
                var chunk = GetChunk(task.ChunkPosition);
                m_ChunkMeshBuilder.UpdateMesh(task, chunk);
            });
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
        public void SaveWorld() => m_WorldSaver.SaveWorld();
    }
}
