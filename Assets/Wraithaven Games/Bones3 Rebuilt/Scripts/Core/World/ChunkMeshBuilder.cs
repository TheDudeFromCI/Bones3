using UnityEngine;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// Handles converting remesh task data to chunk mesh data.
    /// </summary>
    internal class ChunkMeshBuilder
    {
        private readonly BlockWorld m_BlockWorld;
        private readonly ProcMesh m_ProcMesh;

        /// <summary>
        /// Creates a new chunk mesh builder.
        /// </summary>
        /// <param name="blockWorld">The block world this mesh builder is acting on.</param>
        internal ChunkMeshBuilder(BlockWorld blockWorld)
        {
            m_BlockWorld = blockWorld;
            m_ProcMesh = new ProcMesh();
        }

        /// <summary>
        /// Updates mesh data for the given chunk.
        /// </summary>
        /// <param name="taskStack">The set of remesh tasks.</param>
        /// <param name="chunk">The chunk to update.</param>
        internal void UpdateMesh(RemeshTaskStack taskStack, BlockChunk chunk)
        {
            UpdateVisualMesh(taskStack, chunk);
            UpdateCollisionMesh(taskStack, chunk);

            m_ProcMesh.Clear();
        }

        /// <summary>
        /// Pulls vertex data from the task stack and puts it into the procMesh.
        /// </summary>
        /// <param name="taskStack">The task stack to pull from.</param>
        /// <param name="submeshCount">The number of submeshes being generated.</param>
        /// <typeparam name="T">The type of task to look for.</typeparam>
        private void GetVertexData<T>(RemeshTaskStack taskStack, out int submeshCount)
        {
            m_ProcMesh.Clear();
            submeshCount = 0;

            for (int i = 0; i < taskStack.TaskCount; i++)
            {
                var task = taskStack.GetTask(i);

                if (task is T)
                {
                    var newMesh = task.Finish();
                    m_ProcMesh.Vertices.AddRange(newMesh.Vertices);
                    m_ProcMesh.Normals.AddRange(newMesh.Normals);
                    m_ProcMesh.UVs.AddRange(newMesh.UVs);
                    submeshCount++;
                }
            }
        }

        /// <summary>
        /// Retrieves vertex data from the task stack and writes it to the mesh object.
        /// </summary>
        /// <param name="taskStack">The task stack to pull from.</param>
        /// <param name="mesh">The mesh to write to.</param>
        /// <typeparam name="T">The type of tasks to look for in the stack.</typeparam>
        private void ApplyVertexData<T>(RemeshTaskStack taskStack, Mesh mesh)
        {
            GetVertexData<T>(taskStack, out int submeshCount);

            mesh.Clear();
            mesh.subMeshCount = submeshCount;
            mesh.SetVertices(m_ProcMesh.Vertices);
            mesh.SetNormals(m_ProcMesh.Normals);
            mesh.SetUVs(0, m_ProcMesh.UVs);
        }

        /// <summary>
        /// Looks for all visual mesh tasks and writes the generated meshes to the chunk.
        /// </summary>
        /// <param name="taskStack">The task stack.</param>
        /// <param name="chunk">The chunk to update.</param>
        private void UpdateVisualMesh(RemeshTaskStack taskStack, BlockChunk chunk)
        {
            var meshFilter = chunk.GetComponent<MeshFilter>();
            var meshRenderer = chunk.GetComponent<MeshRenderer>();
            var visualMesh = meshFilter.sharedMesh;

            ApplyVertexData<VisualRemeshTask>(taskStack, visualMesh);

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
                    materials[submeshIndex] = m_BlockWorld.BlockList.GetMaterial(vis.MaterialID);

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
        private void UpdateCollisionMesh(RemeshTaskStack taskStack, BlockChunk chunk)
        {
            var meshCollider = chunk.GetComponent<MeshCollider>();
            var collisionMesh = meshCollider.sharedMesh;

            ApplyVertexData<CollisionRemeshTask>(taskStack, collisionMesh);

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
