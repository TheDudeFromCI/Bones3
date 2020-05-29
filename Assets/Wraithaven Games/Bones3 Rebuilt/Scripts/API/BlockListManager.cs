using UnityEngine;

namespace WraithavenGames.Bones3
{
    [AddComponentMenu("Bones3/Block List Manager")]
    [DisallowMultipleComponent]
    public class BlockListManager : MonoBehaviour
    {
        [Tooltip("The block list the manager reads from.")]
        [SerializeField] protected BlockList m_BlockList;

        /// <summary>
        /// Gets the number of block types in this list.
        /// </summary>
        public int BlockCount => m_BlockList?.BlockCount ?? 0;

        /// <summary>
        /// Gets the number of materials in this list.
        /// </summary>
        public int MaterialCount => m_BlockList?.MaterialCount ?? 0;

        /// <summary>
        /// Gets the block with the given block ID.
        /// </summary>
        /// <param name="id">The block ID.</param>
        /// <returns>The block type.</returns>
        public BlockType GetBlockType(int id) => m_BlockList.GetBlockType(id);

        /// <summary>
        /// Gets the material with the specified material ID.
        /// </summary>
        /// <param name="id">The material ID.</param>
        /// <returns>The material.</returns>
        public Material GetMaterial(int id) => m_BlockList.GetMaterial(id);
    }
}
