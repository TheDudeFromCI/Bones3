using UnityEngine;

namespace WraithavenGames.Bones3.Demo
{
    public class FlyingCam : MonoBehaviour
    {
        public float speed = 8.0f;
        public float mouseSensitivity = 1.0f;

        private float yaw;
        private float pitch;
        private bool locked = false;

        void Update()
        {
            Look();
            Fly();
        }

        private void Look()
        {
            if (Input.GetMouseButtonDown(0))
            {
                locked = !locked;

                if (locked)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }

            if (!locked)
                return;

            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch += -Input.GetAxis("Mouse Y") * mouseSensitivity;

            yaw %= 360f;
            pitch = Mathf.Clamp(pitch, -89f, 89f);

            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }

        private void Fly()
        {
            Vector3 dir = Vector3.zero;

            dir += Input.GetAxis("Vertical") * transform.forward;
            dir += Input.GetAxis("Horizontal") * transform.right;

            dir *= Time.deltaTime * speed;
            transform.position += dir;
        }
    }
}
