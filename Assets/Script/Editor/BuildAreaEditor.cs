using TheTD.Building;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuildArea))]
public class BuildAreaEditor : Editor
{
    const string SHOW_GRID_BUTTON_LABEL = "Hide/Show Grid";
    const string ADD_LISTENERS_BUTTON_LABEL = "Add listeners";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var buildArea = (BuildArea)target;

        if(GUILayout.Button(SHOW_GRID_BUTTON_LABEL))
        {
            buildArea.customGrid.IsVisible = !buildArea.customGrid.IsVisible;
        }
        if (GUILayout.Button(ADD_LISTENERS_BUTTON_LABEL))
        {
            buildArea.AddListeners();
            buildArea.customGrid.AddListeners();
        }
    }
}
