namespace WraithavenGames.Bones3
{
    /// <summary>
    /// Contains information about the specific face of a block.
    /// </summary>
    public class ServerBlockFace
    {
        /// <summary>
        /// Gets the texture ID of this block face.
        /// </summary>
        public int TextureID { get; internal set; }

        /// <summary>
        /// Gets the material ID of this block face.
        /// </summary>
        public int MaterialID { get; internal set; }

        /// <summary>
        /// Gets the texture rotation of this block face.
        /// </summary>
        public FaceRotation Rotation { get; internal set; }

        /// <summary>
        /// Creates a new block face.
        /// </summary>
        internal ServerBlockFace() {}
    }
}
