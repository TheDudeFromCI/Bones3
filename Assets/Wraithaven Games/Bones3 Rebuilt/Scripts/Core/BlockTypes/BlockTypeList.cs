using System.Collections.Generic;

namespace Bones3Rebuilt
{
    /// <summary>
    /// A list of block properties which are used to make up a world.
    /// </summary>
    public class BlockTypeList : IBlockTypeList
    {
        /// <summary>
        /// The maximum number of block types which can be added to this list.
        /// </summary>
        public const ushort MAX_BLOCK_TYPES = 65535;

        private readonly List<BlockType> m_BlockTypes = new List<BlockType>();

        /// <inheritdoc cref="IBlockTypeList"/>
        public int Count => m_BlockTypes.Count;

        /// <inheritdoc cref="IBlockTypeList"/>
        public ushort NextBlockID => (ushort) Count;

        /// <summary>
        /// Creates a new block type list and initializes it with ungenerated and air blocks.
        /// </summary>
        public BlockTypeList()
        {
            AddBlockType(new BlockBuilder(NextBlockID)
                .Name("Ungenerated")
                .Solid(false)
                .Visible(false)
                .Build());

            AddBlockType(new BlockBuilder(NextBlockID)
                .Name("Air")
                .Solid(false)
                .Visible(false)
                .Build());
        }

        /// <inheritdoc cref="IBlockTypeList"/>
        public BlockType GetBlockType(ushort id)
        {
            return m_BlockTypes[id];;
        }

        /// <inheritdoc cref="IBlockTypeList"/>
        public void AddBlockType(BlockType blockType)
        {
            if (Count == MAX_BLOCK_TYPES)
                throw new System.ArgumentException($"Too many block types! Max: {MAX_BLOCK_TYPES}");

            if (blockType.ID != NextBlockID)
                throw new System.ArgumentException($"Block ID (blockType.ID) does not match next available ID ({NextBlockID})!");

            m_BlockTypes.Add(blockType);
        }

        /// <inheritdoc cref="IBlockTypeList"/>
        public void RemoveBlockType(BlockType blockType)
        {
            if (blockType == null)
                return;

            if (!m_BlockTypes.Contains(blockType))
                return;

            if (blockType.ID < 2)
                throw new System.AccessViolationException($"Cannot remove protected block type {blockType}!");

            m_BlockTypes.Remove(blockType);
        }
    }
}
