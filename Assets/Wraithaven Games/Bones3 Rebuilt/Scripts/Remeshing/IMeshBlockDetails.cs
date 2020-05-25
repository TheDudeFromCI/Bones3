namespace Bones3Rebuilt.Remeshing
{
    /// <summary>
    /// Details for how a block should be handled by remesh tasks. This object must be thread safe.
    /// </summary>
    public interface IMeshBlockDetails
    {
        /// <summary>
        /// Gets whether or not this block has collision data.
        /// </summary>
        /// <value>True if this block is solid, false otherwise.</value>
        bool IsSolid { get; }

        /// <summary>
        /// Gets whether or not this block has visual data.
        /// </summary>
        /// <value>True if this block is visible, false otherwise.</value>
        bool IsVisible { get; }

        /// <summary>
        /// Gets the material ID of the given face.
        /// </summary>
        /// <param name="face">The face index.</param>
        /// <returns>The material ID.</returns>
        int GetMaterialID(int face);

        /// <summary>
        /// Gets the texture ID of the given face.
        /// </summary>
        /// <param name="face">The face index.</param>
        /// <returns>The texture ID.</returns>
        int GetTextureID(int face);

        /// <summary>
        /// Gets the rotation of the given face.
        /// </summary>
        /// <param name="face">The face index.</param>
        /// <returns>The face rotation.</returns>
        FaceRotation GetRotation(int face);
    }
}
