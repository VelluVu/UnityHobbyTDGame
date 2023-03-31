using UnityEngine;

/// <summary>
/// TODO: Dropped rewards under this object
/// </summary>
public class DroppedItems : MonoBehaviour
{
    public static DroppedItems Instance { get; private set; }

    private void Awake()
    {
        CheckSingleton();
    }

    private void CheckSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
}
