using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

namespace WraithavenGames.Bones3.Meshing
{
    /// <summary>
    /// This job takes in an array quad.offsetf quads and generates the corresponding vertices.
    /// </summary>
    public struct GenerateVertices : IJob
    {
        /// <summary>
        /// The array of quads to read from when generating vertices.
        /// </summary>
        [ReadOnly]
        public NativeArray<Quad> quads;

        /// <summary>
        /// The number of quads in the array.
        /// </summary>
        public int quadCount;

        /// <summary>
        /// The array of vertices to write to. This job will write quadCount * 4 vertices to this array.
        /// </summary>
        public NativeArray<Vector3> vertices;

        /// <summary>
        /// The position within the array to start writing vertices to.
        /// </summary>
        public int vertexPos;

        public void Execute()
        {
            for (int i = 0; i < quadCount; i++)
            {
                Quad quad = quads[i];

                switch (quad.side)
                {
                    case 0:
                        {
                            float sx = quad.offset;
                            float sy = quad.x;
                            float sz = quad.y;
                            float bx = sx + 1;
                            float by = sy + quad.w;
                            float bz = sz + quad.h;

                            vertices[vertexPos++] = new Vector3(bx, by, bz);
                            vertices[vertexPos++] = new Vector3(bx, sy, bz);
                            vertices[vertexPos++] = new Vector3(bx, sy, sz);
                            vertices[vertexPos++] = new Vector3(bx, by, sz);
                            break;
                        }

                    case 1:
                        {
                            float sx = quad.offset;
                            float sy = quad.x;
                            float sz = quad.y;
                            float by = sy + quad.w;
                            float bz = sz + quad.h;

                            vertices[vertexPos++] = new Vector3(sx, sy, sz);
                            vertices[vertexPos++] = new Vector3(sx, sy, bz);
                            vertices[vertexPos++] = new Vector3(sx, by, bz);
                            vertices[vertexPos++] = new Vector3(sx, by, sz);
                            break;
                        }

                    case 2:
                        {
                            float sx = quad.x;
                            float sy = quad.offset;
                            float sz = quad.y;
                            float bx = sx + quad.w;
                            float by = sy + 1;
                            float bz = sz + quad.h;

                            vertices[vertexPos++] = new Vector3(sx, by, sz);
                            vertices[vertexPos++] = new Vector3(sx, by, bz);
                            vertices[vertexPos++] = new Vector3(bx, by, bz);
                            vertices[vertexPos++] = new Vector3(bx, by, sz);
                            break;
                        }

                    case 3:
                        {
                            float sx = quad.x;
                            float sy = quad.offset;
                            float sz = quad.y;
                            float bx = sx + quad.w;
                            float bz = sz + quad.h;

                            vertices[vertexPos++] = new Vector3(bx, sy, bz);
                            vertices[vertexPos++] = new Vector3(sx, sy, bz);
                            vertices[vertexPos++] = new Vector3(sx, sy, sz);
                            vertices[vertexPos++] = new Vector3(bx, sy, sz);
                            break;
                        }

                    case 4:
                        {
                            float sx = quad.x;
                            float sy = quad.y;
                            float sz = quad.offset;
                            float bx = sx + quad.w;
                            float by = sy + quad.h;
                            float bz = sz + 1;

                            vertices[vertexPos++] = new Vector3(bx, by, bz);
                            vertices[vertexPos++] = new Vector3(sx, by, bz);
                            vertices[vertexPos++] = new Vector3(sx, sy, bz);
                            vertices[vertexPos++] = new Vector3(bx, sy, bz);
                            break;
                        }

                    case 5:
                        {
                            float sx = quad.x;
                            float sy = quad.y;
                            float sz = quad.offset;
                            float bx = sx + quad.w;
                            float by = sy + quad.h;

                            vertices[vertexPos++] = new Vector3(sx, sy, sz);
                            vertices[vertexPos++] = new Vector3(sx, by, sz);
                            vertices[vertexPos++] = new Vector3(bx, by, sz);
                            vertices[vertexPos++] = new Vector3(bx, sy, sz);
                            break;
                        }
                }
            }
        }
    }
}