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
            Player.OnInitialized += OnPlayerInitialized;
            Player.OnTakeDamage += OnPlayerTakeDamage;
        }

        private void OnPlayerInitialized(Player player)
        {
            UpdateTextMesh(player.Life.Current.ToString());
        }

        private void OnPlayerTakeDamage(Player player)
        {
            UpdateTextMesh(player.Life.Current.ToString());
        }

        private void UpdateTextMesh(string text)
        {
            TextMesh.SetText(text);
        }
    }
}