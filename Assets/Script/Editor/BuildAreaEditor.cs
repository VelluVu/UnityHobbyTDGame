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
        var grid = (BuildArea)target;
        if(GUILayout.Button(CREATE_GRID_BUTTON_LABEL))
        {
            grid.CreateGrid();
        }
        if(GUILayout.Button(UPDATE_GRID_BUTTON_LABEL))
        {
            grid.UpdateGrid();
        }
        if(GUILayout.Button(SHOW_GRID_BUTTON_LABEL))
        {
            grid.IsGridVisible = !grid.IsGridVisible;
        }
        if (GUILayout.Button(ADD_LISTENERS_BUTTON_LABEL))
        {
            grid.AddListeners();
        }
    }
}
