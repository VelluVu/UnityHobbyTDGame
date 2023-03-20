using UnityEngine;

public class SelectSpotVisual : MonoBehaviour
{
    private bool isSelected = true;
    public bool IsSelected { get => isSelected; set => SetIsSelected(value); }

    public Vector3 Position { set => SetPosition(value); }

    private Material selectedMaterial;
    public Material SelectedMaterial { get => selectedMaterial = selectedMaterial != null ? selectedMaterial : Resources.Load<Material>("Materials/SelectedMaterial"); }

    private Material occupiedMaterial;
    public Material OccupiedMaterial { get => occupiedMaterial = occupiedMaterial != null ? occupiedMaterial : Resources.Load<Material>("Materials/OccupiedMaterial"); }

    private Renderer _renderer;
    public Renderer Renderer { get => _renderer = _renderer != null ? _renderer : GetComponent<Renderer>(); }  

    private void SetIsSelected(bool value)
    {
        if(isSelected == value) return;
        gameObject.SetActive(value);
        isSelected = value;
    }

    public void SetMaterial(bool isOccupied)
    {
        Renderer.sharedMaterial = !isOccupied ? SelectedMaterial : OccupiedMaterial;
    }

    private void SetPosition(Vector3 position)
    {
        transform.position = position;
    }
}
