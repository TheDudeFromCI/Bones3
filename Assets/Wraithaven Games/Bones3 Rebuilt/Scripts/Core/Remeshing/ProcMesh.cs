using System.Collections.Generic;
using Bones3Rebuilt.Util;

namespace Bones3Rebuilt
{
    public class ProcMesh
    {
        /// <summary>
        /// Gets the generated vertex list.
        /// </summary>
        /// <value>The vertex list.</value>
        public List<Vec3> Vertices { get; } = new List<Vec3>();

        /// <summary>
        /// Gets the generated normal list.
        /// </summary>
        /// <value>The normal list.</value>
        public List<Vec3> Normals { get; } = new List<Vec3>();

        /// <summary>
        /// Gets the generated uv list, for local block uvs.
        /// </summary>
        /// <value>The uv list.</value>
        public List<Vec3> UVs { get; } = new List<Vec3>();

        /// <summary>
        /// Gets the generated triangle list.
        /// </summary>
        /// <value>The triangle list.</value>
        public List<int> Triangles { get; } = new List<int>();

        /// <summary>
        /// Checks if this mesh has any triangle data or not.
        /// </summary>
        public bool HasTriangles => Triangles.Count > 0;

        /// <summary>
        /// Clears all mesh data.
        /// </summary>
        public void Clear()
        {
            Vertices.Clear();
            Normals.Clear();
            UVs.Clear();
            Triangles.Clear();
        }

        /// <summary>
        /// Adds all data from another proc mesh to this mesh.
        /// </summary>
        /// <param name="other">The other mesh.</param>
        public void AddData(ProcMesh other)
        {
            if (other == null)
                return;

            Vertices.AddRange(other.Vertices);
            Normals.AddRange(other.Normals);
            UVs.AddRange(other.UVs);
            Triangles.AddRange(other.Triangles);
        }
    }
}
