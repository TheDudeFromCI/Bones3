namespace Bones3Rebuilt
{
    public struct BlockPosition : IVoxelPosition
    {
        /// <summary>
        /// The block position along the X axis.
        /// </summary>
        /// <value>The block X position.</value>
        public int X { get; set; }

        /// <summary>
        /// The block position along the Y axis.
        /// </summary>
        /// <value>The block Y position.</value>
        public int Y { get; set; }

        /// <summary>
        /// The block position along the Z axis.
        /// </summary>
        /// <value>The block Z position.</value>
        public int Z { get; set; }

        /// <summary>
        /// Creates a new block position object.
        /// </summary>
        /// <param name="x">The X position.</param>
        /// <param name="y">The Y position.</param>
        /// <param name="z">The Z position.</param>
        public BlockPosition(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Converts this block position to a chunk position based on the given chunk size.
        /// </summary>
        /// <param name="chunkSize">The size of the chunks.</param>
        /// <returns>The chunk position this block position is in.</returns>
        public ChunkPosition ToChunkPosition(GridSize chunkSize)
        {
            return new ChunkPosition
            {
                X = X >> chunkSize.IntBits,
                Y = Y >> chunkSize.IntBits,
                Z = Z >> chunkSize.IntBits,
            };
        }

        public override string ToString() => $"Block Pos:[{X}, {Y}, {Z}]";

        public static BlockPosition operator +(BlockPosition a, BlockPosition b) =>
            new BlockPosition(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        public static BlockPosition operator *(BlockPosition a, int b) =>
            new BlockPosition(a.X * b, a.Y * b, a.Z * b);

        public static BlockPosition operator &(BlockPosition a, int b) =>
            new BlockPosition(a.X & b, a.Y & b, a.Z & b);
    }
}