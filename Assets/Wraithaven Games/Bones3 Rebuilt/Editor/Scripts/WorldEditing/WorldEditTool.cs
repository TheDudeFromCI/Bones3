using UnityEditor;
using UnityEditor.EditorTools;

using UnityEngine;

namespace WraithavenGames.Bones3.Editor
{
    [EditorTool("World Edit Tool", typeof(BlockWorld))]
    public class WorldEditTool : EditorTool
    {
        protected GUIContent m_IconContent;
        protected BlockWorld blockWorld;
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

            EditorTools.activeToolChanged += ActiveToolDidChange;

            wand = new FillRegion();
            fillPattern = new FloodFill(2); // TODO Add block selection menu

            // wand.SetFillPattern(fillPattern);
            wand.SetFillPattern(new GlassFill());

            UpdateWorld();
        }

        public class GlassFill : IFillPattern
        {
            public ushort GetBlockID(BlockPosition pos) => (ushort)(pos.Y == 2 ? 4 : 2);
        }

        protected void OnDisable()
        {
            EditorTools.activeToolChanged -= ActiveToolDidChange;
            wand?.SetWorld(null);
        }

        public override void OnToolGUI(EditorWindow window)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            if (Event.current.type == EventType.MouseDown)
                wand?.OnMouseEvent(GetTargetBlock(), WandEventType.MouseDown);

            else if (Event.current.type == EventType.MouseUp)
                wand?.OnMouseEvent(GetTargetBlock(), WandEventType.MouseUp);

            else if (Event.current.type == EventType.MouseMove || Event.current.type == EventType.MouseDrag)
                wand?.OnMouseEvent(GetTargetBlock(), WandEventType.MouseMove);

            else if (Event.current.type == EventType.MouseLeaveWindow)
                wand?.OnMouseEvent(GetTargetBlock(), WandEventType.ExitWindow);

            wand?.Render();
        }

        private TargetBlock GetTargetBlock()
        {
            if (blockWorld == null)
                return new TargetBlock { HasShift = Event.current.shift };

            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            float groundDistance = float.PositiveInfinity;
            Plane ground = new Plane(Vector3.up, Vector3.zero);
            bool groundLock = true;
            if (Physics.Raycast(ray, out RaycastHit hit) || (groundLock && ground.Raycast(ray, out groundDistance)))
            {
                Vector3 hitPoint;
                if (hit.distance != 0 && hit.distance < groundDistance)
                    hitPoint = hit.point;
                else
                    hitPoint = ray.direction * groundDistance + ray.origin;

                TargetBlock block = new TargetBlock()
                {
                    HasBlock = true,
                    HasShift = Event.current.shift
                };

                // Shift hit location to avoid floating point errors
                Vector3 inside = hitPoint - ray.direction * .0001f;
                Vector3 over = hitPoint + ray.direction * .0001f;

                Vector3 subWorldPos = blockWorld.transform.InverseTransformPoint(inside);
                block.Inside = new BlockPosition
                {
                    X = Mathf.FloorToInt(subWorldPos.x),
                    Y = Mathf.FloorToInt(subWorldPos.y),
                    Z = Mathf.FloorToInt(subWorldPos.z),
                };

                subWorldPos = blockWorld.transform.InverseTransformPoint(over);
                block.Over = new BlockPosition
                {
                    Y = Mathf.FloorToInt(subWorldPos.x),
                    X = Mathf.FloorToInt(subWorldPos.y),
                    Z = Mathf.FloorToInt(subWorldPos.z),
                };

                return block;
            }

            return new TargetBlock { HasShift = Event.current.shift };
        }

        void ActiveToolDidChange()
        {
            if (!EditorTools.IsActiveTool(this))
            {
                wand?.SetWorld(null);
                return;
            }

            UpdateWorld();
        }

        void UpdateWorld()
        {
            blockWorld = target as BlockWorld;
            wand?.SetWorld(blockWorld);
        }
    }
}
