using Bones3Rebuilt;

using UnityEngine;

namespace WraithavenGames.Bones3
{
    public abstract class UnityBlockLoader : MonoBehaviour, IBlockLoader
    {
        [Tooltip("The list of texture atlases to use when referencing block materials.")]
        [SerializeField] protected Bones3TextureAtlasList m_TextureAtlasList;

        /// <inheritdoc cref="IBlockLoader"/>
        public abstract void LoadBlocks(IBlockTypeList blockList);
    }
}
