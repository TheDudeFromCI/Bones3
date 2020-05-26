using UnityEngine;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// A block type object which describes how a block looks and acts.
    /// </summary>
    [System.Serializable]
    public class BlockType
    {
        [Tooltip("The name of this block type.")]
        [SerializeField] protected string m_Name;

        [Tooltip("The ID of this block type.")]
        [SerializeField] protected int m_ID;

        [Tooltip("Whether or not this block has collision.")]
        [SerializeField] protected bool m_Solid;

        [Tooltip("Whether or not this block is visible.")]
        [SerializeField] protected bool m_Visible;

        [Tooltip("Whether or not this block is transparent.")]
        [SerializeField] protected bool m_Transparent;

        [Tooltip("The list of face properties for this block type.")]
        [SerializeField] protected BlockFace[] m_Faces = new BlockFace[6];

        /// <summary>
        /// Gets the ID of this block type.
        /// </summary>
        public int ID => m_ID;

        /// <summary>
        /// Gets the name of this block type.
        /// </summary>
        public string Name => m_Name;

        /// <summary>
        /// Gets whether or not this block type has collision.
        /// </summary>
        public bool IsSolid => m_Solid;

        /// <summary>
        /// Gets whether or not this block type is visible.
        /// </summary>
        public bool IsVisible => m_Visible;

        /// <summary>
        /// Gets whether or not this block is transparent.
        /// </summary>
        public bool IsTransparent => m_Transparent;

        /// <summary>
        /// Gets the material ID for the given face.
        /// </summary>
        /// <param name="face">The face index.</param>
        /// <returns>The material ID.</returns>
        public int GetMaterialID(int face) => m_Faces[face].MaterialID;

        /// <summary>
        /// Gets the rotation of the given face.
        /// </summary>
        /// <param name="face">The face index.</param>
        /// <returns>The face rotation.</returns>
        public FaceRotation GetRotation(int face) => m_Faces[face].Rotation;

        /// <summary>
        /// Gets the texture ID of the given face.
        /// </summary>
        /// <param name="face">The face index.</param>
        /// <returns>The texture ID.</returns>
        public int GetTextureID(int face) => m_Faces[face].TextureID;
    }
}