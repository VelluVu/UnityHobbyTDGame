using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TransitionSeriliazeObject
{
    public TransitionScriptableObject transition;
    public string Name => transition.name;
}
