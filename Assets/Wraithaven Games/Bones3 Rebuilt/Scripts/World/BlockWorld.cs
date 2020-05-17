using System.Collections.Generic;

using UnityEngine;

#pragma warning disable 649

namespace Bones3Rebuilt
{
    /// <summary>
    /// The main behaviour for containing a voxel block world.
    /// </summary>
    [SelectionBase, ExecuteAlways]
    public class BlockWorld : MonoBehaviour
    {
        [Tooltip("The list of texture atlases to use when referencing block materials.")]
        [SerializeField] private Bones3TextureAtlasList m_TextureAtlasList;

        private readonly List<BlockChunk> m_Chunks = new List<BlockChunk>();
        private readonly ChunkMeshBuilder m_MeshBuilder = new ChunkMeshBuilder();
        private readonly ChunkCreator m_ChunkCreator = new ChunkCreator();
        private WorldContainer m_WorldContainer;

        /// <summary>
        /// Gets the world container being managed.
        /// </summary>
        /// <value>The world container, for editing the world.</value>
        public WorldContainer WorldContainer
        {
            get
            {
                if (m_WorldContainer == null)
                    OnEnable();

                return m_WorldContainer;
            }
        }

        /// <summary>
        /// Called when the BlockWorld behaviour is enabled.
        /// </summary>
        private void OnEnable()
        {
            if (m_WorldContainer != null)
                return; // Already enabled

            var chunkSize = new GridSize(4);

            var world = new World(chunkSize);
            var blockList = new BlockTypeList();
            var remesh = new RemeshHandler();
            var database = new WorldDatabase("/home/thedudefromci/Bones3/TestWorld");

            remesh.AddDistributor(new StandardDistributor());

            m_WorldContainer = new WorldContainer(world, remesh, blockList, database);
            m_WorldContainer.BlockContainerProvider.OnBlockContainerCreated += OnChunkCreated;
            m_WorldContainer.BlockContainerProvider.OnBlockContainerDestroyed += OnChunkDestroyed;
            m_WorldContainer.RemeshHandler.OnRemeshFinish += OnRemeshFinished;

            // TODO Load block types normally

            var atlas1 = m_TextureAtlasList.GetAtlas(0);
            var grass = atlas1.GetTexture(0);
            var sideDirt = atlas1.GetTexture(1);
            var dirt = atlas1.GetTexture(2);

            var atlas2 = m_TextureAtlasList.GetAtlas(1);
            var glass = atlas2.GetTexture(0);

            blockList.AddBlockType(new BlockBuilder(blockList.NextBlockID)
                .Name("Grass")
                .Texture(0, sideDirt)
                .Texture(1, sideDirt)
                .Texture(2, grass)
                .Texture(3, dirt)
                .Texture(4, sideDirt)
                .Texture(5, sideDirt)
                .FaceRotation(4, FaceRotation.Clockwise90)
                .FaceRotation(5, FaceRotation.Clockwise270)
                .Build());

            blockList.AddBlockType(new BlockBuilder(blockList.NextBlockID)
                .Name("Glass")
                .Texture(0, glass)
                .Texture(1, glass)
                .Texture(2, glass)
                .Texture(3, glass)
                .Texture(4, glass)
                .Texture(5, glass)
                .FaceRotation(4, FaceRotation.Clockwise90)
                .FaceRotation(5, FaceRotation.Clockwise270)
                .Build());

#if UNITY_EDITOR
            if (!Application.isPlaying)
                UnityEditor.EditorApplication.update += Update;
#endif
        }

        /// <summary>
        /// Called when the BlockWorld behaviour is disabled.
        /// </summary>
        private void OnDisable()
        {
            if (m_WorldContainer == null)
                return; // Already disabled

            m_WorldContainer.BlockContainerProvider.OnBlockContainerCreated -= OnChunkCreated;
            m_WorldContainer.BlockContainerProvider.OnBlockContainerDestroyed -= OnChunkDestroyed;
            m_WorldContainer = null;

            foreach (var chunk in m_Chunks)
                m_ChunkCreator.DestroyChunk(chunk);

            m_Chunks.Clear();

#if UNITY_EDITOR
            if (!Application.isPlaying)
                UnityEditor.EditorApplication.update -= Update;
#endif
        }

        /// <summary>
        /// Called each frame when enabled.
        /// </summary>
        private void Update()
        {
            WorldContainer?.RemeshHandler.FinishTasks();
        }

        /// <summary>
        /// Called when a new chunk is created (or loaded).
        /// </summary>
        /// <param name="ev">The event.</param>
        private void OnChunkCreated(BlockContainerCreatedEvent ev)
        {
            var chunk = ev.BlockContainer;
            m_Chunks.Add(m_ChunkCreator.LoadChunk(chunk.Position, chunk.Size.Value, transform));
        }

        /// <summary>
        /// Called when a chunk is destroyed (or unloaded).
        /// </summary>
        /// <param name="ev">The event.</param>
        private void OnChunkDestroyed(BlockContainerDestroyedEvent ev)
        {
            var chunk = GetChunk(ev.BlockContainer.Position);

            if (chunk == null)
                return;

            m_Chunks.Remove(chunk);
            m_ChunkCreator.DestroyChunk(chunk);
        }

        /// <summary>
        /// Called when a chunk finishes a remesh event.
        /// </summary>
        /// <param name="ev">The event.</param>
        private void OnRemeshFinished(RemeshFinishEvent ev)
        {
            var chunk = GetChunk(ev.Report.ChunkPosition);
            m_MeshBuilder.RefreshMeshes(chunk, ev, m_TextureAtlasList);
        }

        /// <summary>
        /// Gets the chunk at the given chunk position.
        /// </summary>
        /// <param name="chunkPos">The chunk position.</param>
        /// <returns>The Block Chunk.</returns>
        private BlockChunk GetChunk(ChunkPosition chunkPos)
        {
            foreach (var chunk in m_Chunks)
                if (chunk.Position.Equals(chunkPos))
                    return chunk;

            return null;
        }
    }
}
