using UnityEngine;

namespace bones3.demo
{
    public class FirstPerson : MonoBehaviour
    {
        public float walkingSpeed = 2;
        public float mouseSpeed = 1f;

        public Transform cam;

        private float yaw;
        private float pitch;

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            UpdatePos();
            UpdateRot();
        }

        private void UpdatePos()
        {
            float ver = Input.GetAxis("Vertical");
            float hor = Input.GetAxis("Horizontal");

            Vector3 forward = transform.TransformDirection(new Vector3(0f, 0f, 1f));
            Vector3 right = transform.TransformDirection(new Vector3(1f, 0f, 0f));

            Vector3 shift = forward * ver + right * hor;

            if (shift.sqrMagnitude > 0)
            {
                shift = shift.normalized * Time.deltaTime * walkingSpeed;
                transform.position += shift;
            }
        }

        private void UpdateRot()
        {
            yaw += Input.GetAxis("Mouse X") * mouseSpeed;
            pitch -= Input.GetAxis("Mouse Y") * mouseSpeed;

            yaw = yaw % 360f;
            pitch = Mathf.Clamp(pitch, -89f, 89f);

            cam.localRotation = Quaternion.Euler(pitch, 0f, 0f);
            transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
        }
    }
}
