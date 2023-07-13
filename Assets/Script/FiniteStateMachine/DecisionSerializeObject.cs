using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DecisionSeriliazeObject
{
    public DecisionScriptableObject condition;
    public string Name => condition.name;

}
