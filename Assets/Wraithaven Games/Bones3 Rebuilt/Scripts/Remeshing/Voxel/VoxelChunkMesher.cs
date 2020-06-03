using System;
using System.Threading.Tasks;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// Iterates over a set of chunk properties in order to generate a chunk mesh.
    /// </summary>
    internal abstract class VoxelChunkMesher : IRemeshTask
    {
        private readonly ChunkProperties m_ChunkProperties;
        private readonly Task m_Task;
        private readonly ProcMesh m_Mesh;
        private GreedyMesher m_GreedyMesher;

        /// <inheritdoc cref="IRemeshTask"/>
        public bool IsFinished => m_Task.IsCompleted;

        /// <summary>
        /// Creates a new voxel chunk mesher.
        /// </summary>
        /// <param name="chunkProperties">The chunk properties to handle.</param>
        /// <param name="greedyMesher">The greedy mesher to use.</param>
        internal VoxelChunkMesher(ChunkProperties chunkProperties, GreedyMesher greedyMesher)
        {
            m_ChunkProperties = chunkProperties;
            m_GreedyMesher = greedyMesher;
            m_Mesh = new ProcMesh();
            m_Task = Task.Run(Remesh);
        }

        /// <summary>
        /// Iterates over all the blocks in a chunk and generates quads as needed, finally
        /// remeshing them into the output mesh when complete.
        /// </summary>
        private void Remesh()
        {
            int chunkSize = m_ChunkProperties.ChunkSize.Value;

            for (int j = 0; j < 6; j++)
            {
                for (int t = 0; t < chunkSize; t++)
                {
                    bool planeActive = false;
                    for (int a = 0; a < chunkSize; a++)
                    {
                        for (int b = 0; b < chunkSize; b++)
                        {
                            var pos = GetAsBlockCoords(j, t, a, b);

                            if (!CanPlaceQuad(m_ChunkProperties, pos, j))
                                continue;

                            var type = m_ChunkProperties.GetBlock(pos);
                            var face = type.Face(j);

                            var texture = face.TextureID;
                            var rotation = SolveRotation(pos, face.Rotation);
                            var quad = new GreedyMesher.Quad(rotation, texture);

                            m_GreedyMesher.SetQuad(a, b, quad);
                            planeActive = true;
                        }
                    }

                    if (planeActive)
                        m_GreedyMesher.Mesh(t, j, m_Mesh);
                }
            }

            m_Mesh.WieldVertices();
        }

        /// <summary>
        /// Converts the given face rotation enum to a rotation index for the quad builder.
        /// </summary>
        /// <param name="pos">The position of the block.</param>
        /// <param name="rot">The face rotation.</param>
        /// <returns>The rotation index.</returns>
        private int SolveRotation(BlockPosition pos, FaceRotation rot)
        {
            if (rot == FaceRotation.Random)
            {
                long r;
                r = pos.X * 19;
                r = r * 17 + pos.Y;
                r = r * 31 + pos.Z;

                r = r * 25214903917 + 11;
                r = r * 25214903917 + 11;
                r = r * 25214903917 + 11;
                r = r * 25214903917 + 11;

                return (int)(r & 0x7);
            }

            return (int)rot;
        }

        /// <summary>
        /// Converts the given layered plane coordinates to local block coordinates.
        /// </summary>
        /// <param name="j">The side index.</param>
        /// <param name="t">The layer.</param>
        /// <param name="a">The plane X coordinate.</param>
        /// <param name="b">The plane Y coordinate.</param>
        private BlockPosition GetAsBlockCoords(int j, int t, int a, int b)
        {
            switch (j)
            {
                case 0:
                case 1:
                    return new BlockPosition(t, a, b);

                case 2:
                case 3:
                    return new BlockPosition(a, t, b);

                case 4:
                case 5:
                    return new BlockPosition(a, b, t);

                default:
                    throw new ArgumentException($"Unknown side {j}!");
            }
        }

        /// <inheritdoc cref="IRemeshTask"/>
        public ProcMesh Finish()
        {
            m_Task.Wait();

            m_GreedyMesher?.ReturnToPool();
            m_GreedyMesher = null;

            return m_Mesh;
        }

        /// <summary>
        /// Checks whether or not the given quad within the check should be added to the mesh.
        /// </summary>
        /// <param name="chunkProperties">The chunk properties to read from.</param>
        /// <param name="pos">The block position.</param>
        /// <param name="side">The side of the block being checked.</param>
        /// <returns>True if the quad should be placed. False otherwise.</returns>
        protected abstract bool CanPlaceQuad(ChunkProperties chunkProperties, BlockPosition pos, int side);
    }
}
