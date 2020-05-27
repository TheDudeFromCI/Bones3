using System.Collections.Generic;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// Generates the basic voxel visual and collision chunk meshes.
    /// </summary>
    internal class StandardDistributor : IRemeshDistributor
    {
        private const int MAX_MESHER_POOL_SIZE = 16;

        private readonly List<GreedyMesher> m_GreedyMesherPool = new List<GreedyMesher>();
        private readonly GridSize m_ChunkSize;
        private bool[] m_MaterialBuffer = new bool[128];

        internal StandardDistributor(GridSize chunkSize)
        {
            m_ChunkSize = chunkSize;
        }

        /// <inheritdoc cref="IRemeshTask"/>
        public void CreateTasks(ChunkProperties properties, RemeshTaskStack taskStack)
        {
            GenerateVisuals(properties, taskStack);
            GenerateCollision(properties, taskStack);
        }

        /// <summary>
        /// Generates the visual remeshing tasks, as needed.
        /// </summary>
        /// <param name="properties">The chunk properties.</param>
        /// <param name="tasks">The task list to add to.</param>
        private void GenerateVisuals(ChunkProperties properties, RemeshTaskStack taskStack)
        {
            ResetMaterialBuffer();

            foreach (var pos in BlockIterator(properties.ChunkSize.Value))
            {
                var type = properties.GetBlock(pos);

                if (!type.IsVisible)
                    continue;

                for (int j = 0; j < 6; j++)
                {
                    var material = type.GetMaterialID(j);
                    if (ContainsMaterial(material))
                        continue;

                    m_MaterialBuffer[material] = true;
                    taskStack.AddTask(new VisualRemeshTask(properties, material, PullMesher()));
                }
            }
        }

        /// <summary>
        /// Resets all material IDs in the material buffer.
        /// </summary>
        private void ResetMaterialBuffer()
        {
            for (int i = 0; i < m_MaterialBuffer.Length; i++)
                m_MaterialBuffer[i] = false;
        }

        /// <summary>
        /// Checks if the material is currently in the material buffer.
        /// </summary>
        /// <param name="materialID">The material ID.</param>
        /// <returns>True if the material was already handled, false otherwise.</returns>
        private bool ContainsMaterial(int materialID)
        {
            if (materialID >= m_MaterialBuffer.Length)
            {
                var newBuffer = new bool[materialID + 1];
                System.Array.Copy(m_MaterialBuffer, newBuffer, m_MaterialBuffer.Length);
                m_MaterialBuffer = newBuffer;
            }

            return m_MaterialBuffer[materialID];
        }

        /// <summary>
        /// Generates the collision remesh task, as needed.
        /// </summary>
        /// <param name="properties">The chunk properties.</param>
        /// <param name="tasks">The task list to add to.</param>
        private void GenerateCollision(ChunkProperties properties, RemeshTaskStack taskStack)
        {
            foreach (var pos in BlockIterator(properties.ChunkSize.Value))
            {
                var type = properties.GetBlock(pos);

                if (type.IsSolid)
                {
                    taskStack.AddTask(new CollisionRemeshTask(properties, PullMesher()));
                    return;
                }
            }
        }

        /// <summary>
        /// Iterates over all block positions within a chunk bounds.
        /// </summary>
        /// <param name="chunkSize">The size of the chunk.</param>
        /// <returns>The block position iterator.</returns>
        private IEnumerable<BlockPosition> BlockIterator(int chunkSize)
        {
            for (int x = 0; x < chunkSize; x++)
                for (int y = 0; y < chunkSize; y++)
                    for (int z = 0; z < chunkSize; z++)
                        yield return new BlockPosition(x, y, z);
        }

        /// <summary>
        /// Pulls a greedy mesher instance from the object pool.
        /// </summary>
        /// <returns>The greedy mesher.</returns>
        private GreedyMesher PullMesher()
        {
            if (m_GreedyMesherPool.Count > 0)
            {
                var index = m_GreedyMesherPool.Count - 1;
                var mesher = m_GreedyMesherPool[index];
                m_GreedyMesherPool.RemoveAt(index);
                return mesher;
            }

            return new GreedyMesher(m_ChunkSize, this);
        }

        /// <summary>
        /// Returns a greedy mesher back to the object pool.
        /// </summary>
        /// <param name="mesher">The mesher to return.</param>
        internal void ReturnMesher(GreedyMesher mesher)
        {
            if (m_GreedyMesherPool.Count >= MAX_MESHER_POOL_SIZE)
                return;

            m_GreedyMesherPool.Add(mesher);
        }
    }
}
