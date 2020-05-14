using System.Collections.Generic;

using UnityEngine;

namespace Bones3Rebuilt
{
    /// <summary>
    /// The main behaviour for containing a voxel block world.
    /// </summary>
    [SelectionBase]
    public class BlockWorld : MonoBehaviour
    {
        private readonly List<BlockChunk> m_Chunks = new List<BlockChunk>();
        private readonly ChunkMeshBuilder m_MeshBuilder = new ChunkMeshBuilder();
        private readonly ChunkCreator m_ChunkCreator = new ChunkCreator();

        /// <summary>
        /// Gets the world container being managed.
        /// </summary>
        /// <value>The world container, for editing the world.</value>
        public WorldContainer WorldContainer { get; private set; }

        private void OnEnable()
        {
            var chunkSize = new GridSize(4);

            var world = new World(chunkSize);
            var blockList = new BlockTypeList();
            var remesh = new RemeshHandler();
            var database = new WorldDatabase("/home/thedudefromci/Bones3/TestWorld");

            remesh.AddDistributor(new StandardDistributor());

            WorldContainer = new WorldContainer(world, remesh, blockList, database);
            WorldContainer.BlockContainerProvider.OnBlockContainerCreated += OnChunkCreated;
            WorldContainer.BlockContainerProvider.OnBlockContainerDestroyed += OnChunkDestroyed;
            WorldContainer.RemeshHandler.OnRemeshFinish += OnRemeshFinished;
        }

        private void OnDisable()
        {
            WorldContainer.BlockContainerProvider.OnBlockContainerCreated -= OnChunkCreated;
            WorldContainer.BlockContainerProvider.OnBlockContainerDestroyed -= OnChunkDestroyed;
            WorldContainer = null;

            foreach (var chunk in m_Chunks)
                m_ChunkCreator.DestroyChunk(chunk);

            m_Chunks.Clear();
        }

        private void Update()
        {
            WorldContainer?.RemeshHandler.FinishTasks();
        }

        private void OnChunkCreated(BlockContainerCreatedEvent ev)
        {
            var chunk = ev.BlockContainer;
            m_Chunks.Add(m_ChunkCreator.LoadChunk(chunk.Position, chunk.Size.Value, transform));
        }

        private void OnChunkDestroyed(BlockContainerDestroyedEvent ev)
        {
            var chunk = GetChunk(ev.BlockContainer.Position);

            if (chunk == null)
                return;

            m_Chunks.Remove(chunk);
            m_ChunkCreator.DestroyChunk(chunk);
        }

        private void OnRemeshFinished(RemeshFinishEvent ev)
        {
            var chunk = GetChunk(ev.Report.ChunkPosition);
            var visualMesh = ev.Report.VisualMesh;
            var collisionMesh = ev.Report.CollisionMesh;

            m_MeshBuilder.RefreshMeshes(chunk, visualMesh, collisionMesh);
        }

        private BlockChunk GetChunk(ChunkPosition chunkPos)
        {
            foreach (var chunk in m_Chunks)
                if (chunk.Position.Equals(chunkPos))
                    return chunk;

            return null;
        }
    }
}
