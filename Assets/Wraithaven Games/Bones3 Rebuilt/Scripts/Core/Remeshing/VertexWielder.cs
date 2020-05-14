using System.Numerics;

namespace Bones3Rebuilt
{
    public static class VertexWielder
    {
        public static void WieldVertices(this ProcMesh mesh)
        {
            bool hasNormals = mesh.Normals.Count > 0;
            bool hasUVs = mesh.UVs.Count > 0;

            if ((hasNormals && mesh.Normals.Count != mesh.Vertices.Count)
                || (hasUVs && mesh.UVs.Count != mesh.Vertices.Count))
                throw new System.InvalidOperationException("Mesh does not contain equal vertex sizes!");

            for (int v = 0; v < mesh.Vertices.Count; v++)
            {
                var vertex = mesh.Vertices[v];
                var normal = hasNormals ? mesh.Normals[v] : default;
                var uv = hasUVs ? mesh.UVs[v] : default;

                for (int r = v + 1; r < mesh.Vertices.Count; r++)
                {
                    if (mesh.Vertices[r] != vertex)
                        continue;

                    if (hasNormals && mesh.Normals[r] != normal)
                        continue;

                    if (hasUVs && mesh.UVs[r] != uv)
                        continue;

                    mesh.Vertices.RemoveAt(r);

                    if (hasNormals)
                        mesh.Normals.RemoveAt(r);

                    if (hasUVs)
                        mesh.UVs.RemoveAt(r);

                    for (int t = 0; t < mesh.Triangles.Count; t++)
                    {
                        if (mesh.Triangles[t] < r)
                            continue;

                        if (mesh.Triangles[t] == r)
                        {
                            mesh.Triangles[t] = v;
                            continue;
                        }

                        mesh.Triangles[t] -= 1;
                    }
                }
            }
        }
    }
}
