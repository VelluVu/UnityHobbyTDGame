using TheTD.Building;
using UnityEngine;

namespace TheTD.UI
{
    public class SelectSpotVisual : MonoBehaviour, IEventListener
    {
        private const string SELECTED_MATERIAL_PATH = "Materials/SelectedMaterial";
        private const string OCCUPIED_MATERIAL_PATH = "Materials/OccupiedMaterial";

        private Material _selectedMaterial;
        public Material SelectedMaterial { get => _selectedMaterial = _selectedMaterial != null ? _selectedMaterial : Resources.Load<Material>(SELECTED_MATERIAL_PATH); }

        private Material _occupiedMaterial;
        public Material OccupiedMaterial { get => _occupiedMaterial = _occupiedMaterial != null ? _occupiedMaterial : Resources.Load<Material>(OCCUPIED_MATERIAL_PATH); }

        private Renderer _renderer;
        public Renderer Renderer { get => _renderer = _renderer != null ? _renderer : GetComponent<Renderer>(); }

        private BuildSpot selectedBuildSpot;

        private void Start()
        {
            AddListeners();            
        }

        public void AddListeners()
        {
            BuildArea.OnSelectedBuildSpotChange += OnSelectBuildSpot;
        }

        private void OnSelectBuildSpot(BuildSpot selectedSpot)
        {
            selectedBuildSpot = selectedSpot;
            bool isSelected = selectedSpot != null;
            gameObject.SetActive(isSelected);
            
            if(!isSelected) {
                SetMaterial();
                return;
            }

            SetMaterial(selectedBuildSpot.IsInvalidConstructionSpot);
            transform.position = new Vector3(selectedBuildSpot.CenterPositionInWorld.x, selectedBuildSpot.CenterPositionInWorld.y + 0.1f, selectedBuildSpot.CenterPositionInWorld.z);
        }

        public void SetMaterial(bool isInvalid = false)
        {           
            Renderer.sharedMaterial = !isInvalid ? SelectedMaterial : OccupiedMaterial;
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        public void RemoveListeners()
        {
            BuildArea.OnSelectedBuildSpotChange -= OnSelectBuildSpot;
        }
    }
}