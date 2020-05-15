using UnityEditor;

namespace Bones3Rebuilt
{
    [CustomEditor(typeof(BlockWorld))]
    public class BlockWorldEditor : Editor
    {
        void OnSceneGUI()
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
