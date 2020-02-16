namespace WraithavenGames.Bones3.Meshing
{
    /// <summary>
    /// A quad is an axis-aligned primative generated as a result of the chunk meshing process.
    /// </summary>
    public struct Quad
    {
        /// <summary>
        /// The x position of the quad within the 2D chunk slice.
        /// </summary>
        public int x;

        /// <summary>
        /// The y position of the quad within the 2D chunk slice.
        /// </summary>
        public int y;

        /// <summary>
        /// The width of the quad within the 2D chunk slice.
        /// </summary>
        public int w;

        /// <summary>
        /// The height of the quad within the 2D chunk slice.
        /// </summary>
        public int h;

        /// <summary>
        /// The direction the quad is facing.
        /// <para/>0 = x+
        /// <para/>1 = x-
        /// <para/>2 = y+
        /// <para/>3 = y-
        /// <para/>4 = z+
        /// <para/>5 = z-
        /// </summary>
        public int side;

        /// <summary>
        /// The offset of this quad from the origin of the chunk, along the given side.
        /// </summary>
        public int offset;
    }
}