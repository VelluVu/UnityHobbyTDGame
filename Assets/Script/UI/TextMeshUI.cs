using TMPro;
using UnityEngine;

public class TextMeshUI : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    public TextMeshProUGUI TextMesh { get => textMesh = textMesh != null ? textMesh : GetComponent<TextMeshProUGUI>(); }

    public void SetText(string text)
    {
        TextMesh.text = text;
    }
}
