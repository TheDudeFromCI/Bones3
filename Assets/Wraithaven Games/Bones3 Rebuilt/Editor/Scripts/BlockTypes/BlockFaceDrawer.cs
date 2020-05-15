using UnityEditor;

using UnityEngine;

namespace Bones3Rebuilt
{
    [CustomPropertyDrawer(typeof(BlockFace))]
    public class BlockFaceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            position.width -= 100;

            int oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            EditorGUI.PropertyField(position, property.FindPropertyRelative("rotation"), GUIContent.none);

            position.x += position.width;
            position.width = 50;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("textureAtlas"), GUIContent.none);

            position.x += position.width;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("textureIndex"), GUIContent.none);

            EditorGUI.EndProperty();
            EditorGUI.indentLevel = oldIndent;
        }
    }
}
