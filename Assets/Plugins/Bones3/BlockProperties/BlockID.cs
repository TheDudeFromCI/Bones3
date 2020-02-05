namespace WraithavenGames.Bones3.BlockProperties
{
    /// <summary>
    /// A block ID is used to represent a block, its ID, and a set of properties for it. This
    /// information is largely used in chunk processing and background tasks such as chunk
    /// remeshing.
    /// </summary>
    public struct BlockID
    {
        /// <summary>
        /// This is the ID of the block. Each block has a unquie id value to represent it.
        /// </summary>
        public ushort id;

        /// <summary>
        /// A boolean flag for whether or not this block type has collision information
        /// attached to it.
        /// </summary>
        public bool hasCollision;

        /// <summary>
        /// A boolean flag for whether or not this block type has double-sided meshes. If this
        /// is true, additional mesh faces are generated for the inside surfaces of the block
        /// which originally would have been hidden.
        /// </summary>
        public bool viewInsides;

        /// <summary>
        /// A boolean flag for whether or not this block type is transparent. If true, surrounding
        /// blocks will still generate faces that would normally be occluded by this block.
        /// </summary>
        public bool transparent;

        /// <summary>
        /// A boolean flag for whether or not thiss block type requires faces to be sorted. If
        /// true, a behavior is added to chunks holding this block type which sort the faces of
        /// the submesh for this material to properly render to the main camera. This action
        /// occurs each frame, and may be costly in on lower-end hardware, or in levels where a
        /// large number of this block type exists.
        /// <br>
        /// This flag only sorts blocks of this material with itself within a single chunk,
        /// meaning that it has no effort on the depth sorting of other transparent materials
        /// with this flag, or with other blocks of this same type in other chunks.
        /// </summary>
        public bool depthSort;
    }
}