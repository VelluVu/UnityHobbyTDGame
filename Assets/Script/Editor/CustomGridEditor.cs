using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CustomGrid))]
public class CustomGridEditor : Editor
{
    const string CREATE_GRID_BUTTON_LABEL = "Create Grid";
    const string UPDATE_GRID_BUTTON_LABEL = "Update Grid";
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var grid = (CustomGrid)target;

        if (GUILayout.Button(CREATE_GRID_BUTTON_LABEL))
        {
            grid.Create();
        }
        if (GUILayout.Button(UPDATE_GRID_BUTTON_LABEL))
        {
            grid.Create();
        }
    }
}
