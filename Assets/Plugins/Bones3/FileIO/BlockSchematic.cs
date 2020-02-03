using UnityEngine;
using System.Collections;
using WraithavenGames.Bones3;

namespace WraithavenGames.Bones3.FileIO
{
    public class BlockSchematic : ScriptableObject
    {
        [SerializeField] private ushort[] blocks;
        [SerializeField] private Material[] materials;
        [SerializeField] private int sizeX;
        [SerializeField] private int sizeY;
        [SerializeField] private int sizeZ;

        public void UploadSchematic(BlockWorld world, BlockLocation block1, BlockLocation block2)
        {
            int minX = Mathf.Min(block1.x, block2.x);
            int minY = Mathf.Min(block1.y, block2.y);
            int minZ = Mathf.Min(block1.z, block2.z);
            int maxX = Mathf.Min(block1.x, block2.x);
            int maxY = Mathf.Min(block1.y, block2.y);
            int maxZ = Mathf.Min(block1.z, block2.z);

            int minChunkX = minX >> 4;
            int minChunkY = minY >> 4;
            int minChunkZ = minZ >> 4;
            int maxChunkX = maxX >> 4;
            int maxChunkY = maxY >> 4;
            int maxChunkZ = maxZ >> 4;

            sizeX = maxX - minX + 1;
            sizeY = maxY - minY + 1;
            sizeZ = maxZ - minZ + 1;

            blocks = new ushort[sizeX * sizeY * sizeZ];

            Chunk chunk;
            int a, b, c, x, y, z;
            int minX2, minY2, minZ2;
            int maxX2, maxY2, maxZ2;
            for (a = minChunkX; a <= maxChunkX; a++)
                for (b = minChunkY; b <= maxChunkY; b++)
                    for (c = minChunkZ; c <= maxChunkZ; c++)
                    {
                        chunk = world.GetChunkByCoords(a, b, c, false);
                        if (chunk == null)
                            continue;

                        minX2 = Mathf.Max(minX, chunk.chunkX * 16);
                        minY2 = Mathf.Max(minY, chunk.chunkY * 16);
                        minZ2 = Mathf.Max(minZ, chunk.chunkZ * 16);
                        maxX2 = Mathf.Min(maxX, chunk.chunkX * 16 + 15);
                        maxY2 = Mathf.Min(maxY, chunk.chunkY * 16 + 15);
                        maxZ2 = Mathf.Min(maxZ, chunk.chunkZ * 16 + 15);

                        for (x = minX2; x <= maxX2; x++)
                            for (y = minY2; y <= maxY2; y++)
                                for (z = minZ2; z <= maxZ2; z++)
                                    blocks[(x - minX) * sizeY * sizeZ + (y - minY) * sizeZ + (z - minZ)]
                                        = chunk.GetBlockId(x & 15, y & 15, z & 15);
                    }

            ArrayList materialList = new ArrayList();
            for (int i = 0; i < blocks.Length; i++)
            {
                if (blocks[i] == 0)
                    continue;

                if (!materialList.Contains(blocks[i]))
                    materialList.Add(blocks[i]);
                blocks[i] = (ushort)(materialList.IndexOf(blocks[i]) + 1);
            }

            materials = new Material[materialList.Count];
            for (int i = 0; i < materials.Length; i++)
                materials[i] = materialList[i] as Material;
        }

        public void DownloadSchematic(BlockWorld world, BlockLocation minEdge)
        {
            if (blocks == null || materials == null || world == null)
                return;

            int minX = minEdge.x;
            int minY = minEdge.y;
            int minZ = minEdge.z;
            int maxX = minEdge.x + sizeX - 1;
            int maxY = minEdge.y + sizeY - 1;
            int maxZ = minEdge.z + sizeZ - 1;

            int minChunkX = minX >> 4;
            int minChunkY = minY >> 4;
            int minChunkZ = minZ >> 4;
            int maxChunkX = maxX >> 4;
            int maxChunkY = maxY >> 4;
            int maxChunkZ = maxZ >> 4;

            Chunk chunk;
            int a, b, c, x, y, z;
            int minX2, minY2, minZ2;
            int maxX2, maxY2, maxZ2;
            int index;
            for (a = minChunkX; a <= maxChunkX; a++)
                for (b = minChunkY; b <= maxChunkY; b++)
                    for (c = minChunkZ; c <= maxChunkZ; c++)
                    {
                        chunk = world.GetChunkByCoords(a, b, c, true);

                        minX2 = Mathf.Max(minX, chunk.chunkX * 16);
                        minY2 = Mathf.Max(minY, chunk.chunkY * 16);
                        minZ2 = Mathf.Max(minZ, chunk.chunkZ * 16);
                        maxX2 = Mathf.Min(maxX, chunk.chunkX * 16 + 15);
                        maxY2 = Mathf.Min(maxY, chunk.chunkY * 16 + 15);
                        maxZ2 = Mathf.Min(maxZ, chunk.chunkZ * 16 + 15);

                        for (x = minX2; x <= maxX2; x++)
                            for (y = minY2; y <= maxY2; y++)
                                for (z = minZ2; z <= maxZ2; z++)
                                {
                                    index = (x - minX) * sizeY * sizeZ + (y - minY) * sizeZ + (z - minZ);
                                    chunk.SetBlock(x & 15, y & 15, z & 15, blocks[index] == 0 ? null : materials[blocks[index] - 1]);
                                }
                    }

            if (world.autoRemesh)
                world.UpdateAllChunks();
        }
    }
}
