using UnityEngine;
using System.Collections;
using WraithavenGames.Bones3.Filter;
using WraithavenGames.Bones3.Terrain;
using WraithavenGames.Bones3.Meshing;
using WraithavenGames.Bones3.ChunkFinding;
using WraithavenGames.Bones3.BlockProperties;
using WraithavenGames.Bones3.Utility;
using WraithavenGames.Bones3.SceneEditing;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WraithavenGames.Bones3
{
    [RequireComponent(typeof(EditorCloneCallback))]
    [SelectionBase, DisallowMultipleComponent]
    public class BlockWorld : MonoBehaviour, ISerializationCallbackReceiver, ICloneCallbackReceiver
    {
        public bool autoRemesh = true;
        public int selectedMaterialIndex;
        public SelectedBlock selectedBlock;
        public SelectedBlock dragStart;
        public BlockTypeList blockTypes;
        public ArrayList blockGroups = new ArrayList();

        private ChunkRemesher remesher = new ChunkRemesher();

        public BlockTypeList BlockTypes
        {
            get
            {
                if (blockTypes == null)
                    blockTypes = ScriptableObject.CreateInstance<BlockTypeList>();
                return blockTypes;
            }
        }

        #region SerializationHandling
        [SerializeField] private BlockGroup[] blockGroupArray;

        public void EnsureFullyGenerated()
        {
            if (blockTypes == null)
                blockTypes = ScriptableObject.CreateInstance<BlockTypeList>();
        }

        public void OnBeforeSerialize()
        {
            lock (blockGroups)
            {
                blockGroupArray = new BlockGroup[blockGroups.Count];
                for (int i = 0; i < blockGroupArray.Length; i++)
                {
                    blockGroupArray[i] = blockGroups[i] as BlockGroup;
                    blockGroupArray[i].Serialize();
                }
            }
        }

        public void OnAfterDeserialize()
        {
            lock (blockGroups)
            {
                blockGroups.Clear();

                if (blockGroupArray != null)
                {
                    blockGroups.Capacity = blockGroupArray.Length;
                    for (int i = 0; i < blockGroupArray.Length; i++)
                    {
                        blockGroups.Add(blockGroupArray[i]);
                        blockGroupArray[i].Deserialize();
                    }
                    blockGroupArray = null;
                }
            }
        }

        public void OnObjectCloned()
        {
            // Clone block types
            blockTypes = BlockTypes.Copy();
        }
        #endregion

        #region BlockEditing
        public void SetBlock(int x, int y, int z, Material material)
        {
            // Get the chunk in question, creating chunks if required
            Chunk chunk = GetChunkByBlock(x, y, z, material != null);
            if (chunk != null)
            {
                chunk.MarkUndo();

                // Update the chunk
                int a = x & 15;
                int b = y & 15;
                int c = z & 15;
                bool placedBlock = chunk.SetBlock(a, b, c, material);
                if (!placedBlock)
                    return;

                // If auto remeshing is enabled, rebuild the chunk quickly
                if (autoRemesh)
                {
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        Mesh sharedMesh = chunk.GetComponent<MeshFilter>().sharedMesh;
                        if (sharedMesh != null)
                        {
                            Undo.RecordObject(sharedMesh, "Edited chunk mesh.");
                            Undo.RecordObject(chunk.GetComponent<MeshCollider>().sharedMesh, "Edited chunk collision.");
                        }
                    }
#endif

                    chunk.Regenerate();

                    // Update nearby chunks
                    {
                        Chunk chunk2;
                        if (a == 15)
                        {
                            chunk2 = chunk.GetNearbyChunk(0);
                            if (chunk2 != null) chunk2.Regenerate();
                        }
                        if (a == 0)
                        {
                            chunk2 = chunk.GetNearbyChunk(1);
                            if (chunk2 != null) chunk2.Regenerate();
                        }
                        if (b == 15)
                        {
                            chunk2 = chunk.GetNearbyChunk(2);
                            if (chunk2 != null) chunk2.Regenerate();
                        }
                        if (b == 0)
                        {
                            chunk2 = chunk.GetNearbyChunk(3);
                            if (chunk2 != null) chunk2.Regenerate();
                        }
                        if (c == 15)
                        {
                            chunk2 = chunk.GetNearbyChunk(4);
                            if (chunk2 != null) chunk2.Regenerate();
                        }
                        if (c == 0)
                        {
                            chunk2 = chunk.GetNearbyChunk(5);
                            if (chunk2 != null) chunk2.Regenerate();
                        }
                    }
                }
            }
        }

        public void SetArea(int x1, int y1, int z1, int x2, int y2, int z2, Material material, IFilter filter = null, bool paintMode = false)
        {
            bool create = material != null;

            int chunkX1 = Mathf.Min(x1 >> 4, x2 >> 4);
            int chunkY1 = Mathf.Min(y1 >> 4, y2 >> 4);
            int chunkZ1 = Mathf.Min(z1 >> 4, z2 >> 4);
            int chunkX2 = Mathf.Max(x1 >> 4, x2 >> 4);
            int chunkY2 = Mathf.Max(y1 >> 4, y2 >> 4);
            int chunkZ2 = Mathf.Max(z1 >> 4, z2 >> 4);

            int blockX1 = Mathf.Min(x1, x2);
            int blockY1 = Mathf.Min(y1, y2);
            int blockZ1 = Mathf.Min(z1, z2);
            int blockX2 = Mathf.Max(x1, x2);
            int blockY2 = Mathf.Max(y1, y2);
            int blockZ2 = Mathf.Max(z1, z2);

            if (filter != null)
                filter.InitalizeFilter(new BlockLocation(blockX1, blockY1, blockZ1),
                    new BlockLocation(blockX2, blockY2, blockZ2), material);

            for (int a = chunkX1; a <= chunkX2; a++)
                for (int b = chunkY1; b <= chunkY2; b++)
                    for (int c = chunkZ1; c <= chunkZ2; c++)
                    {
                        Chunk chunk = GetChunkByCoords(a, b, c, create);

                        if (chunk == null)
                            continue;

                        chunk.MarkUndo();

                        for (int a2 = blockX1; a2 <= blockX2; a2++)
                            for (int b2 = blockY1; b2 <= blockY2; b2++)
                                for (int c2 = blockZ1; c2 <= blockZ2; c2++)
                                {
                                    if (a2 >> 4 != a) continue;
                                    if (b2 >> 4 != b) continue;
                                    if (c2 >> 4 != c) continue;

                                    Material cur = chunk.GetBlock(a2 & 15, b2 & 15, c2 & 15);

                                    if (paintMode && cur == null)
                                        continue;

                                    if (filter != null && !filter.CanPlaceBlock(cur, new BlockLocation(a2, b2, c2)))
                                        continue;

                                    chunk.SetBlock(a2 & 15, b2 & 15, c2 & 15, material);
                                }
                    }

            if (autoRemesh)
                UpdateAllChunks();
        }

        public void RemeshChunk(Chunk chunk)
        {
            remesher.Remesh(chunk);
        }
        #endregion

        #region MassChunkUpdates
        public void Clear()
        {
            foreach (BlockGroup group in blockGroups)
                group.Clear();

#if UNITY_EDITOR
            if (Application.isPlaying)
                Resources.UnloadUnusedAssets();
            else
                EditorUtility.UnloadUnusedAssetsImmediate();
#else
            Resources.UnloadUnusedAssets();
#endif
        }

        public void UpdateAllChunks()
        {
            foreach (BlockGroup group in blockGroups)
                group.UpdateAllChunks();
        }

        public void RemeshAllChunks()
        {
            foreach (BlockGroup group in blockGroups)
                group.RemeshAllChunks();
        }

        public void UpdateAllBlockStates(ushort block, byte state)
        {
            foreach (BlockGroup group in blockGroups)
            {
                group.UpdateAllBlockStates(block, state);
                group.UpdateAllChunks();
            }
        }

        public void UnloadDistantChunks(int chunkX, int chunkY, int chunkZ, int radius)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif

            for (int i = 0; i < blockGroups.Count; i++)
            {
                BlockGroup g = blockGroups[i] as BlockGroup;
                g.UnloadDistantChunks(chunkX, chunkY, chunkZ, radius);

                if (g.IsEmpty())
                    blockGroups.RemoveAt(i--);
            }
        }
        #endregion

        #region ChunkFinding
        public Chunk GetChunkByBlock(int x, int y, int z, bool create)
        {
            int chunkX = x >> 4;
            int chunkY = y >> 4;
            int chunkZ = z >> 4;

            return GetChunkByCoords(chunkX, chunkY, chunkZ, create);
        }

        public Chunk GetChunkByCoords(int chunkX, int chunkY, int chunkZ, bool create)
        {
            BlockGroup group = GetBlockGroup(chunkX, chunkY, chunkZ, create);

            if (group == null)
                return null;

            return group.GetChunk(chunkX, chunkY, chunkZ, create);
        }

        private BlockGroup GetBlockGroup(int x, int y, int z, bool create)
        {
            const int zoom = 5;

            x &= ~((1 << zoom) - 1);
            y &= ~((1 << zoom) - 1);
            z &= ~((1 << zoom) - 1);

            foreach (BlockGroup group in blockGroups)
                if (group.X == x && group.Y == y && group.Z == z)
                    return group;

            if (!create)
                return null;

            BlockGroup g = new BlockGroup(zoom, x, y, z, this);
            blockGroups.Add(g);

            return g;
        }

        public void SetChunk(UngeneratedChunk gen)
        {
            Chunk chunk = GetChunkByCoords(gen.chunkX, gen.chunkY, gen.chunkZ, true);
            chunk.LoadFromUngenerated(gen);

            if (autoRemesh)
            {
                chunk.Regenerate();

                Chunk c;

                c = chunk.GetNearbyChunk(0);
                if (c != null) c.Regenerate();

                c = chunk.GetNearbyChunk(1);
                if (c != null) c.Regenerate();

                c = chunk.GetNearbyChunk(2);
                if (c != null) c.Regenerate();

                c = chunk.GetNearbyChunk(3);
                if (c != null) c.Regenerate();

                c = chunk.GetNearbyChunk(4);
                if (c != null) c.Regenerate();

                c = chunk.GetNearbyChunk(5);
                if (c != null) c.Regenerate();
            }
        }
        #endregion

        #region Gizmos
        private void PaintModeGizmos(Vector3 center, Vector3 size)
        {
            Gizmos.color = Color.cyan;
            size += Vector3.one * .02f;
            Gizmos.DrawWireCube(center, size);
        }

        private void BoxShapeGizmos(Vector3 center, Vector3 size, bool eraser)
        {
            if (eraser)
            {
                Gizmos.color = new Color(1f, 0f, 0f, .5f);
                size += Vector3.one * .02f;
                Gizmos.DrawCube(center, size);
            }
            else
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(center, size);
            }
        }

        private void WallShapeGizmos(Vector3 center, Vector3 size, bool eraser)
        {
            if (eraser)
            {
                Gizmos.color = new Color(1f, 0f, 0f, .5f);

                if (dragStart.hasSelectedBlock)
                {
                    Gizmos.DrawCube(new Vector3(
                            (dragStart.xOn + selectedBlock.xOn) / 2f,
                            (dragStart.yOn + selectedBlock.yOn) / 2f,
                            dragStart.zOn
                        ) + Vector3.one * .5f, new Vector3(
                            Mathf.Abs(dragStart.xOn - selectedBlock.xOn) + 1f,
                            Mathf.Abs(dragStart.yOn - selectedBlock.yOn) + 1f,
                            1f
                        ) + Vector3.one * .02f);
                    Gizmos.DrawCube(new Vector3(
                            dragStart.xOn,
                            (dragStart.yOn + selectedBlock.yOn) / 2f,
                            (dragStart.zOn + selectedBlock.zOn) / 2f
                        ) + Vector3.one * .5f, new Vector3(
                            1f,
                            Mathf.Abs(dragStart.yOn - selectedBlock.yOn) + 1f,
                            Mathf.Abs(dragStart.zOn - selectedBlock.zOn) + 1f
                        ) + Vector3.one * .02f);
                    Gizmos.DrawCube(new Vector3(
                            (dragStart.xOn + selectedBlock.xOn) / 2f,
                            (dragStart.yOn + selectedBlock.yOn) / 2f,
                            selectedBlock.zOn
                        ) + Vector3.one * .5f, new Vector3(
                            Mathf.Abs(dragStart.xOn - selectedBlock.xOn) + 1f,
                            Mathf.Abs(dragStart.yOn - selectedBlock.yOn) + 1f,
                            1f
                        ) + Vector3.one * .02f);
                    Gizmos.DrawCube(new Vector3(
                            selectedBlock.xOn,
                            (dragStart.yOn + selectedBlock.yOn) / 2f,
                            (dragStart.zOn + selectedBlock.zOn) / 2f
                        ) + Vector3.one * .5f, new Vector3(
                            1f,
                            Mathf.Abs(dragStart.yOn - selectedBlock.yOn) + 1f,
                            Mathf.Abs(dragStart.zOn - selectedBlock.zOn) + 1f
                        ) + Vector3.one * .02f);
                }
                else
                {
                    size += Vector3.one * .02f;
                    Gizmos.DrawCube(center, Vector3.one * 1.02f);
                }
            }
            else
            {
                Vector3 innerSize = new Vector3(2f, 0f, 2f);
                if (size.x <= 2f) innerSize.x = 0f;
                if (size.z <= 2f) innerSize.z = 0f;

                Gizmos.DrawWireCube(center, size);
                Gizmos.DrawWireCube(center, size - innerSize);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (ToolBelt.Mode == ToolBelt.DO_NOTHING_MODE)
                return;

            if (selectedBlock.hasSelectedBlock)
            {
                Gizmos.matrix = transform.localToWorldMatrix;

                Vector3 center = new Vector3();
                Vector3 size = new Vector3();

                if (ToolBelt.Mode == ToolBelt.PAINT_MODE || ToolBelt.ClearSelectionMode)
                {
                    if (dragStart.hasSelectedBlock)
                    {
                        center.x = (dragStart.xOn + selectedBlock.xOn) / 2f;
                        center.y = (dragStart.yOn + selectedBlock.yOn) / 2f;
                        center.z = (dragStart.zOn + selectedBlock.zOn) / 2f;
                        center += Vector3.one * 0.5f;

                        size.x = Mathf.Abs(dragStart.xOn - selectedBlock.xOn) + 1f;
                        size.y = Mathf.Abs(dragStart.yOn - selectedBlock.yOn) + 1f;
                        size.z = Mathf.Abs(dragStart.zOn - selectedBlock.zOn) + 1f;
                    }
                    else
                    {
                        center = new Vector3(selectedBlock.xOn, selectedBlock.yOn, selectedBlock.zOn);
                        center += Vector3.one * 0.5f;

                        size = Vector3.one;
                    }
                }
                else
                {
                    if (dragStart.hasSelectedBlock)
                    {
                        center.x = (dragStart.xInside + selectedBlock.xInside) / 2f;
                        center.y = (dragStart.yInside + selectedBlock.yInside) / 2f;
                        center.z = (dragStart.zInside + selectedBlock.zInside) / 2f;
                        center += Vector3.one * 0.5f;

                        size.x = Mathf.Abs(dragStart.xInside - selectedBlock.xInside) + 1f;
                        size.y = Mathf.Abs(dragStart.yInside - selectedBlock.yInside) + 1f;
                        size.z = Mathf.Abs(dragStart.zInside - selectedBlock.zInside) + 1f;
                    }
                    else
                    {
                        center = new Vector3(selectedBlock.xInside, selectedBlock.yInside, selectedBlock.zInside);
                        center += Vector3.one * 0.5f;

                        size = Vector3.one;
                    }
                }

                switch (ToolBelt.Mode)
                {
                    case ToolBelt.NORMAL_MODE:
                        {
                            switch (ToolBelt.Shape)
                            {
                                case ToolBelt.BOX_SHAPE:
                                    BoxShapeGizmos(center, size, ToolBelt.ClearSelectionMode);
                                    break;

                                case ToolBelt.WALL_SHAPE:
                                    WallShapeGizmos(center, size, ToolBelt.ClearSelectionMode);
                                    break;

                                case ToolBelt.ELLIPSE_SHAPE:
                                    BoxShapeGizmos(center, size, ToolBelt.ClearSelectionMode);
                                    break;
                            }
                            break;
                        }

                    case ToolBelt.PAINT_MODE:
                        PaintModeGizmos(center, size);
                        break;
                }

                Gizmos.matrix = Matrix4x4.identity;
            }
        }
        #endregion
    }
}
