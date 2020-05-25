using UnityEngine;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// The main behaviour for containing a voxel block world.
    /// </summary>
    [SelectionBase, ExecuteAlways]
    public class BlockWorld : MonoBehaviour
    {
        [Tooltip("The block properties container to use for this world.")]
        [SerializeField] protected BlockList m_Blocks;
    }
}
