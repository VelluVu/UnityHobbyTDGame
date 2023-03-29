using TheTD.Core;
using TMPro;
using UnityEngine;

namespace TheTD.UI
{
    public class PlayerGold : MonoBehaviour
    {
        private string goldFormat = "Gold: {0}";
        private TextMeshProUGUI _textMesh;
        public TextMeshProUGUI TextMesh { get => _textMesh = _textMesh != null ? _textMesh : GetComponentInChildren<TextMeshProUGUI>(); }

        private void Start()
        {
            GameControl.Instance.OnPlayerSpendGold += OnPlayerSpendGold;
            GameControl.Instance.OnPlayerGainGold += OnPlayerGainGold;
            SetGoldText(GameControl.Instance.PlayerGold);
        }
        
        private void OnPlayerGainGold(int goldDifference, int PlayerGoldAfterOperation)
        {
            SetGoldText(PlayerGoldAfterOperation);
        }

        private void OnPlayerSpendGold(int goldDifference, int PlayerGoldAfterOperation)
        {
            SetGoldText(PlayerGoldAfterOperation);
        }

        private string FormatGoldText(int value)
        {
            return string.Format(goldFormat, value.ToString());
        }

        private void SetGoldText(int value)
        {
            TextMesh.SetText(FormatGoldText(value));
        }
    }
}