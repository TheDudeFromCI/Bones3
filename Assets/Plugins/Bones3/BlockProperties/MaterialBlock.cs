using UnityEngine;

namespace WraithavenGames.Bones3.BlockProperties
{
    [System.Serializable]
    public class MaterialBlock
    {
        public const int BLOCK_STATE_TRANSPARENT = 1;
        public const int BLOCK_STATE_VIEW_INSIDES = 2;

        public bool Transparent;
        public bool DepthSort;
        public bool ViewInsides;
        public bool HiddenInInspector;

        [SerializeField] private Material m_Material;
        [SerializeField] private ushort m_Id;
        [SerializeField] private bool m_GroupBlocks;

        public Material Material { get { return m_Material; } }
        public ushort Id { get { return m_Id; } }

        public bool GroupBlocks
        {
            get { return m_GroupBlocks; }
            set { m_GroupBlocks = value; }
        }

        public byte BlockState
        {
            get
            {
                byte b = 0;
                if (Transparent) b |= 1 << 0;
                if (ViewInsides) b |= 1 << 1;
                return b;
            }
        }

        public MaterialBlock(Material material, ushort id)
        {
            m_Material = material;
            m_Id = id;
            m_GroupBlocks = true;
        }
    }
}
