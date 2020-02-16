using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

namespace WraithavenGames.Bones3.Meshing
{
    /// <summary>
    /// This job generates all of the triangle indices for the given number of quads
    /// in the chunk.
    /// </summary>
    public struct GenerateTriangles : IJob
    {
        /// <summary>
        /// The number of quads in this chunk.
        /// </summary>
        public int quadCount;

        /// <summary>
        /// The array of triangles to write to. There will be quadCount * 2 triangles
        /// in this array.
        /// </summary>
        public NativeArray<int> triangles;

        public void Execute()
        {
            int t = 0;
            for (int i = 0; i < quadCount; i++)
            {
                triangles[t++] = i * 4 + 0;
                triangles[t++] = i * 4 + 1;
                triangles[t++] = i * 4 + 2;
                triangles[t++] = i * 4 + 0;
                triangles[t++] = i * 4 + 2;
                triangles[t++] = i * 4 + 3;
            }
        }
    }
}