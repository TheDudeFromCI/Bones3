namespace Bones3Rebuilt
{
    /// <summary>
    /// A voxel position within a 3D grid.
    /// </summary>
    public interface IVoxelPosition
    {
        /// <summary>
        /// The position along the X axis.
        /// </summary>
        /// <value>The X position.</value>
        int X { get; set; }

        /// <summary>
        /// The position along the Y axis.
        /// </summary>
        /// <value>The Y position.</value>
        int Y { get; set; }

        /// <summary>
        /// The position along the Z axis.
        /// </summary>
        /// <value>The Z position.</value>
        int Z { get; set; }
    }

    public static class VoxelPositionUtils
    {
        /// <summary>
        /// Shifts the voxel position along the given direction axis.
        /// </summary>
        /// <param name="side">The side of the block to shift along.</param>
        /// <param name="units">The number of units.</param>
        /// <returns>The modified voxel position</returns>
        public static T ShiftAlongDirection<T>(this T pos, int side, int units = 1)
            where T : IVoxelPosition
        {
            switch (side)
            {
                case 0:
                    pos.X += units;
                    break;

                case 1:
                    pos.X -= units;
                    break;

                case 2:
                    pos.Y += units;
                    break;

                case 3:
                    pos.Y -= units;
                    break;

                case 4:
                    pos.Z += units;
                    break;

                case 5:
                    pos.Z -= units;
                    break;

                default:
                    throw new System.ArgumentException($"Unknown side {side}!");
            }

            return pos;
        }

        /// <summary>
        /// Checks if this voxel position exists within the target grid size.
        /// </summary>
        /// <param name="grid">The grid size to test against.</param>
        /// <returns>True if this position is withint the grid. False otherwise.</returns>
        public static bool IsWithinGrid(this IVoxelPosition pos, GridSize grid)
        {
            return (pos.X & grid.Mask) == pos.X &&
                (pos.Y & grid.Mask) == pos.Y &&
                (pos.Z & grid.Mask) == pos.Z;
        }

        /// <summary>
        /// Gets the array index for a local voxel position within a grid.
        /// </summary>
        /// <param name="grid">The grid to compare within.</param>
        /// <returns>The array index at the given location.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// If the voxel position is negative or greater than or equal to the
        /// grid size along any axis.
        /// </exception>
        public static int Index(this IVoxelPosition pos, GridSize grid)
        {
            if (!pos.IsWithinGrid(grid))
                throw new System.ArgumentOutOfRangeException($"Voxel position ({pos}) is outside of this grid size ({grid})!");

            int size = grid.Value;
            return pos.X * size * size + pos.Y * size + pos.Z;
        }
    }
}
