using UnityEngine;

using WraithavenGames.Bones3;

namespace WraithavenGames.Bones3Demo
{
    [AddComponentMenu("Bones3/Demo/First Person Place Block")]
    public class FPPlaceBlock : MonoBehaviour
    {
        private Camera m_Camera;
        private BlockWorld m_BlockWorld;

        private Ray CameraRay => m_Camera.ScreenPointToRay(new Vector3(Screen.width, Screen.height, 0) / 2);
        private TargetBlock RaycastWorld => m_BlockWorld.RaycastWorld(CameraRay, 5f);

        protected void Awake()
        {
            m_Camera = Camera.main;
            m_BlockWorld = FindObjectOfType<BlockWorld>();
        }

        protected void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var target = RaycastWorld;
                if (target.HasBlock)
                    m_BlockWorld.SetBlock(target.Over, 1);
            }

            if (Input.GetMouseButtonDown(1))
            {
                var target = RaycastWorld;
                if (target.HasBlock)
                    m_BlockWorld.SetBlock(target.Inside, 2);
            }
        }
    }
}
