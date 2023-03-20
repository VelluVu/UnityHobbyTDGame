using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpawnersControl))]
public class SpawnersControlEditor : Editor
{
    const string SPAWN_WITH_SELECTED_SPAWNERS = "Spawn with selected";

    public override void OnInspectorGUI()
    {
        var spawnsControl = (SpawnersControl)target;

        if(GUILayout.Button(SPAWN_WITH_SELECTED_SPAWNERS))
        {
            spawnsControl.SpawnWithSelectedSpawners(GameControl.Instance.nextWave);
        }

        base.OnInspectorGUI();
    }
}
