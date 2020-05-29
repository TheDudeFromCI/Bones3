using UnityEditor;
using UnityEditor.EditorTools;

using UnityEngine;

namespace WraithavenGames.Bones3.Editor
{
    [EditorTool("World Edit Tool", typeof(BlockWorld))]
    public class WorldEditTool : EditorTool
    {
        protected GUIContent m_IconContent;
        protected IWand wand;
        protected IFillPattern fillPattern;

        public override GUIContent toolbarIcon { get => m_IconContent; }

        protected void OnEnable()
        {
            m_IconContent = new GUIContent()
            {
                image = Resources.Load<Texture2D>("Bones3_WorldEditIcon"),
                text = "World Edit Tool",
                tooltip = "The block world editing tool."
            };

            wand = new FillRegion();
            wand.SetFillPattern(new FloodFill(0));

            BlockPickerWindow.OnSelectedChanged += OnSelectedBlockChanged;
        }

        protected void OnDisable()
        {
            BlockPickerWindow.OnSelectedChanged -= OnSelectedBlockChanged;
        }

        private void OnSelectedBlockChanged()
        {
            var fill = new FloodFill(BlockPickerWindow.SelectedBlockID);
            wand.SetFillPattern(fill);
        }

        public override void OnToolGUI(EditorWindow window)
        {
            if (BlockWorldEditor.BlockWorld == null
                || BlockPickerWindow.SelectedBlockID == 0)
                return;

            if (window as SceneView == null)
                return;

            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            if (Event.current.type == EventType.MouseDown)
                wand.OnMouseEvent(GetTargetBlock(), WandEventType.MouseDown);

            else if (Event.current.type == EventType.MouseUp)
                wand.OnMouseEvent(GetTargetBlock(), WandEventType.MouseUp);

            else if (Event.current.type == EventType.MouseLeaveWindow)
                wand.OnMouseEvent(GetTargetBlock(), WandEventType.ExitWindow);

            else
                wand.OnMouseEvent(GetTargetBlock(), WandEventType.MouseMove);

            wand.Render();
        }

        private TargetBlock GetTargetBlock()
        {
            var blockWorld = BlockWorldEditor.BlockWorld;
            if (blockWorld == null)
                return new TargetBlock { HasShift = Event.current.shift };

            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            var targetBlock = blockWorld.RaycastWorld(ray, float.PositiveInfinity);
            targetBlock.HasShift = Event.current.shift;

            return targetBlock;
        }
    }
}
