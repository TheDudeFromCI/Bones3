namespace WraithavenGames.Bones3
{
    /// <summary>
    /// Contains information about a block.
    /// </summary>
    public class ServerBlockType
    {
        private readonly ServerBlockFace[] m_Faces;

        /// <summary>
        /// Gets the ID value of this block type within the block list.
        /// </summary>
        public ushort ID { get; }

        /// <summary>
        /// Gets the name of this block type.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets whether or not this block type is has collision.
        /// </summary>
        public bool Solid { get; internal set; }

        /// <summary>
        /// Gets whether or not this block type is visible.
        /// </summary>
        public bool Visible { get; internal set; }

        /// <summary>
        /// Gets whether or not this block type is transparent.
        /// </summary>
        public bool Transparent { get; internal set; }

        /// <summary>
        /// Gets the number of faces on this block.
        /// </summary>
        public int FaceCount => m_Faces.Length;

        /// <summary>
        /// Creates a new block type.
        /// </summary>
        /// <param name="id">The ID of this block type.</param>
        /// <param name="faceCount">The number of faces on this block type. Defaults to 6.</param>
        internal ServerBlockType(ushort id, int faceCount = 6)
        {
            ID = id;
            m_Faces = new ServerBlockFace[faceCount];
        }

        /// <summary>
        /// Gets the specific face of this block type.
        /// </summary>
        /// <param name="index">The face index.</param>
        /// <returns>The block face.</returns>
        public ServerBlockFace Face(int index) => m_Faces[index];
    }
}
