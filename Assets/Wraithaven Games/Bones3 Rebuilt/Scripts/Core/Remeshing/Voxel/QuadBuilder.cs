using Bones3Rebuilt.Util;

namespace Bones3Rebuilt
{
    /// <summary>
    /// A utility class for converting quad data into mesh data.
    /// </summary>
    public class QuadBuilder
    {
        /// <summary>
        /// A set of parameters for a quad to generate the vertex data for.
        /// </summary>
        public struct QuadMesh
        {
            /// <summary>
            /// Gets the quad X position along the plane.
            /// </summary>
            /// <value>The quad X.</value>
            public int X { get; set; }

            /// <summary>
            /// Gets the quad Y position along the plane.
            /// </summary>
            /// <value>The quad Y.</value>
            public int Y { get; set; }

            /// <summary>
            /// Gets the quad width along the plane.
            /// </summary>
            /// <value>The quad width.</value>
            public int W { get; set; }

            /// <summary>
            /// Gets the quad height along the plane.
            /// </summary>
            /// <value>The quad height.</value>
            public int H { get; set; }

            /// <summary>
            /// Gets the side of the block this quad is on.
            /// </summary>
            /// <value>The block side.</value>
            public int Side { get; set; }

            /// <summary>
            /// Gets the offset of the plane along the 3rd axis.
            /// </summary>
            /// <value>The plane offset.</value>
            public int Offset { get; set; }

            /// <summary>
            /// Gets the texture index of this quad.
            /// </summary>
            /// <value>The texture index.</value>
            public int TextureIndex { get; set; }

            /// <summary>
            /// Gets the texture rotation of this quad.
            /// </summary>
            /// <value>The texture rotation.</value>

            public int TextureRotation { get; set; }
        }

        private readonly UVLookupTable m_UVLookupTable = new UVLookupTable();
        private readonly bool m_EnableUVs;

        /// <summary>
        /// Gets the mesh this builder is writing to.
        /// </summary>
        /// <value>The mesh.</value>
        public ProcMesh Mesh { get; set; }

        /// <summary>
        /// Creates a new quad builder.
        /// </summary>
        /// <param name="enableUVs">Whether or not UVs should be generated for quads.</param>
        public QuadBuilder(bool enableUVs)
        {
            m_EnableUVs = enableUVs;
        }

        /// <summary>
        /// Writes a quad to the mesh.
        /// </summary>
        /// <param name="quad">The quad to write.</param>
        public void WriteQuad(QuadMesh quad)
        {
            AddTriangleIndices();
            AddVertices(quad);
            AddNormals(quad.Side);

            if (m_EnableUVs)
                AddUVs(quad);
        }

        /// <summary>
        /// Adds vertices to the mesh to prepare for a new quad.
        /// </summary>
        void AddTriangleIndices()
        {
            int v = Mesh.Vertices.Count;
            Mesh.Triangles.Add(v + 0);
            Mesh.Triangles.Add(v + 1);
            Mesh.Triangles.Add(v + 2);
            Mesh.Triangles.Add(v + 0);
            Mesh.Triangles.Add(v + 2);
            Mesh.Triangles.Add(v + 3);
        }

        /// <summary>
        /// Adds quad vertices to the mesh based on the given rect and layer information.
        /// </summary>
        /// <param name="quad">The quad data.</param>
        void AddVertices(QuadMesh quad)
        {
            switch (quad.Side)
            {
                case 0:
                    AddQuadVerticesSide0(quad);
                    break;

                case 1:
                    AddQuadVerticesSide1(quad);
                    break;

                case 2:
                    AddQuadVerticesSide2(quad);
                    break;

                case 3:
                    AddQuadVerticesSide3(quad);
                    break;

                case 4:
                    AddQuadVerticesSide4(quad);
                    break;

                case 5:
                    AddQuadVerticesSide5(quad);
                    break;
            }
        }

