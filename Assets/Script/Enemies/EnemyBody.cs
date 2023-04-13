using UnityEngine;

namespace TheTD.Enemies
{
    public class EnemyBody : MonoBehaviour
    {
        virtual public Vector3 BodyCenterLocal { get => transform.localPosition; }
        virtual public Vector3 HeadPointLocal { get => new Vector3(transform.localPosition.x, BodyCenterLocal.y * 2f, transform.localPosition.z); }

    }
}