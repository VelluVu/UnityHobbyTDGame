using UnityEngine;

namespace TheTD.Enemies
{
    public class EnemyBody : MonoBehaviour
    {
        virtual public Vector3 BodyCenter { get => transform.localPosition; }
        virtual public Vector3 HeadPoint { get => new Vector3(transform.localPosition.x, transform.localPosition.y + transform.localPosition.y, transform.localPosition.z); }
    }
}