using UnityEngine;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// Creates and destroyed block chunk objects.
    /// </summary>
    internal class ChunkCreator
    {
        private readonly BlockWorld m_BlockWorld;

        /// <summary>
        /// Creates a new chunk creator object.
        /// </summary>
        /// <param name="blockWorld">The block world this creator is acting on.</param>
        internal ChunkCreator(BlockWorld blockWorld)
        {
            m_BlockWorld = blockWorld;
        }

        /// <summary>
        /// Creates a new chunk object based on the given chunk position.
        /// </summary>
        /// <param name="chunkPos">The position of the chunk.</param>
        /// <returns>The newly created chunk game object.</returns>
        internal BlockChunk LoadChunk(ChunkPosition chunkPos)
        {
            var go = new GameObject($"Chunk: ({chunkPos.X}, {chunkPos.Y}, {chunkPos.Z})");
            var chunk = go.AddComponent<BlockChunk>();
            chunk.Position = chunkPos;

            go.hideFlags = HideFlags.HideAndDontSave;
            go.transform.SetParent(m_BlockWorld.transform);
            go.transform.localPosition = new Vector3(chunkPos.X, chunkPos.Y, chunkPos.Z) * m_BlockWorld.ChunkSize.Value;

            var meshFilter = go.AddComponent<MeshFilter>();
            var meshCollider = go.AddComponent<MeshCollider>();
            go.AddComponent<MeshRenderer>();

            meshFilter.sharedMesh = new Mesh
            {
                name = $"Chunk Visual: ({chunkPos.X}, {chunkPos.Y}, {chunkPos.Z})"
            };

            meshCollider.sharedMesh = new Mesh
            {
                name = $"Chunk Collision: ({chunkPos.X}, {chunkPos.Y}, {chunkPos.Z})"
            };

            return chunk;
        }

        /// <summary>
        /// Destroys a chunk game object and attached resources.
        /// </summary>
        /// <param name="chunk">The chunk to destroy.</param>
        internal void DestroyChunk(BlockChunk chunk)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                Object.Destroy(chunk.GetComponent<MeshFilter>().sharedMesh);
                Object.Destroy(chunk.GetComponent<MeshCollider>().sharedMesh);
                Object.Destroy(chunk.gameObject);
            }
            else
            {
                Object.DestroyImmediate(chunk.GetComponent<MeshFilter>().sharedMesh);
                Object.DestroyImmediate(chunk.GetComponent<MeshCollider>().sharedMesh);
                Object.DestroyImmediate(chunk.gameObject);
            }
#else
            Object.Destroy(chunk.GetComponent<MeshFilter>().sharedMesh);
            Object.Destroy(chunk.GetComponent<MeshCollider>().sharedMesh);
            Object.Destroy(chunk.gameObject);
#endif
        }
    }
}
