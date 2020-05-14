namespace Bones3Rebuilt
{
    /// <summary>
    /// A block type and it's given properties.
    /// </summary>
    public class BlockType
    {
        /// <summary>
        /// The Ungenerated block type.
        /// </summary>
        /// <remarks>
        /// This block type means the block has not yet been generated. This could
        /// be because the block falls into a chunk which does not currently exist,
        /// or because the world generator has not targeted this block yet.
        /// </remarks>
        public const ushort UNGENERATED = 0;

        /// <summary>
        /// The Air block type.
        /// </summary>
        public const ushort AIR = 1;

        private readonly BlockFace[] m_Faces;

        /// <summary>
        /// The name of this block type.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }

        /// <summary>
        /// The ID of this block type.
        /// </summary>
        /// <value>The ID.</value>
        public ushort ID { get; }

        /// <summary>
        /// Whether or not this block type is visible.
        /// </summary>
        /// <value>True if this block is visible, false otherwise.</value>
        public bool IsVisible { get; }

        /// <summary>
        /// Whether or not this block type has collision.
        /// </summary>
        /// <value>True if this block has collision, false otherwise.</value>
        public bool IsSolid { get; }

        /// <summary>
        /// Creates a new block type.
        /// </summary>
        /// <param name="id">The ID of this block type.</param>
        /// <param name="name">The name of this block type.</param>
        /// <param name="solid">Whether or not this block is solid.</param>
        /// <param name="visible">Whether or not this block is visible.</param>
        /// <param name="faces">The list of faces for this block type.</param>
        internal BlockType(ushort id, string name, bool solid, bool visible, BlockFace[] faces)
        {
            ID = id;
            Name = name;
            IsSolid = solid;
            IsVisible = visible;
            m_Faces = faces;
        }

        /// <summary>
        /// Gets the properties for a specific face of this block type.
        /// </summary>
        /// <param name="index">The face index.</param>
        /// <returns>The block face properties.</returns>
        public BlockFace Face(int index) => m_Faces[index];

        public override string ToString() => $"Block:[{ID}) {Name}]";
    }
}
