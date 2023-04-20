using UnityEngine;

namespace TheTD.Enemies
{
    public class EnemyBody : MonoBehaviour
    {
        virtual public Vector3 CenterLocal { get => transform.localPosition; }
        virtual public Vector3 HeadPointLocal { get => new Vector3(transform.localPosition.x, CenterLocal.y * 2f, transform.localPosition.z); }

    }
}