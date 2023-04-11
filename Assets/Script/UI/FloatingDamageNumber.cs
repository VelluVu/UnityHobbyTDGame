using TMPro;
using UnityEngine;

public class FloatingDamageNumber : DamageNumber
{
    [SerializeField] private TextMeshProUGUI _textMeshPro;
    public TextMeshProUGUI TextMeshPro { get => _textMeshPro = _textMeshPro != null ? _textMeshPro : GetComponentInChildren<TextMeshProUGUI>(); }

    public MoveWithAnimationCurve MoveWithAnimationCurve;

    public override void SetDamage(int amount, Color color)
    {
        TextMeshPro.text = amount.ToString();
        TextMeshPro.color = color;
        MoveWithAnimationCurve.StartMove(transform.up, this);
    }

    protected override void SetAlpha(float alpha)
    {
        TextMeshPro.alpha = alpha;
    }
}