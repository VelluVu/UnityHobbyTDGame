using TheTD.Core;
using TMPro;
using UnityEngine;

namespace TheTD.UI
{
    public class PlayerLife : MonoBehaviour
    {
        private TextMeshProUGUI _textMesh;
        public TextMeshProUGUI TextMesh { get => _textMesh = _textMesh != null ? _textMesh : GetComponentInChildren<TextMeshProUGUI>(); }

        private void Start()
        {
            UpdateTextMesh(GameControl.Instance.PlayerLife.ToString());
            GameControl.Instance.OnPlayerTakeDamage += OnPlayerTakeDamage;
        }

        private void OnPlayerTakeDamage()
        {
            UpdateTextMesh(GameControl.Instance.PlayerLife.ToString());
        }

        private void UpdateTextMesh(string text)
        {
            TextMesh.SetText(text);
        }
    }
}