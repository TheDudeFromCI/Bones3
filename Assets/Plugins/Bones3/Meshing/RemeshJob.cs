using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using WraithavenGames.Bones3.BlockProperties;

namespace WraithavenGames.Bones3.Meshing
{
    /// <summary>
    /// The <c>RemeshMaterialJob</c> object is a job which is used to generate the vertex data for
    /// a given target block of a chunk. Multiple remesh jobs are usually run in parallel, one for
    /// each block type within the chunk and one for the collision of the chunk.
    /// </summary>
    public struct RemeshMaterialJob : IJob
    {
        /// <summary>
        /// An array of block ids currently in the chunk. The length of the array is exactly 4069,
        /// or 16x16x16. The block at (x, y, z) is located at index x * 16 * 16 + y * 16 + z. At
        /// each position, the value provided is the id value of the block, with 0 being air.
        /// </summary>
        [ReadOnly]
        ushort[] blocks;

        /// <summary>
        /// A list of block properties for how blocks should be handled in the mesher. All blocks
        /// contained within the block array have a block id object in this list.
        /// </summary>
        [ReadOnly]
        NativeArray<BlockID> blockProperties;

        /// <summary>
        /// The id of the block this remesh job is targeting. All other block types will be ignored
        /// and must be processed in a seperate job.
        /// </summary>
        ushort targetBlock;

        /// <summary>
        /// A list of vertex locations for this mesh. If empty, this vertex data object represents an
        /// empty mesh.
        /// </summary>
        NativeArray<Vector3> vertices;

        /// <summary>
        /// A list of normal values for this mesh.
        /// </summary>
        NativeArray<Vector3> normals;

        /// <summary>
        /// A list of uv values for this mesh.
        /// </summary>
        NativeArray<Vector2> uvs;

        /// <summary>
        /// A list of vertex indices, representing the triangles for this mesh. Each triplet of indices
        /// represents a single triangle.
        /// </summary>
        NativeArray<ushort> triangles;

        /// <summary>
        /// Creates a new <c>RemeshMaterialJob</c> object.
        /// </summary>
        /// <param name="blocks">The list of block ids within the chunk.</param>
        /// <param name="blockProperties">The list of properties for each block type.</param>
        /// <param name="targetBlock">The block id this job is generating.</param>
        /// <param name="vertices">The array to write the generated vertices to.</param>
        /// <param name="normals">The array to write the generated normals to.</param>
        /// <param name="uvs">The array to write the generated uvs to.</param>
        /// <param name="triangles">The array to write the generated triangles to.</param>
        public RemeshMaterialJob(ushort[] blocks, NativeArray<BlockID> blockProperties, ushort targetBlock, NativeArray<Vector3> vertices,
            NativeArray<Vector3> normals, NativeArray<Vector2> uvs, NativeArray<ushort> triangles)
        {
            this.blocks = blocks;
            this.blockProperties = blockProperties;
            this.targetBlock = targetBlock;
            this.vertices = vertices;
            this.normals = normals;
            this.uvs = uvs;
            this.triangles = triangles;
        }

        public void Execute()
        {
            // TODO
        }
    }

    /// <summary>
    /// The <c>RemeshCollisionJob</c> object is a chunk meshing job identical to <c>RemeshMaterialJob</c>,
    /// with the slight differences that one, it doesn't generate uv data and two, it targets all collidable
    /// blocks instead of a single target block.
    /// </summary>
    public struct RemeshCollisionJob : IJob
    {
        /// <summary>
        /// An array of block ids currently in the chunk. The length of the array is exactly 4069,
        /// or 16x16x16. The block at (x, y, z) is located at index x * 16 * 16 + y * 16 + z. At
        /// each position, the value provided is the id value of the block, with 0 being air.
        /// </summary>
        [ReadOnly]
        ushort[] blocks;

        /// <summary>
        /// A list of block properties for how blocks should be handled in the mesher. All blocks
        /// contained within the block array have a block id object in this list.
        /// </summary>
        [ReadOnly]
        NativeArray<BlockID> blockProperties;

        /// <summary>
        /// A list of vertex locations for this mesh. If empty, this vertex data object represents an
        /// empty mesh.
        /// </summary>
        NativeArray<Vector3> vertices;

        /// <summary>
        /// A list of normal values for this mesh.
        /// </summary>
        NativeArray<Vector3> normals;

        /// <summary>
        /// A list of vertex indices, representing the triangles for this mesh. Each triplet of indices
        /// represents a single triangle.
        /// </summary>
        NativeArray<ushort> triangles;

        /// <summary>
        /// Creates a new <c>RemeshCollisionJob</c> object.
        /// </summary>
        /// <param name="blocks">The list of block ids within the chunk.</param>
        /// <param name="blockProperties">The list of properties for each block type.</param>
        /// <param name="targetBlock">The block id this job is generating.</param>
        /// <param name="vertices">The array to write the generated vertices to.</param>
        /// <param name="normals">The array to write the generated normals to.</param>
        /// <param name="uvs">The array to write the generated uvs to.</param>
        /// <param name="triangles">The array to write the generated triangles to.</param>
        public RemeshCollisionJob(ushort[] blocks, NativeArray<BlockID> blockProperties, NativeArray<Vector3> vertices,
            NativeArray<Vector3> normals, NativeArray<ushort> triangles)
        {
            this.blocks = blocks;
            this.blockProperties = blockProperties;
            this.vertices = vertices;
            this.normals = normals;
            this.triangles = triangles;
        }

        public void Execute()
        {
            // TODO
        }
    }
}