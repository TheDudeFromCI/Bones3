using System.Collections.Generic;
using UnityEngine;

namespace Bones3Rebuilt.Remeshing
{
    /// <summary>
    /// A collection of data which is used to make up a mesh object.
    /// </summary>
    public class ProcMesh
    {
        /// <summary>
        /// Gets the generated vertex list.
        /// </summary>
        /// <value>The vertex list.</value>
        public List<Vector3> Vertices { get; } = new List<Vector3>();

        /// <summary>
        /// Gets the generated normal list.
        /// </summary>
        /// <value>The normal list.</value>
        public List<Vector3> Normals { get; } = new List<Vector3>();

        /// <summary>
        /// Gets the generated uv list, for local block uvs.
        /// </summary>
        /// <value>The uv list.</value>
        public List<Vector3> UVs { get; } = new List<Vector3>();

        /// <summary>
        /// Gets the generated triangle list.
        /// </summary>
        /// <value>The triangle list.</value>
        public List<int> Triangles { get; } = new List<int>();

        /// <summary>
        /// Clears all data in this mesh.
        /// </summary>
        public void Clear()
        {
            Vertices.Clear();
            Normals.Clear();
            UVs.Clear();
            Triangles.Clear();
        }
    }
}
