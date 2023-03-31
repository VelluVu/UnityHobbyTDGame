using UnityEngine;

namespace TheTD.Cameras
{
    public class RTSCamera : MonoBehaviour
    {
        public bool isMouseMove = false;
        public float speed = 10f;
        public float zoomSpeed = 10f;
        public float borderThickness = 10f;
        public float zoomLimit = 10f;
        public float inputSensitivity = 0.5f;
        public Vector2 moveAreaSize = new Vector2(50f, 50f);

        private void Update()
        {
            var position = transform.position;
            Vector3 direction = GetDirectionByInput();
            position += direction * speed * Time.deltaTime;
            position.x = Mathf.Clamp(position.x, -moveAreaSize.x, moveAreaSize.x);
            position.z = Mathf.Clamp(position.z, -moveAreaSize.y, moveAreaSize.y);
            transform.position = position;
        }

        private Vector3 GetDirectionByInput()
        {
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");
            var mousePosition = Input.mousePosition;

            if (isMouseMove)
            {
                if (mousePosition.x < borderThickness && mousePosition.y < borderThickness)
                {
                    return new Vector3(-1f, 0f, -1f);
                }
                else if (mousePosition.x > (Screen.width - borderThickness) && mousePosition.y > (Screen.height - borderThickness))
                {
                    return new Vector3(1f, 0f, 1f);
                }
                else if (mousePosition.x < borderThickness && mousePosition.y > (Screen.height - borderThickness))
                {
                    return new Vector3(-1f, 0f, 1f);
                }
                else if (mousePosition.x > (Screen.width - borderThickness) && mousePosition.y < borderThickness)
                {
                    return new Vector3(1f, 0f, -1f);
                }
                else if (mousePosition.x > (Screen.width - borderThickness))
                {
                    return Vector3.right;
                }
                else if (mousePosition.x < borderThickness)
                {
                    return -Vector3.right;
                }
                else if (mousePosition.y > (Screen.height - borderThickness))
                {
                    return Vector3.forward;
                }
                else if (mousePosition.y < borderThickness)
                {
                    return -Vector3.forward;
                }
            }

            if (horizontal < -inputSensitivity && vertical < -inputSensitivity)
            {
                return new Vector3(-1f, 0f, -1f);
            }
            else if (horizontal > inputSensitivity && vertical > inputSensitivity)
            {
                return new Vector3(1f, 0f, 1f);
            }
            else if (horizontal < -inputSensitivity && vertical > inputSensitivity)
            {
                return new Vector3(-1f, 0f, 1f);
            }
            else if (horizontal > inputSensitivity && vertical < -inputSensitivity)
            {
                return new Vector3(1f, 0f, -1f);
            }
            else if (horizontal > inputSensitivity)
            {
                return Vector3.right;
            }
            else if (horizontal < -inputSensitivity)
            {
                return -Vector3.right;
            }
            else if (vertical > inputSensitivity)
            {
                return Vector3.forward;
            }
            else if (vertical < -inputSensitivity)
            {
                return -Vector3.forward;
            }

            return Vector3.zero;
        }
    }
}