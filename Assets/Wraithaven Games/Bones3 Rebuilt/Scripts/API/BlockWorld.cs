using System.Collections.Generic;

using UnityEngine;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// The main behaviour for containing a voxel block world.
    /// </summary>
    [AddComponentMenu("Bones3/Block World")]
    [SelectionBase, ExecuteAlways, DisallowMultipleComponent]
    [RequireComponent(typeof(BlockListManager))]
    [RequireComponent(typeof(WorldGenerator))]
    public class BlockWorld : MonoBehaviour
    {
        /// <summary>
        /// The chunk size int bits, such as the actual number of blocks is
        /// 1 << CHUNK_SIZE
        /// 
        /// This value provides a good balance between performance and playability.
        /// Use caution when adjusting.
        /// </summary>
        private const int CHUNK_SIZE = 4;

        [SerializeField, HideInInspector] protected string ID = System.Guid.NewGuid().ToString();

        private UnityWorldBuilder m_UnityWorldBuilder;

        /// <summary>
        /// Gets the world builder instance, creating it if needed.
        /// </summary>
        private UnityWorldBuilder WorldBuilder
        {
            get
            {
                if (m_UnityWorldBuilder == null)
                {
                    var chunkSize = new GridSize(CHUNK_SIZE);
                    var blockList = GetComponent<BlockListManager>();
                    m_UnityWorldBuilder = new UnityWorldBuilder(transform, blockList, chunkSize, ID);

                    var worldGen = GetComponent<WorldGenerator>();
                    m_UnityWorldBuilder.WorldContainer.ChunkLoader.AddChunkLoadHandler(worldGen);

                    var renderDistance = GetComponent<VoxelRenderDistance>();
                    renderDistance?.LoadPatternIterator.Reset();
                }

                return m_UnityWorldBuilder;
            }
            set => m_UnityWorldBuilder = value;
        }

        /// <summary>
        /// Gets the chunk size of this world.
        /// </summary>
        public GridSize ChunkSize => WorldBuilder.ChunkSize;

        /// <summary>
        /// Gets the number of active chunk loading tasks being run.
        /// </summary>
        public int ActiveChunkLoadingTasks
            => WorldBuilder.WorldContainer.ChunkLoader.ActiveTasks;

        /// <summary>
        /// Gets the number of active chunk remeshing tasks being run.
        /// </summary>
        public int ActiveRemeshingTasks
            => WorldBuilder.WorldContainer.RemeshHandler.ActiveTasks;

#if UNITY_EDITOR
        /// <summary>
        /// Called when the world is enabled to subscribe to editor frame updates
        /// and initialize.
        /// </summary>
        protected void OnEnable()
        {
            if (!Application.isPlaying)
                UnityEditor.EditorApplication.update += Update;
        }

        /// <summary>
        /// Called when the world is disabled to unsubscribe from editor frame updates.
        /// </summary>
        protected void OnDisable()
        {
            if (!Application.isPlaying)
            {
                UnityEditor.EditorApplication.update -= Update;
                WorldBuilder.ClearWorld();
                WorldBuilder = null;
            }
        }
#endif

        /// <summary>
        /// Called when this block world behaviour is destroyed.
        /// </summary>
        protected void OnDestroy()
        {
            ClearWorld();
            WorldBuilder = null;
        }

        /// <summary>
        /// Applies an edit batch to this world, remeshing chunks as needed.
        /// </summary>
        /// <param name="editBatch">The edit batch to apply.</param>
        public void SetBlocks(IEditBatch editBatch)
            => WorldBuilder.SetBlocks(editBatch.GetBlocks);

        /// <summary>
        /// Applies an edit batch to this world, remeshing chunks as needed.
        /// </summary>
        /// <param name="editBatch">The edit batch to apply.</param>
        public void SetBlocks(EditBatch editBatch)
            => WorldBuilder.SetBlocks(editBatch);


        /// <summary>
        /// Sets a world in the world to a given ID.
        /// </summary>
        /// <param name="blockPos">The block position.</param>
        /// <param name="blockID">The ID of the block to place.</param>
        public void SetBlock(BlockPosition blockPos, ushort blockID)
            => WorldBuilder.SetBlock(blockPos, blockID);

        /// <summary>
        /// Gets the block type at the given world position.
        /// 
        /// For ungenerated or unloaded chunks, the Ungenerated block type is return.
        /// </summary>
        /// <param name="blockPos">The position of the block.</param>
        /// <param name="createChunk">Whether or not to create (or load) the chunk if it doesn't currently exist.</param>
        /// <returns>The block type.</returns>
        public BlockType GetBlock(BlockPosition blockPos, bool createChunk = false)
            => WorldBuilder.GetBlock(blockPos, createChunk);

        /// <summary>
        /// Called each frame to pull remesh tasks from the remesh handler.
        /// </summary>
        protected void Update()
            => WorldBuilder.Update();


        /// <summary>
        /// Saves the world to file.
        /// </summary>
        public void SaveWorld()
            => WorldBuilder.SaveWorld();

        /// <summary>
        /// Clears all loaded chunk data for this world.
        /// </summary>
        public void ClearWorld()
        {
            WorldBuilder.SaveWorld();
            gameObject.SendMessage("OnWorldClear", SendMessageOptions.DontRequireReceiver);
        }

        /// <summary>
        /// Force loads all chunks within a given region, if not already loaded.
        /// </summary>
        /// <param name="center">The center of the bounding region.</param>
        /// <param name="extents">The radius of each axis.</param>
        public void LoadChunkRegion(Vector3Int center, Vector3Int extents)
            => WorldBuilder.LoadChunkRegion(center, extents);

        /// <summary>
        /// Requests the chunk at the given position to start loading in the background.
        /// </summary>
        /// <param name="chunkPos">The chunk position.</param>
        /// <returns>True if the operation was started. False if the chunk is already loaded.</returns>
        public bool LoadChunkAsync(ChunkPosition chunkPos)
            => WorldBuilder.LoadChunkAsync(chunkPos);
    }
}
