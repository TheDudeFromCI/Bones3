namespace Bones3Rebuilt
{
    /// <summary>
    /// The rotation of a block face's texture.
    /// </summary>
    public enum FaceRotation
    {
        Normal,
        Clockwise90,
        Clockwise180,
        Clockwise270,

        Mirrored,
        MirroredClockwise90,
        MirroredClockwise180,
        MirroredClockwise270,

        /// <summary>
        /// Sets the rotation based on the block's position, allowing
        /// more natural looking texturing on terrain.
        /// </summary>
        Random
    }
}
