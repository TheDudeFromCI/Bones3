using System.Linq;

using Bones3Rebuilt;

using UnityEngine;

namespace WraithavenGames.Bones3
{
    public class ChunkMeshBuilder
    {
        /// <summary>
        /// Applies the given proc mesh to the Unity mesh.
        /// </summary>
        /// <param name="procMesh">The proc mesh to retrieve the data from.</param>
        /// <param name="mesh">The Unity mesh to write to.</param>
        public void UpdateMesh(LayeredProcMesh procMesh, Mesh mesh)
        {
            mesh.Clear();
            mesh.subMeshCount = procMesh.TotalLayers;

            var root = CompileRootMesh(procMesh);
            AssignVertexData(root, mesh);

            AssignTriangleData(procMesh, mesh);

            mesh.RecalculateBounds();
        }

        /// <summary>
        /// Compiles the vertex data from a layered proc mesh into a single layered mesh.
        /// </summary>
        /// <param name="procMesh">The layered proc mesh.</param>
        /// <returns>The single layered mesh.</returns>
        private ProcMesh CompileRootMesh(LayeredProcMesh procMesh)
        {
            ProcMesh root = new ProcMesh();
            for (int i = 0; i < procMesh.TotalLayers; i++)
            {
                var layer = procMesh.GetLayer(i);
                root.Vertices.AddRange(layer.Vertices);
                root.Normals.AddRange(layer.Normals);
                root.UVs.AddRange(layer.UVs);
            }

            return root;
        }

        /// <summary>
        /// Applies the vertex data from the given proc mesh to the target Unity mesh.
        /// </summary>
        /// <param name="root">The generated mesh.</param>
        /// <param name="mesh">The target mesh.</param>
        private void AssignVertexData(ProcMesh root, Mesh mesh)
        {
            mesh.SetVertices(root.Vertices.Select(v => new Vector3(v.X, v.Y, v.Z)).ToList());

            if (root.Normals.Count > 0)
                mesh.SetNormals(root.Normals.Select(v => new Vector3(v.X, v.Y, v.Z)).ToList());

            if (root.UVs.Count > 0)
                mesh.SetUVs(0, root.UVs.Select(v => new Vector3(v.X, v.Y, v.Z)).ToList());
        }

        /// <summary>
        /// Applies the generated triangle data to the target Unity mesh.
        /// </summary>
        /// <param name="root">The generated mesh.</param>
        /// <param name="mesh">The target mesh.</param>
        private void AssignTriangleData(LayeredProcMesh procMesh, Mesh mesh)
        {
            int baseVertex = 0;
            int k = 0;
            for (int i = 0; i < procMesh.TotalLayers; i++)
            {
                var layer = procMesh.GetLayer(i);

                mesh.SetTriangles(layer.Triangles, k++, true, baseVertex);
                baseVertex += layer.Vertices.Count;
            }
        }

        /// <summary>
        /// Applied the given visual and collision meshes to the chunk object.
        /// </summary>
        /// <param name="chunk">The chunk to refresh.</param>
        /// <param name="visualMesh">The new visual mesh data.</param>
        /// <param name="collisionMesh">The new collision mesh data.</param>
        public void RefreshMeshes(BlockChunk chunk, RemeshFinishEvent ev, Bones3TextureAtlasList atlasList)
        {
            var visualMesh = ev.Report.VisualMesh;
            var collisionMesh = ev.Report.CollisionMesh;
            var atlases = ev.Report.Atlases;

            var meshFilter = chunk.GetComponent<MeshFilter>();
            var meshCollider = chunk.GetComponent<MeshCollider>();
            var meshRenderer = chunk.GetComponent<MeshRenderer>();

            UpdateMesh(visualMesh, meshFilter.sharedMesh);
            UpdateMesh(collisionMesh, meshCollider.sharedMesh);

            var materials = new Material[visualMesh.TotalLayers];
            for (int i = 0; i < materials.Length; i++)
                materials[i] = atlasList.GetAtlas(atlases[i]).Material;

            meshRenderer.sharedMaterials = materials;

            // To trigger a refresh.
            meshFilter.sharedMesh = meshFilter.sharedMesh;
            meshCollider.sharedMesh = meshCollider.sharedMesh;
        }
    }
}
