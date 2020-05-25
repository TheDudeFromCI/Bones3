using System.Collections.Generic;

namespace Bones3Rebuilt.Remeshing.Voxel
{
    /// <summary>
    /// Generates the basic voxel visual and collision chunk meshes.
    /// </summary>
    public class StandardDistributor : IRemeshDistributor
    {
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
            List<int> materials = new List<int>();

            foreach (var pos in BlockIterator(properties.ChunkSize.Value))
            {
                var type = properties.GetBlock(pos);

                if (!type.IsVisible)
                    continue;

                for (int j = 0; j < 6; j++)
                {
                    var material = type.GetMaterialID(j);
                    if (materials.Contains(material))
                        continue;

                    materials.Add(material);
                    taskStack.AddTask(new VisualRemeshTask(properties, material));
                }
            }
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
                    taskStack.AddTask(new CollisionRemeshTask(properties));
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
    }
}
