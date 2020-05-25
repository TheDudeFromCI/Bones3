using UnityEngine;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// Handles converting remesh task data to chunk mesh data.
    /// </summary>
    public static class ChunkMeshBuilder
    {
        /// <summary>
        /// Updates mesh data for the given chunk.
        /// </summary>
        /// <param name="taskStack">The set of remesh tasks.</param>
        /// <param name="chunk">The chunk to update.</param>
        /// <param name="blockList">The block list.</param>
        public static void UpdateMesh(RemeshTaskStack taskStack, BlockChunk chunk, BlockList blockList)
        {
            ProcMesh procMesh = new ProcMesh();
            UpdateVisualMesh(taskStack, chunk, blockList, procMesh);
            UpdateCollisionMesh(taskStack, chunk, procMesh);
        }

        /// <summary>
        /// Pulls vertex data from the task stack and puts it into the procMesh.
        /// </summary>
        /// <param name="taskStack">The task stack to pull from.</param>
        /// <param name="procMesh">The proc mesh to write to.</param>
        /// <param name="submeshCount">The number of submeshes being generated.</param>
        /// <typeparam name="T">The type of task to look for.</typeparam>
        private static void GetVertexData<T>(RemeshTaskStack taskStack, ProcMesh procMesh, out int submeshCount)
        {
            procMesh.Clear();
            submeshCount = 0;

            for (int i = 0; i < taskStack.TaskCount; i++)
            {
                var task = taskStack.GetTask(i);

                if (task is T)
                {
                    var newMesh = task.Finish();
                    procMesh.Vertices.AddRange(newMesh.Vertices);
                    procMesh.Normals.AddRange(newMesh.Normals);
                    procMesh.UVs.AddRange(newMesh.UVs);
                    submeshCount++;
                }
            }
        }

        /// <summary>
        /// Retrieves vertex data from the task stack and writes it to the mesh object.
        /// </summary>
        /// <param name="taskStack">The task stack to pull from.</param>
        /// <param name="procMesh">The proc mesh to use as a data buffer.</param>
        /// <param name="mesh">The mesh to write to.</param>
        /// <typeparam name="T">The type of tasks to look for in the stack.</typeparam>
        private static void ApplyVertexData<T>(RemeshTaskStack taskStack, ProcMesh procMesh, Mesh mesh)
        {
            GetVertexData<T>(taskStack, procMesh, out int submeshCount);

            mesh.Clear();
            mesh.subMeshCount = submeshCount;
            mesh.SetVertices(procMesh.Vertices);
            mesh.SetNormals(procMesh.Normals);
            mesh.SetUVs(0, procMesh.UVs);
        }

        /// <summary>
        /// Looks for all visual mesh tasks and writes the generated meshes to the chunk.
        /// </summary>
        /// <param name="taskStack">The task stack.</param>
        /// <param name="chunk">The chunk to update.</param>
        /// <param name="blockList">The block list being used.</param>
        /// <param name="procMesh">The proc mesh to use as a data buffer.</param>
        private static void UpdateVisualMesh(RemeshTaskStack taskStack, BlockChunk chunk, BlockList blockList, ProcMesh procMesh)
        {
            var meshFilter = chunk.GetComponent<MeshFilter>();
            var meshRenderer = chunk.GetComponent<MeshRenderer>();
            var visualMesh = meshFilter.sharedMesh;

            ApplyVertexData<VisualRemeshTask>(taskStack, procMesh, visualMesh);

            var materials = new Material[visualMesh.subMeshCount];

            int baseVertex = 0;
            int submeshIndex = 0;
            for (int i = 0; i < taskStack.TaskCount; i++)
            {
                var task = taskStack.GetTask(i);

                if (task is VisualRemeshTask vis)
                {
                    var newMesh = vis.Finish();
                    visualMesh.SetTriangles(newMesh.Triangles, submeshIndex, true, baseVertex);
                    materials[submeshIndex] = blockList.GetMaterial(vis.MaterialID);

                    submeshIndex++;
                    baseVertex += newMesh.Vertices.Count;
                }
            }

            meshFilter.sharedMesh = visualMesh;
            meshRenderer.sharedMaterials = materials;
        }

        /// <summary>
        /// Looks for all collision mesh tasks and writes the generated meshes to the chunk.
        /// </summary>
        /// <param name="taskStack">The task stack.</param>
        /// <param name="chunk">The chunk to update.</param>
        /// <param name="blockList">The block list being used.</param>
        /// <param name="procMesh">The proc mesh to use as a data buffer.</param>
        private static void UpdateCollisionMesh(RemeshTaskStack taskStack, BlockChunk chunk, ProcMesh procMesh)
        {
            var meshCollider = chunk.GetComponent<MeshCollider>();
            var collisionMesh = meshCollider.sharedMesh;

            ApplyVertexData<CollisionRemeshTask>(taskStack, procMesh, collisionMesh);

            int baseVertex = 0;
            int submeshIndex = 0;
            for (int i = 0; i < taskStack.TaskCount; i++)
            {
                var task = taskStack.GetTask(i);

                if (task is CollisionRemeshTask vis)
                {
                    var newMesh = vis.Finish();
                    collisionMesh.SetTriangles(newMesh.Triangles, submeshIndex, true, baseVertex);

                    submeshIndex++;
                    baseVertex += newMesh.Vertices.Count;
                }
            }

            meshCollider.sharedMesh = collisionMesh;
        }
    }
}
