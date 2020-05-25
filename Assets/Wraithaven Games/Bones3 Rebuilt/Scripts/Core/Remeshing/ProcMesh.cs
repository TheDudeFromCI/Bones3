using System.Collections.Generic;

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
    }
}
