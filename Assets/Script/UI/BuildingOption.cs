using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TheTD.UI
{
    public class BuildingOption : MonoBehaviour, IEventListener
    {
        private Color originalColor;

        private bool _isSelected = false;
        public bool IsSelected { get => _isSelected; set => SetIsSelected(value); }

        private Image _background = null;
        public Image Background { get => _background = _background != null ? _background : GetComponent<Image>(); }

        private Button _button = null;
        public Button Button { get => _button = _button != null ? _button : GetComponent<Button>(); }

        private TextMeshProUGUI _buttonTextMesh = null;
        public TextMeshProUGUI ButtonTextMesh { get => _buttonTextMesh = _buttonTextMesh != null ? _buttonTextMesh : Button.GetComponentInChildren<TextMeshProUGUI>(); }

        private ITowerLoadData _towerLoadData = null;
        public ITowerLoadData TowerLoadData { get => _towerLoadData; private set => _towerLoadData = value; }

        public delegate void BuildingOptionDelegate(BuildingOption buildingOption);
        public static event BuildingOptionDelegate OnSelectTowerOption;

        private void Start()
        {
            originalColor = Background.color;
            AddListeners();
        }

        public void AddListeners()
        {
            Button.onClick.AddListener(OnClickBuildingOption);
        }

        public void RemoveListeners()
        {
            Button.onClick.RemoveAllListeners();
        }

        public void InitBuildingOption(ITowerLoadData tower)
        {
            TowerLoadData = tower;
            ButtonTextMesh.text = TowerLoadData.Name;
        }

        public void OnClickBuildingOption()
        {
            OnSelectTowerOption?.Invoke(this);
            IsSelected = true;
        }

        private void SetIsSelected(bool value)
        {
            if (_isSelected == value) return;
            _isSelected = value;

            if (_isSelected)
            {
                Background.color = Color.green;
            }
            else
            {
                Background.color = originalColor;
            }
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }
    }
}