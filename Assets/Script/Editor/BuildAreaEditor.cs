using TheTD.Building;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuildArea))]
public class BuildAreaEditor : Editor
{
    const string CREATE_GRID_BUTTON_LABEL = "Create Grid";
    const string UPDATE_GRID_BUTTON_LABEL = "Update Grid";
    const string SHOW_GRID_BUTTON_LABEL = "Hide/Show Grid";
    const string ADD_LISTENERS_BUTTON_LABEL = "Add listeners";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var buildArea = (BuildArea)target;

        if(GUILayout.Button(CREATE_GRID_BUTTON_LABEL))
        {
            buildArea.CustomGrid.Create();
        }
        if(GUILayout.Button(UPDATE_GRID_BUTTON_LABEL))
        {
            buildArea.CustomGrid.Create();
        }
        if(GUILayout.Button(SHOW_GRID_BUTTON_LABEL))
        {
            buildArea.CustomGrid.IsVisible = !buildArea.CustomGrid.IsVisible;
        }
        if (GUILayout.Button(ADD_LISTENERS_BUTTON_LABEL))
        {
            buildArea.AddListeners();
            buildArea.CustomGrid.AddListeners();
        }
    }
}
