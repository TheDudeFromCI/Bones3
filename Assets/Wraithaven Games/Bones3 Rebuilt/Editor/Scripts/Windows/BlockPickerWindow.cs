using UnityEditor;

using UnityEngine;

namespace WraithavenGames.Bones3.Editor
{
    /// <summary>
    /// A utility window for displaying a list of blocks based on the currently
    /// selected world.
    /// </summary>
    public class BlockPickerWindow : EditorWindow
    {
        [MenuItem("Window/Bones3/Block List")]
        protected static void Init()
        {
            var window = (BlockPickerWindow)GetWindow(typeof(BlockPickerWindow), false, "Block List", true);
            window.Show();
        }

        private static ushort m_SelectedBlockID;

        /// <summary>
        /// Gets the selected block ID, or 0 if no block is selected.
        /// </summary>
        public static ushort SelectedBlockID
        {
            get => m_SelectedBlockID;
            private set
            {
                m_SelectedBlockID = value;
                OnSelectedChanged?.Invoke();
            }
        }

        /// <summary>
        /// A callback for when the selected block ID changes.
        /// </summary>
        public static event System.Action OnSelectedChanged;

        private BlockWorld BlockWorld => BlockWorldEditor.BlockWorld;
        private BlockListManager BlockList => BlockWorldEditor.BlockList;

        protected void OnEnable()
        {
            BlockWorldEditor.OnBlockWorldChanged += UpdateBlockWorld;
        }

        protected void OnDisable()
        {
            BlockWorldEditor.OnBlockWorldChanged -= UpdateBlockWorld;
        }

        private void UpdateBlockWorld()
        {
            SelectedBlockID = 0;
            Repaint();
        }

        protected void OnGUI()
        {
            if (BlockWorld == null)
                return;

            if (BlockList.BlockCount == 0)
            {
                EditorGUILayout.HelpBox("No blocks defined!", MessageType.Warning);
                return;
            }

            for (ushort i = 1; i < BlockList.BlockCount; i++)
                DrawBlock(i);
        }

        private void DrawBlock(ushort id)
        {
            var blockType = BlockList.GetBlockType(id);

            bool selected = SelectedBlockID == id;
            bool newSelected = GUILayout.Toggle(selected, $"{id}) {blockType.Name}");

            if (newSelected && !selected)
                SelectedBlockID = id;
        }
    }
}
