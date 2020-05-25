using System.Collections.Generic;

namespace Bones3Rebuilt.Remeshing.Voxel
{
    /// <summary>
    /// A utility class for combining quads on a plane in a greedy fashion.
    /// </summary>
    internal class GreedyMesher
    {
        /// <summary>
        /// A quad within the greedy mesher awaiting combining.
        /// </summary>
        internal struct Quad
        {
            /// <summary>
            /// The rotation index of this quad.
            /// </summary>
            public int Rotation { get; }

            /// <summary>
            /// The texture index of this quad.
            /// </summary>
            public int TextureIndex { get; }

            /// <summary>
            /// Whether or not this quad is rendered or ignored.
            /// </summary>
            public bool Active { get; }

            /// <summary>
            /// Created a new, active quad.
            /// </summary>
            /// <param name="rotation">The rotation index.</param>
            /// <param name="texture">The texture index.</param>
            public Quad(int rotation, int texture)
            {
                Rotation = rotation;
                TextureIndex = texture;
                Active = true;
            }
        }

        /// <summary>
        /// A quad position on the plane.
        /// </summary>
        struct QuadPos
        {
            /// <summary>
            /// Gets the quad X position.
            /// </summary>
            /// <value>The quad X.</value>
            public int X { get; }

            /// <summary>
            /// Gets the quad Y position.
            /// </summary>
            /// <value>The quad Y.</value>
            public int Y { get; }

            public Quad Quad { get; }

            /// <summary>
            /// Creates a new quad position.
            /// </summary>
            /// <param name="x">The quad X.</param>
            /// <param name="y">The quad Y.</param>
            /// <param name="y">The quad data.</param>
            public QuadPos(int x, int y, Quad quad)
            {
                X = x;
                Y = y;
                Quad = quad;
            }
        }

        private readonly QuadBuilder m_QuadBuilder;
        private readonly bool m_EnableUVs;
        private readonly Quad[] m_Quads;
        private readonly int m_ChunkSize;

        /// <summary>
        /// Creates a new greedy mesher.
        /// </summary>
        /// <param name="chunkSize">The size of the chunk being meshed.</param>
        /// <param name="enableUVs">Whether or not to generate UVs.</param>
        public GreedyMesher(GridSize chunkSize, bool enableUVs)
        {
            m_ChunkSize = chunkSize.Value;
            m_EnableUVs = enableUVs;
            m_Quads = new Quad[m_ChunkSize * m_ChunkSize];
            m_QuadBuilder = new QuadBuilder(enableUVs);
        }

        /// <summary>
        /// Sets the given quad on the grid to include or exclude it from the meshing.
        /// </summary>
        /// <param name="x">The x position on the grid.</param>
        /// <param name="y">The y position on the grid.</param>
        /// <param name="state">The quad.</param>
        /// <remarks>
        /// Quads with the same state are combined, quads with different states are not combined.
        /// </remarks>
        public void SetQuad(int x, int y, Quad state)
        {
            if (!m_EnableUVs && state.Active)
                state = new Quad(0, 0);

            if (x < 0 || x >= m_ChunkSize ||
                y < 0 || y >= m_ChunkSize)
                throw new System.ArgumentOutOfRangeException($"Quad position ({x}, {y}) not within chunk!");

            m_Quads[x * m_ChunkSize + y] = state;
        }

        /// <summary>
        /// Gets the quad at the given plane position.
        /// </summary>
        /// <param name="a">The plane X coordinate.</param>
        /// <param name="b">The plane Y coordinate.</param>
        /// <returns>The quad at the given position.</returns>
        public Quad GetQuad(int a, int b)
        {
            return m_Quads[a * m_ChunkSize + b];
        }

        /// <summary>
        /// Runs a greedy meshing algorithm over a single axis.
        /// </summary>
        /// <param name="offset">The offset along the plane axis.</param>
        /// <param name="side">The side of the block being targeted.</param>
        /// <param name="mesh">The proc mesh to add the vertices to.</param>
        public void Mesh(int offset, int side, ProcMesh mesh)
        {
            m_QuadBuilder.Mesh = mesh;

            foreach (var q in PlaneIterator())
            {
                if (!q.Quad.Active)
                    continue;

                FindNextQuad(q, out int w, out int h);

                var quadMesh = new QuadBuilder.QuadMesh
                {
                    X = q.X,
                    Y = q.Y,
                    W = w - q.X + 1,
                    H = h - q.Y + 1,
                    Side = side,
                    Offset = offset,
                    TextureIndex = q.Quad.TextureIndex,
                    TextureRotation = q.Quad.Rotation,
                };

                m_QuadBuilder.WriteQuad(quadMesh);
                ClearQuad(q, w, h);
            }

            m_QuadBuilder.Mesh = null;
        }

        /// <summary>
        /// Resets the data within the quad to default.
        /// </summary>
        /// <param name="q">The quad position.</param>
        /// <param name="w">The quad width.</param>
        /// <param name="h">The quad height.</param>
        private void ClearQuad(QuadPos q, int w, int h)
        {
            for (int a = w; a >= q.X; a--)
                for (int b = h; b >= q.Y; b--)
                    SetQuad(a, b, default);
        }

        /// <summary>
        /// Finds the width and height of the quad at the given position.
        /// </summary>
        /// <param name="q">The quad position.</param>
        /// <param name="w">The quad width to write to.</param>
        /// <param name="h">The quad height to write to.</param>
        private void FindNextQuad(QuadPos q, out int w, out int h)
        {
            int maxH = m_ChunkSize - 1;

            for (w = q.X; ; w++)
            {
                for (h = q.Y; ; h++)
                {
                    if (h == maxH || !GetQuad(w, h + 1).Equals(q.Quad))
                        break;
                }

                if (h < maxH)
                    maxH = h;

                if (w == m_ChunkSize - 1 || !GetQuad(w + 1, q.Y).Equals(q.Quad))
                    break;
            }
        }

        /// <summary>
        /// Iterates over all quads within the plane.
        /// </summary>
        /// <returns>A quad iterator.</returns>
        private IEnumerable<QuadPos> PlaneIterator()
        {
            for (int x = 0; x < m_ChunkSize; x++)
                for (int y = 0; y < m_ChunkSize; y++)
                    yield return new QuadPos(x, y, GetQuad(x, y));
        }
    }
}
