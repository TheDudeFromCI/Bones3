using UnityEditor;

using UnityEngine;

namespace WraithavenGames.Bones3.Editor
{
    /// <summary>
    /// The main editor for Block Worlds.
    /// </summary>
    [CustomEditor(typeof(BlockWorld))]
    public class BlockWorldEditor : UnityEditor.Editor
    {
        private static BlockWorld m_BlockWorld;

        /// <summary>
        /// Gets the currently selected block world, or null if none are selected.
        /// </summary>
        public static BlockWorld BlockWorld
        {
            get => m_BlockWorld;
            set
            {
                m_BlockWorld = value;
                BlockList = BlockWorld?.GetComponent<BlockListManager>();
                OnBlockWorldChanged?.Invoke();
            }
        }

        /// <summary>
        /// Gets the block list being used by the selected block world, or null
        /// if no world is selected.
        /// </summary>
        public static BlockListManager BlockList { get; private set; }

        /// <summary>
        /// A callback for when the selected block world changes.
        /// </summary>
        public static event System.Action OnBlockWorldChanged;

        protected void OnEnable()
        {
            BlockWorld = target as BlockWorld;
        }

        protected void OnDisable()
        {
            BlockWorld = null;
        }

        protected void OnSceneGUI()
        {
            if (BlockWorld == null)
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
