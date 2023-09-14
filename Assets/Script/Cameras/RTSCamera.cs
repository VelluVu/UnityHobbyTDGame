using UnityEngine;

namespace TheTD.Cameras
{
    public class RTSCamera : MonoBehaviour
    {
        const string Horizontal = "Horizontal";
        const string Vertical = "Vertical";
        public bool isMouseMove = false;
        public float speed = 10f;
        public float zoomSpeed = 10f;
        public float borderThickness = 10f;
        public float zoomLimit = 10f;
        public float zoomRatio = 1f;
        public float inputSensitivity = 0.5f;
        public Vector2 moveAreaSize = new Vector2(50f, 50f);

        public delegate void OnCameraZoomDelegate(float zoomValue);
        public static event OnCameraZoomDelegate OnCameraZoomChange;

        private void Start()
        {
            OnCameraZoomChange?.Invoke(zoomRatio);
        }

        private void FixedUpdate()
        {
            MoveCamera();
        }

        private void MoveCamera()
        {
            var position = transform.position;
            Vector3 direction = GetDirectionByInput();
            position += direction * speed * Time.deltaTime;
            position.x = Mathf.Clamp(position.x, -moveAreaSize.x, moveAreaSize.x);
            position.z = Mathf.Clamp(position.z, -moveAreaSize.y, moveAreaSize.y);
            transform.position = position;
        }

        private Vector3 GetDirectionByMousePosition()
        {
            var mousePosition = Input.mousePosition;
            var direction = Vector3.zero;
            if (mousePosition.x > (Screen.width - borderThickness)) direction = Vector3.right;      
            if (mousePosition.x < borderThickness) direction = -Vector3.right;        
            if (mousePosition.y > (Screen.height - borderThickness)) direction = Vector3.forward;                
            if (mousePosition.y < borderThickness) direction = -Vector3.forward;  
            if (mousePosition.x < borderThickness && mousePosition.y < borderThickness) direction = new Vector3(-1f, 0f, -1f);       
            if (mousePosition.x > (Screen.width - borderThickness) && mousePosition.y > (Screen.height - borderThickness)) direction = new Vector3(1f, 0f, 1f);      
            if (mousePosition.x < borderThickness && mousePosition.y > (Screen.height - borderThickness)) direction = new Vector3(-1f, 0f, 1f);      
            if (mousePosition.x > (Screen.width - borderThickness) && mousePosition.y < borderThickness) direction = new Vector3(1f, 0f, -1f);     
            return direction;
        }

        private Vector3 GetDirectionByInput()
        {
            var horizontal = Input.GetAxis(Horizontal);
            var vertical = Input.GetAxis(Vertical);
            var direction = Vector3.zero;

            if (isMouseMove) direction = GetDirectionByMousePosition();  
    
            if (horizontal > inputSensitivity) direction = Vector3.right; 
            if (horizontal < -inputSensitivity) direction = -Vector3.right;    
            if (vertical > inputSensitivity) direction = Vector3.forward; 
            if (vertical < -inputSensitivity) direction = -Vector3.forward;
            if (horizontal < -inputSensitivity && vertical < -inputSensitivity) direction = new Vector3(-1f, 0f, -1f);
            if (horizontal > inputSensitivity && vertical > inputSensitivity) direction = new Vector3(1f, 0f, 1f);
            if (horizontal < -inputSensitivity && vertical > inputSensitivity) direction = new Vector3(-1f, 0f, 1f);
            if (horizontal > inputSensitivity && vertical < -inputSensitivity) direction = new Vector3(1f, 0f, -1f);
            
            return direction;
        }
    }
}