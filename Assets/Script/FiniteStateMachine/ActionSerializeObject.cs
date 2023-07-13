using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActionSeriliazeObject
{
    public ActionScriptableObject action;
    public int order = 0;
    public string Name => action.name;
}
