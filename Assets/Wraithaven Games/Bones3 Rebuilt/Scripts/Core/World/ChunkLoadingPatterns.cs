using UnityEngine;

namespace WraithavenGames.Bones3
{
    public enum ChunkLoadingPattern
    {
        Circle,
    }

    /// <summary>
    /// A simple iterator for iterating over a set of relative local chunk positions.
    /// </summary>
    internal abstract class ChunkLoadPatternIterator
    {
        internal static ChunkLoadPatternIterator Build(ChunkLoadingPattern pattern, Vector3Int extents)
        {
            switch (pattern)
            {
                case ChunkLoadingPattern.Circle:
                    return new CircleChunkLoader(extents);

                default:
                    throw new System.InvalidOperationException("Pattern does not exist!");
            }
        }

        private readonly ChunkPosition[] m_Positions;
        private int m_IteratorPosition;

        /// <summary>
        /// Checks whether or not this iterator has more chunks to load.
        /// </summary>
        internal bool HasNext => m_IteratorPosition < m_Positions.Length;

        /// <summary>
        /// Gets the next relative chunk position in this iterator.
        /// </summary>
        internal ChunkPosition Next => m_Positions[m_IteratorPosition++];

        /// <summary>
        /// Creates a new chunk loading pattern for the given extends.
        /// </summary>
        /// <param name="extents">The radial extends of the bounding box for this loading pattern.</param>
        internal ChunkLoadPatternIterator(Vector3Int extents)
        {
            var volume = 1;
            volume *= extents.x * 2 + 1;
            volume *= extents.y * 2 + 1;
            volume *= extents.z * 2 + 1;

            m_Positions = new ChunkPosition[volume];
            GeneratePositions(m_Positions, extents);
        }

        /// <summary>
        /// Resets this iterator base to the beginning.
        /// </summary>
        internal void Reset()
        {
            m_IteratorPosition = 0;
        }

        /// <summary>
        /// Caches the chunk positions for this loading patterns.
        /// </summary>
        /// <param name="positions">The chunk position buffer to write to.</param>
        /// <param name="extents">The radial extends of the bounding box for this loading pattern.</param>
        protected abstract void GeneratePositions(ChunkPosition[] positions, Vector3Int extents);
    }

    /// <summary>
    /// Loads chunks in a simple circular pattern around the camera.
    /// </summary>
    internal class CircleChunkLoader : ChunkLoadPatternIterator
    {
        /// <summary>
        /// Creates a new single circle chunk loader.
        /// </summary>
        /// <param name="extents">The radial extents for the bounding box for this pattern.</param>
        internal CircleChunkLoader(Vector3Int extents) : base(extents) { }

        /// <inheritdoc cref="ChunkLoadingPattern"/>
        protected override void GeneratePositions(ChunkPosition[] positions, Vector3Int extents)
        {
            int i = 0;
            for (int x = -extents.x; x <= extents.x; x++)
                for (int y = -extents.y; y <= extents.y; y++)
                    for (int z = -extents.z; z <= extents.z; z++)
                        positions[i++] = new ChunkPosition(x, y, z);

            System.Array.Sort(positions, (a, b) =>
                (a.X * a.X + a.Y * a.Y + a.Z * a.Z) - (b.X * b.X + b.Y * b.Y + b.Z * b.Z));
        }
    }
}