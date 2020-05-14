namespace Bones3Rebuilt
{
    /// <summary>
    /// The properties for a single face of a block type.
    /// </summary>
    public class BlockFace
    {
        /// <summary>
        /// The side of the block this face is on.
        /// </summary>
        /// <value>The block side index.</value>
        public int Side { get; }

        /// <summary>
        /// The rotation value for this face's texture.
        /// </summary>
        /// <value>The rotation value of this face.</value>
        public FaceRotation Rotation { get; }

        /// <summary>
        /// The index of the texture within the texture atlas.
        /// </summary>
        /// <value>The texture index.</value>
        public int TextureIndex { get; }

        /// <summary>
        /// The index of the texture atlas to pull the texture from.
        /// </summary>
        /// <value>The texture atlas index.</value>
        public int TextureAtlas { get; }

        /// <summary>
        /// Creates a new block face.
        /// </summary>
        /// <param name="side">The side of the block this face is on.</param>
        /// <param name="rotation">The texture rotation for this block face.</param>
        /// <param name="textureIndex">The texture index for this block face.</param>
        /// <param name="textureAtlas">The texture atlas for this block face.</param>
        internal BlockFace(int side, FaceRotation rotation, int textureIndex, int textureAtlas)
        {
            Side = side;
            Rotation = rotation;
            TextureIndex = textureIndex;
            TextureAtlas = textureAtlas;
        }
    }
}
