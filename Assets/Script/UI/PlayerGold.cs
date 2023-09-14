using TheTD.Players;
using TMPro;
using UnityEngine;

namespace TheTD.UI
{
    public class PlayerGold : MonoBehaviour, IEventListener
    {
        private string goldFormat = "Gold: {0}";
        private TextMeshProUGUI _textMesh;
        public TextMeshProUGUI TextMesh { get => _textMesh = _textMesh != null ? _textMesh : GetComponentInChildren<TextMeshProUGUI>(); }

        private void Start()
        {
            AddListeners();
        }

        public void AddListeners()
        {
            Player.OnSpendGold += OnPlayerGoldChange;
            Player.OnGainGold += OnPlayerGoldChange;
            Player.OnInitialized += OnPlayerGoldChange;
        }

        private void OnPlayerGoldChange(Player player)
        {
            SetGoldText(player.Gold.Current);
        }

        private string FormatGoldText(int value)
        {
            return string.Format(goldFormat, value.ToString());
        }

        private void SetGoldText(int value)
        {
            TextMesh.SetText(FormatGoldText(value));
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        public void RemoveListeners()
        {
            Player.OnSpendGold -= OnPlayerGoldChange;
            Player.OnGainGold -= OnPlayerGoldChange;
            Player.OnInitialized -= OnPlayerGoldChange;
        }
    }
}