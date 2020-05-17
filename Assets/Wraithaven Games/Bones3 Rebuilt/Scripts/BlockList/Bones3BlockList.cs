using Bones3Rebuilt;

using UnityEngine;

#pragma warning disable 649

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// The main container for a block list object.
    /// </summary>
    [AddComponentMenu("Bones3/Texture Atlas"), ExecuteAlways]
    public class Bones3BlockList : MonoBehaviour, IBlockTypeList
    {
        [Tooltip("The block loader in charge of creating default block types when this block list is loaded.")]
        [SerializeField] private UnityBlockLoader m_BlockLoader;

        #region Object Wrapper
        private IBlockTypeList m_BlockList;

        /// <summary>
        /// The texture atlas currently maintained by this behaviour.
        /// </summary>
        /// <value>The raw block list.</value>
        public IBlockTypeList RawBlockList
        {
            get
            {
                if (m_BlockList == null)
                    OnEnable();

                return m_BlockList;
            }
            private set => m_BlockList = value;
        }

        /// <summary>
        /// Called when the block list behaviour is enabled.
        /// </summary>
        void OnEnable()
        {
            if (m_BlockList != null)
                return;

            m_BlockList = new BlockTypeList();
            m_BlockLoader.LoadBlocks(this);
        }

        /// <summary>
        /// Called when the block list behaviour is disabled.
        /// </summary>
        void OnDisable()
        {
            if (m_BlockList == null)
                return;

            m_BlockList = null;
        }
        #endregion

        #region Object API
        /// <inheritdoc cref="IBlockTypeList"/>
        public int Count => RawBlockList.Count;

        /// <inheritdoc cref="IBlockTypeList"/>
        public ushort NextBlockID => RawBlockList.NextBlockID;

        /// <inheritdoc cref="IBlockTypeList"/>
        public BlockType GetBlockType(ushort id) => RawBlockList.GetBlockType(id);

        /// <inheritdoc cref="IBlockTypeList"/>
        public void AddBlockType(BlockType blockType) => RawBlockList.AddBlockType(blockType);

        /// <inheritdoc cref="IBlockTypeList"/>
        public void RemoveBlockType(BlockType blockType) => RawBlockList.RemoveBlockType(blockType);
        #endregion
    }
}
