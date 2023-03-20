using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Progress))]
public class ProgressEditor : Editor
{
    private const string RESET_TOWERS = "Reset Towers";
    private const string UNLOCK_ALL_TOWERS = "Unlock All Towers";

    public override void OnInspectorGUI()
    {
        var progress = (Progress)target;
        if(GUILayout.Button(RESET_TOWERS))
        {
            progress.ResetTowersList();
        }
        if(GUILayout.Button(UNLOCK_ALL_TOWERS))
        {
            progress.UnlockAllTowers();
        }
        base.OnInspectorGUI();
    }
}
