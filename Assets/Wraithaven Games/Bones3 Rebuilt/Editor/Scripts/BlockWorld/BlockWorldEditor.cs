using UnityEditor;

using UnityEngine;

namespace WraithavenGames.Bones3.Editor
{
    [CustomEditor(typeof(BlockWorld))]
    public class BlockWorldEditor : UnityEditor.Editor
    {
        private BlockWorld m_BlockWorld;

        protected void OnEnable()
        {
            m_BlockWorld = target as BlockWorld;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(20f);

            if (GUILayout.Button("Save World"))
                m_BlockWorld.SaveWorld();

            if (GUILayout.Button("Clear World"))
                m_BlockWorld.ClearWorld();
        }

        protected void OnSceneGUI()
        {
            if (target == null)
                return;

            var tool = Tools.current;
            if (tool == Tool.Move ||
                tool == Tool.Rotate ||
                tool == Tool.Scale ||
                tool == Tool.Transform ||
                tool == Tool.Rect)
                UnityEditor.EditorTools.EditorTools.SetActiveTool<WorldEditTool>();
        }
    }
}
