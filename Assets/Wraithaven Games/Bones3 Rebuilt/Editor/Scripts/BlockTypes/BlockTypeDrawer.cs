using Bones3Rebuilt;

using UnityEditor;

using UnityEngine;

namespace WraithavenGames.Bones3
{
    [CustomPropertyDrawer(typeof(BlockType))]
    public class BlockTypeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            position.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("visible"));

            position.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("solid"));

            position.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(position, "Faces");

            SerializedProperty faces = property.FindPropertyRelative("faces");

            EditorGUI.indentLevel++;
            EditorGUIUtility.labelWidth /= 3f;

            for (int j = 0; j < 6; j++)
            {
                position.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(position, faces.GetArrayElementAtIndex(j),
                    new GUIContent(j + ""));
            }

            EditorGUIUtility.labelWidth *= 3f;
            EditorGUI.indentLevel--;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            EditorGUIUtility.singleLineHeight * 9f;
    }
}
