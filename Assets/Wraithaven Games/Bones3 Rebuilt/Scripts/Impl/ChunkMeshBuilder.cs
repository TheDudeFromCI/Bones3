using System.Linq;

using UnityEngine;

namespace Bones3Rebuilt
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

            ProcMesh root = new ProcMesh();
            for (int i = 0; i < procMesh.TotalLayers; i++)
            {
                var layer = procMesh.GetLayer(i);
                root.Vertices.AddRange(layer.Vertices);
                root.Normals.AddRange(layer.Normals);
                root.UVs.AddRange(layer.UVs);
            }

            mesh.subMeshCount = procMesh.TotalLayers;
            mesh.SetVertices(root.Vertices.Select(v => new Vector3(v.X, v.Y, v.Z)).ToList());

            if (root.Normals.Count > 0)
                mesh.SetNormals(root.Normals.Select(v => new Vector3(v.X, v.Y, v.Z)).ToList());

            if (root.UVs.Count > 0)
                mesh.SetUVs(0, root.UVs.Select(v => new Vector3(v.X, v.Y, v.Z)).ToList());

            int baseVertex = 0;
            int k = 0;
            for (int i = 0; i < procMesh.TotalLayers; i++)
            {
                var layer = procMesh.GetLayer(i);

                mesh.SetTriangles(layer.Triangles, k++, true, baseVertex);
                baseVertex += layer.Vertices.Count;
            }

            mesh.RecalculateBounds();
            mesh.Optimize();
        }

        /// <summary>
        /// Applied the given visual and collision meshes to the chunk object.
        /// </summary>
        /// <param name="chunk">The chunk to refresh.</param>
        /// <param name="visualMesh">The new visual mesh data.</param>
        /// <param name="collisionMesh">The new collision mesh data.</param>
        public void RefreshMeshes(BlockChunk chunk, LayeredProcMesh visualMesh, LayeredProcMesh collisionMesh)
        {
            var meshFilter = chunk.GetComponent<MeshFilter>();
            var meshCollider = chunk.GetComponent<MeshCollider>();
            var meshRenderer = chunk.GetComponent<MeshRenderer>();

            UpdateMesh(visualMesh, meshFilter.sharedMesh);
            UpdateMesh(collisionMesh, meshCollider.sharedMesh);

            meshRenderer.sharedMaterials = new Material[] { null };

            // To trigger a refresh.
            meshFilter.sharedMesh = meshFilter.sharedMesh;
            meshCollider.sharedMesh = meshCollider.sharedMesh;
        }
    }
}
