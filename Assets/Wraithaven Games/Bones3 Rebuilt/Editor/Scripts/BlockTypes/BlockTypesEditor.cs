/*
using UnityEditor;

using UnityEditorInternal;

using UnityEngine;

namespace Bones3Rebuilt
{
    [CustomEditor(typeof(BlockTypeList))]
    public class BlockTypesEditor : Editor
    {
        private SerializedProperty blockTypes;
        private SerializedProperty textureAtlases;
        private BlockTypeList obj;

        private ReorderableList blockTypeList;
        private ReorderableList textureAtlasesList;
        private BlockTypesPreview preview;
        private Vector2 scrollPosition;
        private ObjectPicker objectPicker;

        void OnEnable()
        {
            blockTypes = serializedObject.FindProperty("blockTypes");
            textureAtlases = serializedObject.FindProperty("textureAtlases");
            obj = target as BlockTypeList;

            preview = new BlockTypesPreview(obj);
            objectPicker = new ObjectPicker();

            InitTextureAtlasArray();
            InitBlockTypesArray();
        }

        void OnDisable()
        {
            preview.Dispose();
        }

        void InitBlockTypesArray()
        {
            blockTypeList = new ReorderableList(serializedObject,
                blockTypes, false, true, true, true);

            blockTypeList.drawHeaderCallback += DrawBlockListHeader;
            blockTypeList.drawElementCallback += DrawBlockElement;
            blockTypeList.onRemoveCallback += RemoveBlockType;
            blockTypeList.onAddCallback += AddBlockType;
            blockTypeList.onCanAddCallback += CanAddBlockType;
            blockTypeList.onCanRemoveCallback += CanRemoveBlockType;
            blockTypeList.onSelectCallback += RebuildPreviewMesh;
        }

        void InitTextureAtlasArray()
        {
            textureAtlasesList = new ReorderableList(serializedObject,
                textureAtlases, false, true, true, true);

            textureAtlasesList.drawHeaderCallback += DrawTextureListHeader;
            textureAtlasesList.drawElementCallback += DrawTextureAtlasElement;
            textureAtlasesList.onRemoveCallback += RemoveTexture;
            textureAtlasesList.onAddCallback += AddTexture;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var newTexture = objectPicker.PickerObject("textures");
            if (newTexture != null)
            {
                int index = textureAtlases.arraySize;
                textureAtlases.InsertArrayElementAtIndex(index);
                textureAtlases.GetArrayElementAtIndex(index).objectReferenceValue = newTexture;
            }

            ValidateTextureList();

            textureAtlasesList.DoLayoutList();

            if (textureAtlases.arraySize == 0)
                EditorGUILayout.HelpBox("Must have at least one texture atlas!", MessageType.Warning);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (blockTypeList.GetHeight() >= 256)
            {
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(256));
                blockTypeList.DoLayoutList();
                EditorGUILayout.EndScrollView();
            }
            else
                blockTypeList.DoLayoutList();

            if (blockTypeList.index >= 0)
            {
                EditorGUILayout.Space();
                DrawBlockType(blockTypeList.index);
            }

            serializedObject.ApplyModifiedProperties();
        }

        void ValidateTextureList()
        {
            for (int i = 0; i < textureAtlases.arraySize; i++)
            {
                if (textureAtlases.GetArrayElementAtIndex(i).objectReferenceValue == null)
                {
                    textureAtlasesList.index = i;
                    RemoveTexture(textureAtlasesList);
                }
            }
        }

        public override bool UseDefaultMargins() => false;
        public override bool HasPreviewGUI() => true;
        public override void OnPreviewGUI(Rect r, GUIStyle background) => preview.Render(r, background);
        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background) => preview.Render(r, background);
        public void RebuildPreviewMesh(ReorderableList list) => preview.RebuildMesh(obj[(ushort) list.index]);

        void DrawBlockListHeader(Rect rect) =>
            EditorGUI.LabelField(rect, "Block Types", GUI.skin.GetStyle("BoldLabel"));

        void DrawTextureListHeader(Rect rect) =>
            EditorGUI.LabelField(rect, "Texture Atlases", GUI.skin.GetStyle("BoldLabel"));

        void DrawBlockType(int index)
        {
            var serializedBlock = blockTypes.GetArrayElementAtIndex(index);
            var type = obj[(ushort) index];

            EditorGUI.BeginDisabledGroup(type.IsProtectedBlockType || !type.IsUsed);

            DrawBlockHeader(serializedBlock, index);

            if (type.IsUsed)
                DrawBlockProperties(serializedBlock, index);

            EditorGUI.EndDisabledGroup();
        }

        void DrawBlockHeader(SerializedProperty serializedBlock, int index)
        {
            EditorGUILayout.BeginHorizontal();

            string label = $"{index}) ";
            float idWidth = GUI.skin.label.CalcSize(new GUIContent(label)).x;
            EditorGUILayout.LabelField(label, GUI.skin.GetStyle("BoldLabel"), GUILayout.Width(idWidth));
            EditorGUILayout.PropertyField(serializedBlock.FindPropertyRelative("name"), GUIContent.none);

            EditorGUILayout.EndHorizontal();
        }

        void DrawBlockProperties(SerializedProperty serializedBlock, int index)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedBlock, GUIContent.none);
            if (EditorGUI.EndChangeCheck() && blockTypeList.index == index)
            {
                serializedObject.ApplyModifiedProperties();
                preview.RebuildMesh(obj[(ushort) index]);
            }
        }

        void AddBlockType(ReorderableList list)
        {
            for (ushort i = 0; i < blockTypes.arraySize; i++)
            {
                if (obj[i].IsUsed)
                    continue;

                CreateEmptyBlockType(blockTypes.GetArrayElementAtIndex(i), "New Block", i);
                return;
            }

            int blockId = blockTypes.arraySize;
            blockTypes.InsertArrayElementAtIndex(blockId);
            var newBlock = blockTypes.GetArrayElementAtIndex(blockId);
            CreateEmptyBlockType(newBlock, "New Block", blockId);
        }

        void CreateEmptyBlockType(SerializedProperty block, string name, int id)
        {
            block.FindPropertyRelative("name").stringValue = name;
            block.FindPropertyRelative("used").boolValue = true;
            block.FindPropertyRelative("visible").boolValue = true;
            block.FindPropertyRelative("solid").boolValue = true;
            block.FindPropertyRelative("id").intValue = id;

            var faces = block.FindPropertyRelative("faces");
            for (int j = 0; j < 6; j++)
            {
                var face = faces.GetArrayElementAtIndex(j);
                face.FindPropertyRelative("rotation").enumValueIndex = 0;
                face.FindPropertyRelative("textureAtlas").intValue = 0;
                face.FindPropertyRelative("textureIndex").intValue = 0;
            }
        }

        void RemoveBlockType(ReorderableList list)
        {
            var block = blockTypes.GetArrayElementAtIndex(list.index);
            CreateEmptyBlockType(block, "Reserved ID", list.index);
            block.FindPropertyRelative("used").boolValue = false;

            for (int i = blockTypes.arraySize - 1; i >= 0; i--)
            {
                block = blockTypes.GetArrayElementAtIndex(i);
                if (!block.FindPropertyRelative("used").boolValue)
                    blockTypes.DeleteArrayElementAtIndex(i);
            }
        }

        bool CanAddBlockType(ReorderableList list)
        {
            return obj.UnusedIDs > 0 || obj.Count < BlockTypes.MAX_BLOCK_TYPES;
        }

        bool CanRemoveBlockType(ReorderableList list)
        {
            return !(target as BlockTypes) [(ushort) list.index].IsProtectedBlockType;
        }

        void DrawTextureAtlasElement(Rect rect, int index, bool active, bool focused) =>
            EditorGUI.PropertyField(rect, textureAtlases.GetArrayElementAtIndex(index), new GUIContent($"Texture Atlas {index}"));

        void RemoveTexture(ReorderableList list)
        {
            if (textureAtlases.GetArrayElementAtIndex(list.index).objectReferenceValue != null)
                textureAtlases.DeleteArrayElementAtIndex(list.index);

            textureAtlases.DeleteArrayElementAtIndex(list.index);
        }

        void AddTexture(ReorderableList list) =>
            objectPicker.ShowPickerWindow<Material>("textures", null, false);

        void DrawBlockElement(Rect rect, int index, bool active, bool focused)
        {
            string label = blockTypes.GetArrayElementAtIndex(index).FindPropertyRelative("name").stringValue;
            EditorGUI.LabelField(rect, $"{index}) {label}", GUI.skin.GetStyle("BoldLabel"));
        }
    }
}
*/
