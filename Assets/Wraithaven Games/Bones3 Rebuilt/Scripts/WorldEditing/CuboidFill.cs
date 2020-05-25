using System.Collections.Generic;

namespace Bones3Rebuilt
{
    /// <summary>
    /// A cuboid fill is a simple edit which fills in a cubic area.
    /// </summary>
    public class CuboidFill : IEditBatch, IRegionalEdit
    {
        private IFillPattern m_FillPattern;
        private BlockPosition m_Start;
        private BlockPosition m_End;

        /// <inheritdoc cref="IRegionalEdit"/>
        public void Set(BlockPosition point1, BlockPosition point2, IFillPattern fillPattern)
        {
            m_FillPattern = fillPattern;
            m_Start = MinPos(point1, point2);
            m_End = MaxPos(point1, point2);
        }

        /// <summary>
        /// Creates a new position which uses the smallest elements of the two given positions.
        /// </summary>
        /// <param name="a">The first position.</param>
        /// <param name="b">The second position.</param>
        /// <returns>A position which uses the smallest elements of the two.</returns>
        private BlockPosition MinPos(BlockPosition a, BlockPosition b)
        {
            int x = Min(a.X, b.X);
            int y = Min(a.Y, b.Y);
            int z = Min(a.Z, b.Z);
            return new BlockPosition(x, y, z);
        }

        /// <summary>
        /// Gets the smaller of the two given numbers.
        /// </summary>
        /// <param name="a">The first number.</param>
        /// <param name="b">The second number.</param>
        /// <returns>The smaller number.</returns>
        private int Min(int a, int b)
        {
            if (a < b)
                return a;
            else
                return b;
        }

        /// <summary>
        /// Creates a new position which uses the largest elements of the two given positions.
        /// </summary>
        /// <param name="a">The first position.</param>
        /// <param name="b">The second position.</param>
        /// <returns>A position which uses the largest elements of the two.</returns>
        private BlockPosition MaxPos(BlockPosition a, BlockPosition b)
        {
            int x = Max(a.X, b.X);
            int y = Max(a.Y, b.Y);
            int z = Max(a.Z, b.Z);
            return new BlockPosition(x, y, z);
        }

        /// <summary>
        /// Gets the larger of the two given numbers.
        /// </summary>
        /// <param name="a">The first number.</param>
        /// <param name="b">The second number.</param>
        /// <returns>The larger number.</returns>
        private int Max(int a, int b)
        {
            if (a > b)
                return a;
            else
                return b;
        }

        /// <inheritdoc cref="IEditBatch"/>
        public IEnumerable<BlockPlacement> GetBlocks()
        {
            for (int x = m_Start.X; x <= m_End.X; x++)
                for (int y = m_Start.Y; y <= m_End.Y; y++)
                    for (int z = m_Start.Z; z <= m_End.Z; z++)
                    {
                        var pos = new BlockPosition(x, y, z);
                        yield return new BlockPlacement
                        {
                            Position = pos,
                                BlockID = m_FillPattern.GetBlockID(pos),
                        };
                    }
        }
    }
}
