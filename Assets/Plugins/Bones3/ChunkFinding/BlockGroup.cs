using UnityEngine;
using WraithavenGames.Bones3;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WraithavenGames.Bones3.ChunkFinding
{
    [System.Serializable]
    public class BlockGroup : IBlockHolder
    {
        [SerializeField] private int size;
        [SerializeField] private int x;
        [SerializeField] private int y;
        [SerializeField] private int z;
        [SerializeField] private BlockWorld world;

        public int Size { get { return size; } }
        public int X { get { return x; } }
        public int Y { get { return y; } }
        public int Z { get { return z; } }

        private IBlockHolder[] children = new IBlockHolder[8];

        public BlockGroup(int size, int x, int y, int z, BlockWorld world)
        {
            this.size = size;
            this.x = x;
            this.y = y;
            this.z = z;
            this.world = world;
        }

        #region OctreeHandle
        public void Clear()
        {
            for (int i = 0; i < 8; i++)
            {
                if (IsNull(i))
                    continue;

                if (size == 1)
                {
                    Chunk c = children[i] as Chunk;
#if UNITY_EDITOR
                        if (Application.isPlaying)
                            GameObject.Destroy(c.gameObject);
                        else
                            GameObject.DestroyImmediate(c.gameObject);
#else
                    GameObject.Destroy(c.gameObject);
#endif
                }
                else
                    ((BlockGroup)children[i]).Clear();

                children[i] = null;
            }
        }

        private bool IsNull(int index)
        {
            if (size == 1)
            {
                if (children[index] != null && ((Chunk)children[index]) == null)
                    children[index] = null;
            }

            return children[index] == null;
        }

        public void UpdateAllChunks()
        {
            for (int i = 0; i < 8; i++)
            {
                if (IsNull(i))
                    continue;

                if (size == 1)
                    ((Chunk)children[i]).Regenerate();
                else
                    ((BlockGroup)children[i]).UpdateAllChunks();
            }
        }

        public void RemeshAllChunks()
        {
            for (int i = 0; i < 8; i++)
            {
                if (IsNull(i))
                    continue;

                if (size == 1)
                    world.RemeshChunk(children[i] as Chunk);
                else
                    ((BlockGroup)children[i]).RemeshAllChunks();
            }
        }

        public void UpdateAllBlockStates(ushort block, byte state)
        {
            for (int i = 0; i < 8; i++)
            {
                if (IsNull(i))
                    continue;

                if (size == 1)
                    ((Chunk)children[i]).RebuildBlockStates(block, state);
                else
                    ((BlockGroup)children[i]).UpdateAllBlockStates(block, state);
            }
        }

        public void UnloadDistantChunks(int x, int y, int z, int radius)
        {
            float groupRadius = 1 << (size - 1);
            float rootTwo = 1.414213562373095f;
            float r = radius * rootTwo;
            float r2 = r * r;
            float c2 = (0.5f * rootTwo) * (0.5f * rootTwo);

            Vector3 pos = new Vector3();
            pos.x = this.x + groupRadius;
            pos.y = this.y + groupRadius;
            pos.z = this.z + groupRadius;

            Vector3 center = new Vector3();
            center.x = x + 0.5f;
            center.y = y + 0.5f;
            center.z = z + 0.5f;

            // If group is completely outside radius, unload it
            float unloadRadius = groupRadius * rootTwo * groupRadius * rootTwo;
            if ((pos - center).sqrMagnitude >= r2 + unloadRadius)
            {
                Clear();
                return;
            }

            // If group is completely inside radius, ignore it
            if ((pos - center).magnitude + groupRadius * rootTwo <= r)
                return;

            // Check children
            for (int i = 0; i < 8; i++)
            {
                if (IsNull(i))
                    continue;

                if (size == 1)
                {
                    Chunk c = children[i] as Chunk;
                    pos.x = c.chunkX + 0.5f;
                    pos.y = c.chunkY + 0.5f;
                    pos.z = c.chunkZ + 0.5f;

                    // If chunk is completely outside radius, unload it
                    if ((pos - center).sqrMagnitude >= r2 + c2)
                    {
#if UNITY_EDITOR
                            if (Application.isPlaying)
                                GameObject.Destroy(c.gameObject);
                            else
                                GameObject.DestroyImmediate(c.gameObject);
#else
                        GameObject.Destroy(c.gameObject);
#endif

                        children[i] = null;
                    }
                }
                else
                {
                    BlockGroup group = children[i] as BlockGroup;
                    group.UnloadDistantChunks(x, y, z, radius);

                    if (group.IsEmpty())
                        children[i] = null;
                }
            }
        }

        public void ClearChunk(int x, int y, int z)
        {
            int a = (x >> (size - 1)) & 1;
            int b = (y >> (size - 1)) & 1;
            int c = (z >> (size - 1)) & 1;
            int index = a * 2 * 2 + b * 2 + c;

            if (IsNull(index))
                return;

            if (size == 1)
            {
                Chunk chunk = children[index] as Chunk;
#if UNITY_EDITOR
                    if (Application.isPlaying)
                        GameObject.Destroy(chunk.gameObject);
                    else
                        GameObject.DestroyImmediate(chunk.gameObject);
#else
                GameObject.Destroy(chunk.gameObject);
#endif

                children[index] = null;
            }
            else
            {
                BlockGroup group = children[index] as BlockGroup;
                group.ClearChunk(x, y, z);

                if (group.IsEmpty())
                    children[index] = null;
            }
        }

        public bool IsEmpty()
        {
            for (int i = 0; i < 8; i++)
                if (!IsNull(i))
                    return false;
            return true;
        }

        private void SetChunk(int x, int y, int z, Chunk chunk)
        {
            int a = (x >> (size - 1)) & 1;
            int b = (y >> (size - 1)) & 1;
            int c = (z >> (size - 1)) & 1;
            int index = a * 2 * 2 + b * 2 + c;

            if (IsNull(index))
            {
                if (size == 1)
                {
                    children[index] = chunk;
                    return;
                }
                else
                {
                    int s = 1 << (size - 1);
                    children[index] = new BlockGroup(size - 1, this.x + a * s, this.y + b * s, this.z + c * s, world);
                }
            }

            if (size == 1)
                children[index] = chunk;
            else
                ((BlockGroup)children[index]).SetChunk(x, y, z, chunk);
        }

        public Chunk GetChunk(int x, int y, int z, bool create)
        {
            int a = (x >> (size - 1)) & 1;
            int b = (y >> (size - 1)) & 1;
            int c = (z >> (size - 1)) & 1;
            int index = a * 2 * 2 + b * 2 + c;

            if (IsNull(index))
            {
                if (!create)
                    return null;

                if (size == 1)
                {
                    GameObject go = new GameObject();
                    go.name = "Chunk (" + x + ", " + y + ", " + z + ")";
                    go.isStatic = world.gameObject.isStatic;
                    Chunk ch = go.AddComponent<Chunk>();
                    ch.chunkX = x;
                    ch.chunkY = y;
                    ch.chunkZ = z;

#if UNITY_EDITOR
                        if (!Application.isPlaying)
                        {
                            Undo.RegisterCreatedObjectUndo(go, "Created chunk.");
                            Undo.SetTransformParent(go.transform, world.transform, "Created chunk heirarchy.");
                        }
                        else
                            go.transform.SetParent(world.transform, false);
#else
                    go.transform.SetParent(world.transform, false);
#endif

                    go.transform.localPosition = new Vector3(x, y, z) * 16;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;

#if UNITY_EDITOR
                        Undo.RecordObject(go, "Initialized chunk.");
						EditorUtility.SetSelectedRenderState(go.GetComponent<Renderer>(), EditorSelectedRenderState.Hidden);
#endif

                    children[index] = ch;
                }
                else
                {
                    int s = 1 << (size - 1);
                    children[index] = new BlockGroup(size - 1, this.x + a * s, this.y + b * s, this.z + c * s, world);
                }
            }

            if (size == 1)
                return children[index] as Chunk;
            return ((BlockGroup)children[index]).GetChunk(x, y, z, create);
        }
        #endregion

        #region SerializationHandler
        [SerializeField] private Chunk[] chunkArray;

        public void Serialize()
        {
            int side = 1 << size;
            lock (children)
            {
                chunkArray = new Chunk[side * side * side];

                int x, y, z;
                for (x = 0; x < side; x++)
                    for (y = 0; y < side; y++)
                        for (z = 0; z < side; z++)
                            chunkArray[x * side * side + y * side + z] = GetChunk(x, y, z, false);
            }
        }

        public void Deserialize()
        {
            children = new IBlockHolder[8];
            int side = 1 << size;

            lock (children)
            {
                if (chunkArray == null)
                    return;

                int x, y, z, index;
                for (x = 0; x < side; x++)
                    for (y = 0; y < side; y++)
                        for (z = 0; z < side; z++)
                        {
                            index = x * side * side + y * side + z;
                            if (chunkArray[index] == null)
                                continue;
                            SetChunk(x + this.x, y + this.y, z + this.z, chunkArray[index]);
                        }
                chunkArray = null;
            }
        }
        #endregion
    }
}
