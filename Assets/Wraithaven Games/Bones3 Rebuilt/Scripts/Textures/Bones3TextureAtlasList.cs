using Bones3Rebuilt;

using UnityEngine;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// A container for a list of texture atlases which are used together.
    /// </summary>
    public class Bones3TextureAtlasList : MonoBehaviour
    {
        [Tooltip("The list of texture atlas.")]
        [SerializeField] private Bones3TextureAtlas[] m_AtlasList = new Bones3TextureAtlas[0];

        /// <summary>
        /// Gets a texture atlas container based on the given Bones3 texture atlas object.
        /// </summary>
        /// <param name="atlas">The atlas object.</param>
        /// <returns>
        /// The Bones3 texture atlas object which manages this object, or null if it is
        /// not in this list.
        /// </returns>
        public Bones3TextureAtlas GetAtlas(ITextureAtlas atlas)
        {
            foreach (var a in m_AtlasList)
                if (a.RawAtlas == atlas)
                    return a;

            return null;
        }

        /// <summary>
        /// Gets the atlas at the specified index within this list.
        /// </summary>
        /// <param name="index">The atlas index.</param>
        /// <returns>The atlas.</returns>
        public Bones3TextureAtlas GetAtlas(int index) => m_AtlasList[index];
    }
}