        void AddQuadVerticesSide0(QuadMesh quad)
        {
            int sx = quad.Offset;
            int sy = quad.X;
            int sz = quad.Y;

            int bx = sx + 1;
            int by = sy + quad.W;
            int bz = sz + quad.H;

            Mesh.Vertices.Add(new Vec3(bx, by, bz));
            Mesh.Vertices.Add(new Vec3(bx, sy, bz));
            Mesh.Vertices.Add(new Vec3(bx, sy, sz));
            Mesh.Vertices.Add(new Vec3(bx, by, sz));
        }

        void AddQuadVerticesSide1(QuadMesh quad)
        {
            int sx = quad.Offset;
            int sy = quad.X;
            int sz = quad.Y;

            int by = sy + quad.W;
            int bz = sz + quad.H;

            Mesh.Vertices.Add(new Vec3(sx, sy, sz));
            Mesh.Vertices.Add(new Vec3(sx, sy, bz));
            Mesh.Vertices.Add(new Vec3(sx, by, bz));
            Mesh.Vertices.Add(new Vec3(sx, by, sz));
        }

        void AddQuadVerticesSide2(QuadMesh quad)
        {
            int sx = quad.X;
            int sy = quad.Offset;
            int sz = quad.Y;

            int bx = sx + quad.W;
            int by = sy + 1;
            int bz = sz + quad.H;

            Mesh.Vertices.Add(new Vec3(sx, by, sz));
            Mesh.Vertices.Add(new Vec3(sx, by, bz));
            Mesh.Vertices.Add(new Vec3(bx, by, bz));
            Mesh.Vertices.Add(new Vec3(bx, by, sz));
        }

        void AddQuadVerticesSide3(QuadMesh quad)
        {
            int sx = quad.X;
            int sy = quad.Offset;
            int sz = quad.Y;

            int bx = sx + quad.W;
            int bz = sz + quad.H;

            Mesh.Vertices.Add(new Vec3(bx, sy, bz));
            Mesh.Vertices.Add(new Vec3(sx, sy, bz));
            Mesh.Vertices.Add(new Vec3(sx, sy, sz));
            Mesh.Vertices.Add(new Vec3(bx, sy, sz));
        }

        void AddQuadVerticesSide4(QuadMesh quad)
        {
            int sx = quad.X;
            int sy = quad.Y;
            int sz = quad.Offset;

            int bx = sx + quad.W;
            int by = sy + quad.H;
            int bz = sz + 1;

            Mesh.Vertices.Add(new Vec3(bx, by, bz));
            Mesh.Vertices.Add(new Vec3(sx, by, bz));
            Mesh.Vertices.Add(new Vec3(sx, sy, bz));
            Mesh.Vertices.Add(new Vec3(bx, sy, bz));
        }

        void AddQuadVerticesSide5(QuadMesh quad)
        {
            int sx = quad.X;
            int sy = quad.Y;
            int sz = quad.Offset;

            int bx = sx + quad.W;
            int by = sy + quad.H;

            Mesh.Vertices.Add(new Vec3(sx, sy, sz));
            Mesh.Vertices.Add(new Vec3(sx, by, sz));
            Mesh.Vertices.Add(new Vec3(bx, by, sz));
            Mesh.Vertices.Add(new Vec3(bx, sy, sz));
        }

        /// <summary>
        /// Adds quad uvs to the mesh based on the given rect and layer information.
        /// </summary>
        /// <param name="quad">The quad data.</param>
        void AddUVs(QuadMesh quad)
        {
            var specs = new UVLookupTable.UVSpecs
            {
                Side = quad.Side,
                Rotation = quad.TextureRotation,
                W = quad.W,
                H = quad.H,
                Texture = quad.TextureIndex,
            };

            m_UVLookupTable.Find(Mesh.UVs, specs);
        }

        /// <summary>
        /// Adds the quad normals to the mesh.
        /// </summary>
        /// <param name="side">The side of the block.</param>
        void AddNormals(int side)
        {
            var dir = new BlockPosition(0, 0, 0).ShiftAlongDirection(side);
            var vec = new Vec3(dir.X, dir.Y, dir.Z);

            for (int i = 0; i < 4; i++)
                Mesh.Normals.Add(vec);
        }
    }
}
