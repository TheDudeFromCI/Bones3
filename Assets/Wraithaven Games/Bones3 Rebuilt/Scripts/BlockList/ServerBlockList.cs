namespace WraithavenGames.Bones3
{
    /// <summary>
    /// A collection of block types which exist on this embedded world server.
    /// /// </summary>
    public class ServerBlockList
    {
        private readonly ServerBlockType[] m_BlockTypes = new ServerBlockType[64];

        /// <summary>
        /// Gets the internal capacity of this block type list.
        /// </summary>
        internal int Capacity => m_BlockTypes.Length;

        /// <summary>
        /// Creates a new default server block list.
        /// </summary>
        internal ServerBlockList()
        {
            m_BlockTypes[0] = new ServerBlockType(0, 0)
            {
                Name = "Ungenerated",
                Solid = false,
                Visible = false,
                Transparent = false
            };

            m_BlockTypes[1] = new ServerBlockType(1, 0)
            {
                Name = "Air",
                Solid = false,
                Visible = false,
                Transparent = false
            };
        }

        /// <summary>
        /// Gets the block with the given block ID.
        /// </summary>
        /// <param name="id">The ID of the block.</param>
        /// <returns>The block type, or Ungenerated if there is no block with the given ID.</returns>
        public ServerBlockType GetBlockType(ushort id)
        {
            if (id >= m_BlockTypes.Length)
                return m_BlockTypes[0];

            return m_BlockTypes[id] ?? m_BlockTypes[0];
        }
    }
}
