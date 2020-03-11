using UnityEngine;
using System.Collections;

namespace WraithavenGames.Bones3.BlockProperties
{
    public class BlockTypeList : ScriptableObject, ISerializationCallbackReceiver
    {
        private ArrayList materials = new ArrayList();

        #region MaterialHandling
        public MaterialBlock GetMaterialProperties(Material material)
        {
            if (material == null)
                return null;

            foreach (MaterialBlock type in materials)
                if (type.Material == material)
                    return type;

            MaterialBlock block = new MaterialBlock(material, (ushort)(materials.Count + 1));
            materials.Add(block);
            return block;
        }

        public MaterialBlock GetMaterialProperties(ushort id)
        {
            if (id == 0)
                return null;

            id--;
            if (id < materials.Count)
                return materials[id] as MaterialBlock;

            throw new System.Exception($"Unknown Material: {id}. (Actual: {materials.Count})");
        }

        public int GetLength()
        {
            lock (materials)
            {
                return materials.Count;
            }
        }

        public int GetVisibleLength()
        {
            int count = 0;
            foreach (MaterialBlock type in materials)
                if (!type.HiddenInInspector)
                    count++;

            return count;
        }

        public MaterialBlock GetAt(int index)
        {
            if (index < 0)
                return null;

            if (index >= materials.Count)
                return null;
            return materials[index] as MaterialBlock;
        }

        public MaterialBlock GetVisibleAt(int index)
        {
            if (index < 0)
                return null;

            int i = 0;
            foreach (MaterialBlock type in materials)
                if (!type.HiddenInInspector)
                {
                    if (i == index)
                        return type;
                    i++;
                }

            return null;
        }
        #endregion

        #region SerializationHandling
        [SerializeField] private MaterialBlock[] tempArray;

        public void OnBeforeSerialize()
        {
            tempArray = new MaterialBlock[materials.Count];

            for (int i = 0; i < materials.Count; i++)
                tempArray[i] = materials[i] as MaterialBlock;
        }

        public void OnAfterDeserialize()
        {
            materials.Clear();

            if (tempArray == null)
                return;

            materials.Capacity = tempArray.Length;
            for (int i = 0; i < tempArray.Length; i++)
                materials.Add(tempArray[i]);

            tempArray = null;
        }

        public BlockTypeList Copy()
        {
            BlockTypeList list = ScriptableObject.CreateInstance<BlockTypeList>();

            list.materials = new ArrayList();
            list.materials.Capacity = Mathf.Max(list.materials.Capacity, materials.Count);
            foreach (MaterialBlock mat in materials)
                list.materials.Add(mat);

            return list;
        }
        #endregion
    }
}
