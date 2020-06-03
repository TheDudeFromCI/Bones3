using System.Collections.Generic;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// Generates the basic voxel visual and collision chunk meshes.
    /// </summary>
    internal class StandardDistributor : IRemeshDistributor
    {
        // TODO Move GreedyMesher pool to own class.
        private const int MAX_MESHER_POOL_SIZE = 16;

        private readonly List<GreedyMesher> m_GreedyMesherPool = new List<GreedyMesher>();
        private readonly GridSize m_ChunkSize;
        private readonly ServerBlockList m_BlockList;
        private bool[] m_MaterialBuffer = new bool[1024];

        internal StandardDistributor(GridSize chunkSize, ServerBlockList blockList)
        {
            m_ChunkSize = chunkSize;
            m_BlockList = blockList;
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
            PrepareMaterialBuffer();
            VisualBlockIterator(properties, taskStack);
            ResetMaterialBuffer();
        }

        /// <summary>
        /// Iterates over all blocks in the chunk, creating visual tasks as needed.
        /// </summary>
        /// <param name="properties">The chunk properties.</param>
        /// <param name="tasks">The task list to add to.</param>
        private void VisualBlockIterator(ChunkProperties properties, RemeshTaskStack taskStack)
        {
            var volume = m_ChunkSize.Volume;
            var blocks = properties.Blocks;

            for (int i = 0; i < volume; i++)
            {
                var type = blocks[i];

                if (!type.Visible)
                    continue;

                // TODO Make sure block type is a cube

                for (int j = 0; j < type.FaceCount; j++)
                {
                    var material = type.Face(j).MaterialID;
                    if (m_MaterialBuffer[material])
                        continue;

                    m_MaterialBuffer[material] = true;
                    taskStack.AddTask(new VisualRemeshTask(properties, material, PullMesher()));
                }
            }
        }

        /// <summary>
        /// Ensure the material buffer has a large enough capacity.
        /// </summary>
        private void PrepareMaterialBuffer()
        {
            // This feels like a bad approach. Might look back into refactoring this, later.

            int materialCount = 0;
            for (ushort i = 0; i < m_BlockList.Capacity; i++)
            {
                var block = m_BlockList.GetBlockType(i);
                if (!block.Visible)
                    continue;

                var faces = UnityEngine.Mathf.Min(block.FaceCount, 6);
                for (int j = 0; j < faces; j++)
                    materialCount = UnityEngine.Mathf.Min(materialCount, block.Face(j).MaterialID);
            }

            if (materialCount >= m_MaterialBuffer.Length)
            {
                var newBuffer = new bool[materialCount];
                System.Array.Copy(m_MaterialBuffer, newBuffer, m_MaterialBuffer.Length);
                m_MaterialBuffer = newBuffer;
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
        /// Generates the collision remesh task, as needed.
        /// </summary>
        /// <param name="properties">The chunk properties.</param>
        /// <param name="tasks">The task list to add to.</param>
        private void GenerateCollision(ChunkProperties properties, RemeshTaskStack taskStack)
        {
            int volume = m_ChunkSize.Volume;
            for (int i = 0; i < volume; i++)
            {
                var type = properties.Blocks[i];

                if (type.Solid)
                {
                    taskStack.AddTask(new CollisionRemeshTask(properties, PullMesher()));
                    return;
                }
            }
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
