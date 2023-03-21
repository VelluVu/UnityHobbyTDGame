using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Spawner))]
public class SpawnerEditor : Editor
{
    const string SPAWN_BUTTON_LABEL = "Spawn";
    const string SPAWN_SINGLE_BUTTON_LABEL = "Test Spawn Single Enemy";
    const string SPAWN_MULTIPLE_BUTTON_LABEL = "Test Spawn Multiple Enemy";

    public override void OnInspectorGUI()
    {
        var spawner = (Spawner)target;

        if (GUILayout.Button(SPAWN_BUTTON_LABEL))
        {
            spawner.SpawnSet();
        }

        GUILayout.Space(10f);

        if (GUILayout.Button(SPAWN_SINGLE_BUTTON_LABEL))
        {
            spawner.SpawnSingleEnemy(spawner.testSingleEnemySpawnEnemyType);
        }

        GUILayout.Space(10f);
     
        if(GUILayout.Button(SPAWN_MULTIPLE_BUTTON_LABEL))
        {
            spawner.SpawnAmountOfEnemies(spawner.testMultipleSpawnSpawn);
        }

        base.OnInspectorGUI();
    }
}
