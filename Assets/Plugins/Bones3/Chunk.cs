using UnityEngine;
using System;
using System.Collections;
using WraithavenGames.Bones3.Terrain;
using WraithavenGames.Bones3.ChunkFinding;
using WraithavenGames.Bones3.BlockProperties;
using WraithavenGames.Bones3.Utility;

#if UNITY_EDITOR
using UnityEditor;
#endif

#pragma warning disable 0108

namespace WraithavenGames.Bones3
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(EditorCloneCallback))]
    [DisallowMultipleComponent]
    public class Chunk : MonoBehaviour, IBlockHolder, ICloneCallbackReceiver
    {
        [HideInInspector, SerializeField] public int chunkX;
        [HideInInspector, SerializeField] public int chunkY;
        [HideInInspector, SerializeField] public int chunkZ;
        [HideInInspector, SerializeField] private BlockWorld world;
        [HideInInspector, SerializeField] private Chunk[] nearbyChunks;
        [HideInInspector, SerializeField] private ushort[] blocks = new ushort[16 * 16 * 16];
        [HideInInspector, SerializeField] private byte[] blockStates = new byte[16 * 16 * 16];
        [HideInInspector, SerializeField] private int blockCount = 0;
        [HideInInspector, SerializeField] private bool needsRemesh;
        [HideInInspector, SerializeField] private bool undoMarked;
        [HideInInspector, SerializeField] private int redoMarked;

        public bool EmptyChunk { get { return blockCount == 0; } }
        public bool NeedsRemesh { get { return needsRemesh || undoMarked || redoMarked > 0; } }
        public BlockTypeList BlockTypes { get { return World.BlockTypes; } }

        public BlockWorld World
        {
            get
            {
                if (world == null)
                    world = GetComponentInParent<BlockWorld>();
                return world;
            }
        }

        public ushort[] GetBlocksRaw()
        {
            return blocks;
        }

        public byte[] GetBlockStatesRaw()
        {
            return blockStates;
        }

        public void LoadFromUngenerated(UngeneratedChunk gen)
        {
            blockCount = gen.GetBlockCount();
            Array.Copy(gen.GetBlocks(), blocks, blocks.Length);
            Array.Copy(gen.GetBlockStates(), blockStates, blockStates.Length);
            MarkForRemesh();

            // Update nearby chunks
            {
                Chunk c;

                c = GetNearbyChunk(0);
                if (c != null) c.MarkForRemesh();

                c = GetNearbyChunk(1);
                if (c != null) c.MarkForRemesh();

                c = GetNearbyChunk(2);
                if (c != null) c.MarkForRemesh();

                c = GetNearbyChunk(3);
                if (c != null) c.MarkForRemesh();

                c = GetNearbyChunk(4);
                if (c != null) c.MarkForRemesh();

                c = GetNearbyChunk(5);
                if (c != null) c.MarkForRemesh();
            }
        }

        public bool SetBlock(int x, int y, int z, Material m)
        {
            MaterialBlock matProps = BlockTypes.GetMaterialProperties(m);

            int index = x * 16 * 16 + y * 16 + z;
            ushort type = m == null ? (ushort)0 : matProps.Id;

            if (blocks[index] == type) return false;

            if ((blocks[index] == 0) != (type == 0))
            {
                if (type == 0)
                    blockCount--;
                else
                    blockCount++;
            }

            blocks[index] = type;
            blockStates[index] = m == null ? (byte)0 : matProps.BlockState;

            MarkForRemesh();

            // Update nearby chunks
            {
                Chunk c;
                if (x == 15)
                {
                    c = GetNearbyChunk(0);
                    if (c != null) c.MarkForRemesh();
                }
                if (x == 0)
                {
                    c = GetNearbyChunk(1);
                    if (c != null) c.MarkForRemesh();
                }
                if (y == 15)
                {
                    c = GetNearbyChunk(2);
                    if (c != null) c.MarkForRemesh();
                }
                if (y == 0)
                {
                    c = GetNearbyChunk(3);
                    if (c != null) c.MarkForRemesh();
                }
                if (z == 15)
                {
                    c = GetNearbyChunk(4);
                    if (c != null) c.MarkForRemesh();
                }
                if (z == 0)
                {
                    c = GetNearbyChunk(5);
                    if (c != null) c.MarkForRemesh();
                }
            }

            return true;
        }

        public Chunk GetNearbyChunk(int side)
        {
            if (nearbyChunks == null || nearbyChunks.Length != 6)
                nearbyChunks = new Chunk[6];

            if (nearbyChunks[side] == null)
            {
                int x = chunkX;
                int y = chunkY;
                int z = chunkZ;

                switch (side)
                {
                    case 0:
                        x++;
                        break;
                    case 1:
                        x--;
                        break;
                    case 2:
                        y++;
                        break;
                    case 3:
                        y--;
                        break;
                    case 4:
                        z++;
                        break;
                    case 5:
                        z--;
                        break;
                }

                nearbyChunks[side] = World.GetChunkByCoords(x, y, z, false);
            }

            return nearbyChunks[side];
        }

        public void OnObjectCloned()
        {
            MeshFilter filter = GetComponent<MeshFilter>();
            if (filter.sharedMesh != null)
                filter.sharedMesh = (Mesh)Instantiate(filter.sharedMesh);

            MeshCollider collider = GetComponent<MeshCollider>();
            if (collider.sharedMesh != null)
                collider.sharedMesh = (Mesh)Instantiate(collider.sharedMesh);
        }

        public void MarkForRemesh()
        {
            needsRemesh = true;
        }

        public void MarkRemeshComplete()
        {
            needsRemesh = false;
        }

        public int GetBlockState(int x, int y, int z)
        {
            return blockStates[x * 16 * 16 + y * 16 + z];
        }

        public Material GetBlock(int x, int y, int z)
        {
            int index = x * 16 * 16 + y * 16 + z;
            if (blocks[index] == 0)
                return null;
            return BlockTypes.GetMaterialProperties(blocks[index]).Material;
        }

        public ushort GetBlockID(int index)
        {
            return blocks[index];
        }

        public ushort GetBlockId(int x, int y, int z)
        {
            return blocks[x * 16 * 16 + y * 16 + z];
        }

        public MaterialBlock GetBlockState(int index)
        {
            return BlockTypes.GetMaterialProperties(blocks[index]);
        }

        public void MarkUndo()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                undoMarked = true;
                Undo.RecordObject(this, "Edited voxel chunk.");
                undoMarked = false;
            }
#endif
        }

        public void Regenerate()
        {
            if (!NeedsRemesh)
                return;

            if (needsRemesh)
                redoMarked = 0;
            else if (undoMarked)
                redoMarked++;
            undoMarked = false;
            needsRemesh = false;

            World.RemeshChunk(this);
        }

        public void RebuildBlockStates(ushort block, byte state)
        {
            for (int i = 0; i < blocks.Length; i++)
            {
                if (blocks[i] == block)
                {
                    blockStates[i] = state;
                    MarkForRemesh();
                }
            }
        }
    }
}
