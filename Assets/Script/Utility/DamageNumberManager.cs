using TheTD.DamageSystem;
using UnityEngine;

public class DamageNumberManager : MonoBehaviour
{
    public static DamageNumberManager Instance { get; private set; }

    [SerializeField] private GenericObjectPool _damageNumberPool;
    public GenericObjectPool DamageNumberPool { get => _damageNumberPool = _damageNumberPool != null ? _damageNumberPool : FindObjectOfType<GenericObjectPool>(); }

    private FloatingDamageNumber _floatingDamageNumberPrefab;
    public FloatingDamageNumber FloatingDamageNumberPrefab { get => _floatingDamageNumberPrefab = _floatingDamageNumberPrefab != null ? _floatingDamageNumberPrefab : Resources.Load<GameObject>("Prefabs/UI/FloatingDamageNumber").GetComponent<FloatingDamageNumber>(); }

    private void Awake()
    {
        CheckSingleton();
    }

    private void CheckSingleton()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public FloatingDamageNumber SpawnFloatingDamageNumber(Damage damage, Vector3 position, IOvertimeEffect overtimeEffect = null)
    {
        FloatingDamageNumber damageNumber = DamageNumberPool.Spawn(FloatingDamageNumberPrefab);
        damageNumber.InitDamageNumber(damage, position, Quaternion.identity, transform, overtimeEffect);
        return damageNumber;
    }
}