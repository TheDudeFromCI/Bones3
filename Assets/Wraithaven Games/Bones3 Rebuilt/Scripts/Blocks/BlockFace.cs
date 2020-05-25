using UnityEngine;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// Details about the face of a block type.
    /// </summary>
    [System.Serializable]
    public class BlockFace
    {
        [SerializeField] protected FaceRotation m_Rotation;
        [SerializeField] protected int m_MaterialID;
        [SerializeField] protected int m_TextureID;

        /// <summary>
        /// The texture rotation of this block face.
        /// </summary>
        public FaceRotation Rotation => m_Rotation;

        /// <summary>
        /// The material index of this block face.
        /// </summary>
        public int MaterialID => m_MaterialID;

        /// <summary>
        /// The texture index of this block face.
        /// </summary>
        public int TextureID => m_TextureID;
    }
}