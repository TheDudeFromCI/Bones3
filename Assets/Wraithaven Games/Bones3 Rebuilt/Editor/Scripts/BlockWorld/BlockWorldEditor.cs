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

            if (GUILayout.Button("Load Nearby Chunks"))
                LoadAroundCamera();

            if (GUILayout.Button("Clear World"))
                m_BlockWorld.ClearWorld();
        }

        private void LoadAroundCamera()
        {
            var cam = Camera.main.transform.position;
            var cameraPos = ToChunkCoords(Vector3Int.FloorToInt(cam));
            var extents = new Vector3Int(10, 5, 10);

            m_BlockWorld.LoadChunkRegion(cameraPos, extents);
        }

        private Vector3Int ToChunkCoords(Vector3Int vec)
        {
            int bits = m_BlockWorld.ChunkSize.IntBits;

            return new Vector3Int
            {
                x = vec.x >> bits,
                    y = vec.y >> bits,
                    z = vec.z >> bits,
            };
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
