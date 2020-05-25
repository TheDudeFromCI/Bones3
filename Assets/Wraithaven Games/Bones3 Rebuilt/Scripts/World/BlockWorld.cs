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
        private World m_World;
        private RemeshHandler m_RemeshHandler;

        private GridSize ChunkSize => new GridSize(4);

        /// <summary>
        /// Called when the world is enabled, either in the editor or in game.
        /// </summary>
        protected void OnEnable()
        {
            m_World = new World(ChunkSize);

            m_RemeshHandler = new RemeshHandler();
            m_RemeshHandler.AddDistributor(new StandardDistributor());

#if UNITY_EDITOR
            if (!Application.isPlaying)
                UnityEditor.EditorApplication.update += Update;
#endif
        }

        /// <summary>
        /// Called when the world is disabled, either in the editor or in game.
        /// </summary>
        protected void OnDisable()
        {
            foreach (var chunk in m_Chunks)
                ChunkCreator.DestroyChunk(chunk);

            m_Chunks.Clear();

#if UNITY_EDITOR
            if (!Application.isPlaying)
                UnityEditor.EditorApplication.update -= Update;
#endif
        }

        /// <summary>
        /// Applies an edit batch to this world, remeshing chunks as needed.
        /// </summary>
        /// <param name="editBatch">The edit batch to apply.</param>
        public void SetBlocks(IEditBatch editBatch)
        {
            List<Chunk> chunksToRemesh = new List<Chunk>();

            foreach (var block in editBatch.GetBlocks())
            {
                var chunkPos = block.Position.ToChunkPosition(ChunkSize);
                var blockPos = block.Position & ChunkSize.Mask;
                var blockType = block.BlockID;

                var chunk = m_World.CreateChunk(chunkPos);
                chunk.SetBlockID(blockPos, blockType);

                if (!chunksToRemesh.Contains(chunk))
                    chunksToRemesh.Add(chunk);
            }

            foreach (var chunk in chunksToRemesh)
                UpdateChunk(chunk);
        }

        /// <summary>
        /// Sends a chunk to the remesh handler.
        /// </summary>
        /// <param name="chunk">The chunk to remesh.</param>
        private void UpdateChunk(Chunk chunk)
        {
            var chunkProperties = new ChunkProperties();
            chunkProperties.Reset(chunk.Position, ChunkSize);

            for (int x = -1; x <= ChunkSize.Value; x++)
                for (int y = -1; y <= ChunkSize.Value; y++)
                    for (int z = -1; z <= ChunkSize.Value; z++)
                    {
                        var blockPos = new BlockPosition(x, y, z);
                        if (!chunkProperties.IsValidPosition(blockPos))
                            continue;

                        ushort blockID;
                        if (blockPos.IsWithinGrid(ChunkSize))
                            blockID = chunk.GetBlockID(blockPos);
                        else
                            blockID = 0;

                        var blockType = m_BlockList.GetBlockType(blockID);
                        chunkProperties.SetBlock(blockPos, blockType);
                    }

            m_RemeshHandler.RemeshChunk(chunkProperties);
        }

        // A buffer for pulling remesh tasks.
        private readonly List<RemeshTaskStack> taskStacks = new List<RemeshTaskStack>();

        /// <summary>
        /// Called each frame to pull remesh tasks from the remesh handler.
        /// </summary>
        protected void Update()
        {
            m_RemeshHandler.FinishTasks(taskStacks);

            foreach (var taskStack in taskStacks)
            {
                var chunk = GetChunk(taskStack.ChunkPosition);
                ChunkMeshBuilder.UpdateMesh(taskStack, chunk, m_BlockList);
            }

            taskStacks.Clear();
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

            var c = ChunkCreator.LoadChunk(chunkPos, ChunkSize.Value, transform);
            m_Chunks.Add(c);

            return c;
        }
    }
}
