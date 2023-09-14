using TheTD.Cameras;
using TMPro;
using UnityEngine;

namespace TheTD.UI
{
    public class FontSize : MonoBehaviour, IEventListener
    {
        private float originalFontSize = 0;
 
        private TextMeshProUGUI _textMesh;
        public TextMeshProUGUI TextMesh { get => _textMesh = _textMesh != null ? _textMesh : GetComponentInChildren<TextMeshProUGUI>(); }

        private void Start()
        {
            originalFontSize = TextMesh.fontSize;
            AddListeners();
        }

        public void AddListeners()
        {
            RTSCamera.OnCameraZoomChange += OnCameraZoomChange;
        }

        public void RemoveListeners()
        {        
            RTSCamera.OnCameraZoomChange -= OnCameraZoomChange;
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        private void ResetFontSize()
        {
            TextMesh.fontSize = originalFontSize;
        }

        private void OnCameraZoomChange(float zoomValue)
        {
            ChangeFontSize(zoomValue);
        }

        private void ChangeFontSize(float multiplier)
        {
            TextMesh.fontSize = originalFontSize * multiplier;
        }
    }
}