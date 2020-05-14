using System.Collections.Generic;

namespace Bones3Rebuilt
{
    /// <summary>
    /// Generates the basic voxel visual and collision chunk meshes.
    /// </summary>
    public class StandardDistributor : IRemeshDistributor
    {
        /// <inheritdoc cref="IRemeshTask"/>
        public void CreateTasks(IChunkProperties properties, RemeshTaskStack taskStack)
        {
            GenerateVisuals(properties, taskStack);
            GenerateCollision(properties, taskStack);
        }

        /// <summary>
        /// Generates the visual remeshing tasks, as needed.
        /// </summary>
        /// <param name="properties">The chunk properties.</param>
        /// <param name="tasks">The task list to add to.</param>
        private void GenerateVisuals(IChunkProperties properties, RemeshTaskStack taskStack)
        {
            List<int> materials = new List<int>();

            foreach (var pos in BlockIterator(properties.ChunkSize.Value))
            {
                var type = properties.GetBlock(pos);

                if (type.IsVisible)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        var face = type.Face(j);
                        var atlas = face.TextureAtlas;

                        if (materials.Contains(atlas))
                            continue;

                        materials.Add(atlas);
                        taskStack.AddVisualTask(new VisualRemeshTask(properties, atlas));
                    }
                }
            }
        }

        /// <summary>
        /// Generates the collision remeshing task, as needed.
        /// </summary>
        /// <param name="properties">The chunk properties.</param>
        /// <param name="tasks">The task list to add to.</param>
        private void GenerateCollision(IChunkProperties properties, RemeshTaskStack taskStack)
        {
            foreach (var pos in BlockIterator(properties.ChunkSize.Value))
            {
                var type = properties.GetBlock(pos);

                if (type.IsSolid)
                {
                    taskStack.AddCollisionTask(new CollisionRemeshTask(properties));
                    return;
                }
            }
        }

        /// <summary>
        /// Iterates over all block positions within a chunk bounds.
        /// </summary>
        /// <param name="chunkSize">The size of the chunk.</param>
        /// <returns>The block position iterator.</returns>
        IEnumerable<BlockPosition> BlockIterator(int chunkSize)
        {
            for (int x = 0; x < chunkSize; x++)
                for (int y = 0; y < chunkSize; y++)
                    for (int z = 0; z < chunkSize; z++)
                        yield return new BlockPosition(x, y, z);
        }
    }
}
