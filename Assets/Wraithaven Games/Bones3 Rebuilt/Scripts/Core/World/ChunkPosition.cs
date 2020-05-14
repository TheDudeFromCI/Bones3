namespace Bones3Rebuilt
{
    public struct ChunkPosition : IVoxelPosition
    {
        /// <summary>
        /// The chunk position along the X axis.
        /// </summary>
        /// <value>The chunk X position.</value>
        public int X { get; set; }

        /// <summary>
        /// The chunk position along the Y axis.
        /// </summary>
        /// <value>The chunk Y position.</value>
        public int Y { get; set; }

        /// <summary>
        /// The chunk position along the Z axis.
        /// </summary>
        /// <value>The chunk Z position.</value>
        public int Z { get; set; }

        /// <summary>
        /// Creates a new chunk position object.
        /// </summary>
        /// <param name="x">The X position.</param>
        /// <param name="y">The Y position.</param>
        /// <param name="z">The Z position.</param>
        public ChunkPosition(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString() => $"Chunk Pos:[{X}, {Y}, {Z}]";

        public static ChunkPosition operator +(ChunkPosition a, ChunkPosition b) =>
            new ChunkPosition(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        public static ChunkPosition operator *(ChunkPosition a, int b) =>
            new ChunkPosition(a.X * b, a.Y * b, a.Z * b);

        public static ChunkPosition operator &(ChunkPosition a, int b) =>
            new ChunkPosition(a.X & b, a.Y & b, a.Z & b);
    }
}