using UnityEngine;
using Bones3Rebuilt.Remeshing;
using Bones3Rebuilt;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// A block type object which describes how a block looks and acts.
    /// </summary>
    [System.Serializable]
    public class BlockType : IMeshBlockDetails
    {
        [Tooltip("The name of this block type.")]
        [SerializeField] protected string m_Name;

        [Tooltip("The ID of this block type.")]
        [SerializeField] protected int m_ID;

        [Tooltip("Whether or not this block has collision.")]
        [SerializeField] protected bool m_Solid;

        [Tooltip("Whether or not this block is visible.")]
        [SerializeField] protected bool m_Visible;

        [Tooltip("The list of face properties for this block type.")]
        [SerializeField] protected BlockFace[] m_Faces = new BlockFace[6];

        /// <inheritdoc cref="IMeshBlockDetails"/>
        public int ID => m_ID;

        /// <inheritdoc cref="IMeshBlockDetails"/>
        public string Name => m_Name;

        /// <inheritdoc cref="IMeshBlockDetails"/>
        public bool IsSolid => m_Solid;

        /// <inheritdoc cref="IMeshBlockDetails"/>
        public bool IsVisible => m_Visible;

        /// <inheritdoc cref="IMeshBlockDetails"/>
        public int GetMaterialID(int face) => m_Faces[face].MaterialID;

        /// <inheritdoc cref="IMeshBlockDetails"/>
        public FaceRotation GetRotation(int face) => m_Faces[face].Rotation;

        /// <inheritdoc cref="IMeshBlockDetails"/>
        public int GetTextureID(int face) => m_Faces[face].TextureID;
    }
}