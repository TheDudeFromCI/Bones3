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
    }
}
