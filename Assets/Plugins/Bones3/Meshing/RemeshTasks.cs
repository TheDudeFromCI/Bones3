using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using WraithavenGames.Bones3;
using WraithavenGames.Bones3.BlockProperties;
using System.Collections.Generic;

namespace WraithavenGames.Bones3.Meshing
{
    public interface IRemeshTask
    {
        void Complete();

        void Dispose();
    }

    public interface IQuadCollectionTask
    {
        NativeArray<byte> GetBlockFaces();

        JobHandle GetJob();
    }

    public class CollectBlocksTask : IRemeshTask
    {
        private NativeArray<ushort> blocks;
        private NativeArray<ushort> nearbyBlocks;
        private NativeArray<BlockID> blockProperties;
        private List<ushort> blockRef;

        public CollectBlocksTask()
        {
            blocks = new NativeArray<ushort>(16 * 16 * 16, Allocator.Persistent);
            nearbyBlocks = new NativeArray<ushort>(16 * 16 * 6, Allocator.Persistent);
            blockProperties = new NativeArray<BlockID>(16 * 16 * 16 * 6, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            blockRef = new List<ushort>();

            for (int i = 0; i < blockProperties.Length; i++)
                blockProperties[i] = BlockID.AIR;
        }

        public void Schedule(Chunk chunk)
        {
            ClearBlockProperties();
            CollectBlocks(chunk);
            CollectNearbyBlocks(chunk);
            CollectBlockProperties(chunk);
        }

        private void ClearBlockProperties()
        {
            for (int i = 0; i < blockRef.Count; i++)
                blockProperties[i] = BlockID.AIR;
            blockRef.Clear();
        }

        private void CollectBlocks(Chunk chunk)
        {
            for (int i = 0; i < 4096; i++)
            {
                blocks[i] = chunk.GetBlockID(i);

                if (!blockRef.Contains(blocks[i]))
                    blockRef.Add(blocks[i]);
            }
        }

        private void CollectNearbyBlocks(Chunk chunk)
        {
            for (int j = 0; j < 6; j++)
            {
                Chunk near = chunk.GetNearbyChunk(j);
                if (near == null)
                {
                    if (!blockRef.Contains(0))
                        blockRef.Add(0);

                    continue;
                }

                int offset = j * 16 * 16;
                for (int a = 0; a < 16; a++)
                {
                    for (int b = 0; b < 16; b++)
                    {
                        int index = a * 16 + b + offset;
                        switch (j)
                        {
                            case 0:
                            case 1:
                                nearbyBlocks[index] = near.GetBlockId(j % 2 == 0 ? 0 : 15, a, b);
                                break;

                            case 2:
                            case 3:
                                nearbyBlocks[index] = near.GetBlockId(a, j % 2 == 0 ? 0 : 15, b);
                                break;

                            case 4:
                            case 5:
                                nearbyBlocks[index] = near.GetBlockId(a, b, j % 2 == 0 ? 0 : 15);
                                break;
                        }

                        if (!blockRef.Contains(nearbyBlocks[index]))
                            blockRef.Add(nearbyBlocks[index]);
                    }
                }
            }
        }

        private void CollectBlockProperties(Chunk chunk)
        {
            for (int i = 0; i < blockRef.Count; i++)
            {
                MaterialBlock blockState = chunk.GetBlockState(blockRef[i]);

                if (blockState != null)
                {
                    BlockID block = new BlockID();
                    block.id = blockRef[i];

                    block.hasCollision = (byte)(block.id > 0 ? 1 : 0);
                    block.transparent = (byte)(blockState.Transparent ? 1 : 0);
                    block.viewInsides = (byte)(blockState.ViewInsides ? 1 : 0);
                    block.depthSort = (byte)(blockState.DepthSort ? 1 : 0);

                    blockProperties[i] = block;

                    UnityEngine.Debug.Log($"Loaded Block: {block.id}, Col={block.hasCollision}");
                }
                else
                    UnityEngine.Debug.Log($"Failed to Load Block: {blockRef[i]}");
            }
        }

        public void Dispose()
        {
            blocks.Dispose();
            nearbyBlocks.Dispose();
            blockProperties.Dispose();
        }

        public void Complete()
        {
            // Nothing to do.
        }

        public NativeArray<ushort> GetBlocks()
        {
            return blocks;
        }

        public NativeArray<ushort> GetNearbyBlocks()
        {
            return nearbyBlocks;
        }

        public NativeArray<BlockID> GetBlockProperties()
        {
            return blockProperties;
        }

        public List<ushort> GetBlockTypes()
        {
            return blockRef;
        }
    }

