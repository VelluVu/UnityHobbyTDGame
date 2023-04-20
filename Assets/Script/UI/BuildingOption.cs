using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TheTD.UI
{
    public class BuildingOption : MonoBehaviour
    {
        private Color originalColor;

        private bool isSelected;
        public bool IsSelected { get => isSelected; set => SetIsSelected(value); }

        private Image background;
        public Image BackGround { get => background = background != null ? background : GetComponent<Image>(); }

        private Button button;
        public Button Button { get => button = button != null ? button : GetComponent<Button>(); }

        private TextMeshProUGUI buttonTextMesh;
        public TextMeshProUGUI ButtonTextMesh { get => buttonTextMesh = buttonTextMesh != null ? buttonTextMesh : Button.GetComponentInChildren<TextMeshProUGUI>(); }

        private ITowerLoadData towerLoadData;
        public ITowerLoadData TowerLoadData { get => towerLoadData; private set => towerLoadData = value; }

        public delegate void BuildingOptionDelegate(BuildingOption buildingOption);
        public static event BuildingOptionDelegate OnSelectTowerOption;

        private void Start()
        {
            Button.onClick.AddListener(OnClickBuildingOption);
            originalColor = BackGround.color;
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
            if (isSelected == value) return;
            isSelected = value;

            if (isSelected)
            {
                BackGround.color = Color.green;
            }
            else
            {
                BackGround.color = originalColor;
            }
        }
    }
}