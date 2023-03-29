using UnityEngine;

public class EnemyBody : MonoBehaviour
{
    virtual public Vector3 BodyCenter { get => transform.localPosition; }
}
