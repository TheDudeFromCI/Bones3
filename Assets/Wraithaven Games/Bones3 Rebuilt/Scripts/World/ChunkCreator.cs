using Bones3Rebuilt;

using UnityEngine;

namespace WraithavenGames.Bones3
{
    public static class ChunkCreator
    {
        /// <summary>
        /// Creates a new chunk object based on the given chunk position.
        /// </summary>
        /// <param name="chunkPos">The position of the chunk.</param>
        /// <param name="chunkSize">The number of blocks in a chunk along 1 axis.</param>
        /// <param name="transform">The transform of the block world.</param>
        /// <returns>The newly created chunk game object.</returns>
        public static BlockChunk LoadChunk(ChunkPosition chunkPos, int chunkSize, Transform transform)
        {
            var go = new GameObject($"Chunk: ({chunkPos.X}, {chunkPos.Y}, {chunkPos.Z})");
            var chunk = go.AddComponent<BlockChunk>();
            chunk.Position = chunkPos;

            go.hideFlags = HideFlags.HideAndDontSave;
            go.transform.SetParent(transform);
            go.transform.localPosition = new Vector3(chunkPos.X, chunkPos.Y, chunkPos.Z) * chunkSize;

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
        public static void DestroyChunk(BlockChunk chunk)
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
