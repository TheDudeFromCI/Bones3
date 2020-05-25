using UnityEngine;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// A list of block types and texture atlas which can be applied to a world.
    /// </summary>
    [CreateAssetMenu(fileName = "Blocks", menuName = "Bones3/Block List")]
    public class BlockList : ScriptableObject
    {
        [Tooltip("The list of material types to be referenced by the block types.")]
        [SerializeField] protected Material[] m_Materials;

        [Tooltip("The list of block types in this list.")]
        [SerializeField] protected BlockType[] m_BlockTypes;

        /// <summary>
        /// Gets the block with the given block ID.
        /// </summary>
        /// <param name="id">The block ID.</param>
        /// <returns>The block type.</returns>
        public BlockType GetBlockType(int id) => m_BlockTypes[id];

        /// <summary>
        /// Gets the material with the specified material ID.
        /// </summary>
        /// <param name="id">The material ID.</param>
        /// <returns>The material.</returns>
        public Material GetMaterial(int id) => m_Materials[id];

        /// <summary>
        /// Gets the number of block types in this list.
        /// </summary>
        public int BlockCount => m_BlockTypes.Length;

        /// <summary>
        /// Gets the number of materials in this list.
        /// </summary>
        public int MaterialCount => m_Materials.Length;
    }
}
