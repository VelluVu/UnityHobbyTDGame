using UnityEngine;

[CreateAssetMenu(menuName = "DamageTypes/DamageType")]
public class DamageType : ScriptableObject
{
    [SerializeField]private Color _color = Color.yellow;
    public Color Color { get => _color; }
}
