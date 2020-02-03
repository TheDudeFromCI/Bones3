using UnityEngine;

namespace WraithavenGames.Bones3.Player
{
    public class FPBlockPlacer : MonoBehaviour
    {
        [Header("Settings")]
        public Material block;
        public float range = 5f;
        public BlockWorld world;

        [Header("Targeting and Controls")]
        public Camera cam;
        public bool flipBlockPlace = false;
        public float editCooldown = .2f;
        public GameObject blockSelectionPrefab;
        public bool selectionInside;

        private float lastPlace = -100f;
        private GameObject selectionBox;

        private void Awake()
        {
            selectionBox = Instantiate(blockSelectionPrefab) as GameObject;
            selectionBox.SetActive(false);

            if (world == null)
                world = FindObjectOfType<BlockWorld>();
        }

        private void Update()
        {
            if (world == null)
                return;

            if (selectionBox.transform.parent == null)
            {
                selectionBox.transform.SetParent(world.transform);
                selectionBox.transform.localRotation = Quaternion.identity;
                selectionBox.transform.localScale = Vector3.one;
            }

            if (Time.time - lastPlace < editCooldown)
                return;

            SelectedBlock sel = GetSelectedBlock();
            if (!sel.hasSelectedBlock)
            {
                selectionBox.SetActive(false);
                return;
            }
            selectionBox.SetActive(true);

            if (selectionInside)
                selectionBox.transform.localPosition = new Vector3(sel.xOn, sel.yOn, sel.zOn) + Vector3.one * 0.5f;
            else
                selectionBox.transform.localPosition = new Vector3(sel.xInside, sel.yInside, sel.zInside) + Vector3.one * 0.5f;

            if (HasRequestedPlace() && block != null)
            {
                if (CanPlaceBlockAt(sel.xInside, sel.yInside, sel.zInside))
                {
                    lastPlace = Time.time;
                    world.SetBlock(sel.xInside, sel.yInside, sel.zInside, block);
                }
            }
            else if (HasRequestedBreak())
            {
                lastPlace = Time.time;
                world.SetBlock(sel.xOn, sel.yOn, sel.zOn, null);
            }
        }

        private bool CanPlaceBlockAt(int x, int y, int z)
        {
            Transform t = world.transform;
            Vector3 center = t.TransformPoint(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
            Vector3 size = t.TransformVector(Vector3.one * 0.49f);
            Quaternion rotation = t.rotation;

            return Physics.OverlapBox(center, size, rotation, 1, QueryTriggerInteraction.Ignore).Length == 0;
        }

        private SelectedBlock GetSelectedBlock()
        {
            Ray ray = cam.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, range))
            {
                // Created selected block data
                SelectedBlock block = new SelectedBlock();
                block.hasSelectedBlock = true;

                // Shift hit location to avoid floating point errors
                Vector3 inside = hit.point - ray.direction * .0001f;
                Vector3 over = hit.point + ray.direction * .0001f;

                // Translate hit location to blockworld coords
                Vector3 subWorldPos = world.transform.InverseTransformPoint(inside);
                block.xInside = Mathf.FloorToInt(subWorldPos.x);
                block.yInside = Mathf.FloorToInt(subWorldPos.y);
                block.zInside = Mathf.FloorToInt(subWorldPos.z);

                subWorldPos = world.transform.InverseTransformPoint(over);
                block.xOn = Mathf.FloorToInt(subWorldPos.x);
                block.yOn = Mathf.FloorToInt(subWorldPos.y);
                block.zOn = Mathf.FloorToInt(subWorldPos.z);

                return block;
            }

            return new SelectedBlock();
        }

        private bool HasRequestedPlace()
        {
            return Input.GetMouseButtonDown(flipBlockPlace ? 1 : 0);
        }

        private bool HasRequestedBreak()
        {
            return Input.GetMouseButtonDown(flipBlockPlace ? 0 : 1);
        }
    }
}
