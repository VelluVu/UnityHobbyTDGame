using UnityEngine;

public class GametimeUI : MonoBehaviour
{
    private TextMeshUI[] textMeshes;
    public TextMeshUI[] TextMeshes { get => textMeshes = !textMeshes.IsNullOrEmpty() ? textMeshes : GetComponentsInChildren<TextMeshUI>(); }

    private void TrackSeconds()
    {
        TextMeshes[2].SetText(StringHelper.ConvertNumberToTwoDigitString(Gametime.Seconds));
    }

    private void TrackMinutes()
    {
        string text = StringHelper.ConvertNumberToTwoDigitString(Gametime.Minutes);
        text = ": " + text + " :";
        TextMeshes[1].SetText(text);
    }

    private void TrackHours()
    {
        TextMeshes[0].SetText(StringHelper.ConvertNumberToTwoDigitString(Gametime.Hours));
    }

    private void Update()
    {
        TrackSeconds();
        TrackMinutes();
        TrackHours();
    }
}
