using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StateSerializeObject
{
    public StateScriptableObject state;
    public bool isDefaultState = false;
    public string Name => state.name;
}