    /// <summary>
    /// This task is used to collect all of the quads within a chunk for a given material type.
    /// </summary>
    public class CollectQuadsTask : IRemeshTask, IQuadCollectionTask
    {
        private NativeArray<byte> blockFaces;
        private JobHandle job;

        public CollectQuadsTask()
        {
            blockFaces = new NativeArray<byte>(16 * 16 * 16 * 6, Allocator.Persistent);
        }

        public void Schedule(CollectBlocksTask task, ushort targetBlock)
        {
            job = new CollectQuads()
            {
                blocks = task.GetBlocks(),
                nearbyBlocks = task.GetNearbyBlocks(),
                blockProperties = task.GetBlockProperties(),
                quads = blockFaces,
                targetBlock = targetBlock
            }.Schedule();
        }

        public JobHandle GetJob()
        {
            return job;
        }

        public void Dispose()
        {
            blockFaces.Dispose();
        }

        public NativeArray<byte> GetBlockFaces()
        {
            return blockFaces;
        }

        public void Complete()
        {
            job.Complete();
        }
    }

    /// <summary>
    /// This task is used to collect all of the collision quads within a chunk.
    /// </summary>
    public class CollectColQuadsTask : IRemeshTask, IQuadCollectionTask
    {
        private NativeArray<byte> blockFaces;
        private JobHandle job;

        public CollectColQuadsTask()
        {
            blockFaces = new NativeArray<byte>(16 * 16 * 16 * 6, Allocator.Persistent);
        }

        public void Schedule(CollectBlocksTask task)
        {
            job = new CollectColQuads()
            {
                blocks = task.GetBlocks(),
                blockProperties = task.GetBlockProperties(),
                quads = blockFaces
            }.Schedule();
        }

        public JobHandle GetJob()
        {
            return job;
        }

        public void Dispose()
        {
            blockFaces.Dispose();
        }

        public NativeArray<byte> GetBlockFaces()
        {
            return blockFaces;
        }

        public void Complete()
        {
            job.Complete();
        }
    }

    public class CombineQuadsTask : IRemeshTask
    {
        private NativeArray<Quad> quads;
        private NativeArray<int> quadCount;
        private NativeArray<byte> storage;
        private JobHandle job;

        private int materialIndex;


        public CombineQuadsTask()
        {
            quads = new NativeArray<Quad>(16 * 16 * 8, Allocator.Persistent);
            quadCount = new NativeArray<int>(1, Allocator.Persistent);
            storage = new NativeArray<byte>(16 * 16, Allocator.Persistent);
        }

        public void Schedule(IQuadCollectionTask task, int materialIndex)
        {
            this.materialIndex = materialIndex;

            job = new CombineQuads()
            {
                quadsIn = task.GetBlockFaces(),
                quadsOut = quads,
                quadCount = quadCount,
                storage = storage
            }.Schedule(task.GetJob());
        }

        public JobHandle GetJob()
        {
            return job;
        }

        public void Dispose()
        {
            quads.Dispose();
            quadCount.Dispose();
            storage.Dispose();
        }

        public void Complete()
        {
            job.Complete();
        }

        public NativeArray<Quad> GetQuads()
        {
            return quads;
        }

        public NativeArray<int> GetQuadCount()
        {
            return quadCount;
        }

        public int GetMaterialIndex()
        {
            return materialIndex;
        }
    }
}