using TheTD.Building;
using UnityEngine;

namespace TheTD.UI
{
    public class SelectSpotVisual : MonoBehaviour
    {
        private const string SELECTED_MATERIAL_PATH = "Materials/SelectedMaterial";
        private const string OCCUPIED_MATERIAL_PATH = "Materials/OccupiedMaterial";

        private bool _isSelected = true;
        public bool IsSelected { get => _isSelected; set => SetIsSelected(value); }

        private Material _selectedMaterial;
        public Material SelectedMaterial { get => _selectedMaterial = _selectedMaterial != null ? _selectedMaterial : Resources.Load<Material>(SELECTED_MATERIAL_PATH); }

        private Material _occupiedMaterial;
        public Material OccupiedMaterial { get => _occupiedMaterial = _occupiedMaterial != null ? _occupiedMaterial : Resources.Load<Material>(OCCUPIED_MATERIAL_PATH); }

        private Renderer _renderer;
        public Renderer Renderer { get => _renderer = _renderer != null ? _renderer : GetComponent<Renderer>(); }

        private BuildSpot selectedBuildSpot;

        private void Start()
        {
            BuildArea.OnSelectedBuildSpotChange += OnSelectBuildSpot;
        }

        private void OnSelectBuildSpot(BuildSpot selectedSpot)
        {
            selectedBuildSpot = selectedSpot;
            IsSelected = selectedSpot != null;
        }

        private void SetIsSelected(bool value)
        {          
            gameObject.SetActive(value);
            _isSelected = value;
            if (_isSelected && selectedBuildSpot != null)
            {
                var isOccupied = selectedBuildSpot.HasConstruction || selectedBuildSpot.IsInvalidSpot;
                SetMaterial(isOccupied);
                transform.position = new Vector3(selectedBuildSpot.CenterPositionInWorld.x, selectedBuildSpot.CenterPositionInWorld.y + transform.localPosition.y, selectedBuildSpot.CenterPositionInWorld.z);
            }
        }

        public void SetMaterial(bool isOccupied)
        {
            Renderer.sharedMaterial = !isOccupied ? SelectedMaterial : OccupiedMaterial;
        }
    }
}