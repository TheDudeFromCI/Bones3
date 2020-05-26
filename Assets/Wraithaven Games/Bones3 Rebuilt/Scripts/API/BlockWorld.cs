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
        private ChunkCreator m_ChunkCreator;
        private ChunkMeshBuilder m_ChunkMeshBuilder;

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
            m_World = new World(ChunkSize);

            m_RemeshHandler = new RemeshHandler();
            m_RemeshHandler.AddDistributor(new StandardDistributor());

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
                var chunkPos = block.Position.ToChunkPosition(ChunkSize);
                var blockPos = block.Position & ChunkSize.Mask;
                var blockType = block.BlockID;

                var chunk = m_World.CreateChunk(chunkPos);

                if (chunk.GetBlockID(blockPos) == blockType)
                    continue; // Don't remesh unchanged chunks

                if (blockType < 0 || blockType >= m_BlockList.BlockCount)
                {
                    RemeshAllDirtyChunks();
                    throw new System.ArgumentOutOfRangeException("editBatch", $"Invalid block type '{blockType}'!");
                }

                chunk.SetBlockID(blockPos, (ushort)blockType);
                RemeshEffectedChunks(blockPos, chunkPos);
            }

            RemeshAllDirtyChunks();
        }

        /// <summary>
        /// Sets a world in the world to a given ID.
        /// </summary>
        /// <param name="blockPos">The block position.</param>
        /// <param name="blockID">The ID of the block to place.</param>
        public void SetBlock(BlockPosition blockPos, int blockID)
        {
            SetBlocks(() => PlaceSingleBlock(blockPos, blockID));
        }

        /// <summary>
        /// An edit batch delegate for placing a single block.
        /// </summary>
        /// <param name="blockPos">The block position.</param>
        /// <param name="blockID">The block ID.</param>
        /// <returns>The edit batch enumerable.</returns>
        private IEnumerable<BlockPlacement> PlaceSingleBlock(BlockPosition blockPos, int blockID)
        {
            yield return new BlockPlacement
            {
                Position = blockPos,
                BlockID = blockID,
            };
        }

        /// <summary>
        /// Gets the block type at the given world position.
        /// 
        /// For ungenerated or unloaded chunks, the Ungenerated block type is return.
        /// </summary>
        /// <param name="blockPosition">The position of the block.</param>
        /// <param name="createChunk">Whether or not to create (or load) the chunk if it doesn't currently exist.</param>
        /// <returns>The block type.</returns>
        public BlockType GetBlock(BlockPosition blockPosition, bool createChunk = false)
        {
            var chunkPos = blockPosition.ToChunkPosition(ChunkSize);
            var blockPos = blockPosition & ChunkSize.Mask;

            ushort blockID;
            if (createChunk)
                blockID = m_World.CreateChunk(chunkPos).GetBlockID(blockPos);
            else
                blockID = m_World.GetChunk(chunkPos)?.GetBlockID(blockPos) ?? 0;

            return m_BlockList.GetBlockType(blockID);
        }

        // A buffer for holding chunks to remesh
        private readonly List<Chunk> chunksToRemesh = new List<Chunk>();

        /// <summary>
        /// Determines which chunks should be remeshed based on the given block position.
        /// </summary>
        /// <param name="blockPos">The local block position.</param>
        /// <param name="toRemesh">The chunk position.</param>
        private void RemeshEffectedChunks(BlockPosition blockPos, ChunkPosition chunkPos)
        {
            RemeshChunkAt(chunkPos);

            if (blockPos.X == 0)
                RemeshChunkAt(chunkPos.ShiftAlongDirection(1));

            if (blockPos.X == ChunkSize.Mask)
                RemeshChunkAt(chunkPos.ShiftAlongDirection(0));

            if (blockPos.Y == 0)
                RemeshChunkAt(chunkPos.ShiftAlongDirection(3));

            if (blockPos.Y == ChunkSize.Mask)
                RemeshChunkAt(chunkPos.ShiftAlongDirection(2));

            if (blockPos.Z == 0)
                RemeshChunkAt(chunkPos.ShiftAlongDirection(5));

            if (blockPos.Z == ChunkSize.Mask)
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
        /// Triggers a remesh event for all chunks marked for remesh and clears the list.
        /// </summary>
        private void RemeshAllDirtyChunks()
        {
            foreach (var chunk in chunksToRemesh)
                UpdateChunk(chunk);

            chunksToRemesh.Clear();
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
                        {
                            var worldBlockPos = LocalBlockPosToWorld(blockPos, chunk.Position);
                            var neighborChunkPos = worldBlockPos.ToChunkPosition(ChunkSize);
                            var neighborChunk = m_World.GetChunk(neighborChunkPos);

                            if (neighborChunk == null)
                                blockID = 0;
                            else
                                blockID = neighborChunk.GetBlockID(worldBlockPos & ChunkSize.Mask);
                        }

                        var blockType = m_BlockList.GetBlockType(blockID);
                        chunkProperties.SetBlock(blockPos, blockType);
                    }

            m_RemeshHandler.RemeshChunk(chunkProperties);
        }

        private BlockPosition LocalBlockPosToWorld(BlockPosition blockPos, ChunkPosition chunkPos)
        {
            chunkPos *= ChunkSize.Value;
            return blockPos + new BlockPosition(chunkPos.X, chunkPos.Y, chunkPos.Z);
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
                m_ChunkMeshBuilder.UpdateMesh(taskStack, chunk);
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

            var c = m_ChunkCreator.LoadChunk(chunkPos);
            m_Chunks.Add(c);

            return c;
        }
    }
}
