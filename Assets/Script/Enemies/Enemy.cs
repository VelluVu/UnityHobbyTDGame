using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private const string END_POINT_TAG = "EndPoint";

    public float originalAgentRadius = 0.3f;
    public float shrinkedAgentRadius = 0.1f;

    protected EnemyType type = EnemyType.Goblin;

    protected bool isReachedEnd = false;
    public bool IsReachedEnd { get => isReachedEnd; private set => SetReachedEnd(value); }

    private bool isDead;
    virtual public bool IsDead { get => isDead; set => SetIsDead(value); }

    [SerializeField] protected float currentHealth = 20f;
    virtual public float CurrentHealth { get => currentHealth; private set => currentHealth = value; }

    [SerializeField] protected float maxHealth = 20f;
    virtual public float MaxHealth { get => maxHealth; private set => maxHealth = value; }

    [SerializeField]protected int damage = 1;
    virtual public int Damage { get => damage; private set => damage = value; }

    [SerializeField] protected int _goldValue = 1;
    virtual public int GoldValue { get => _goldValue; private set => _goldValue = value; }

    protected NavMeshAgent agent;
    virtual public NavMeshAgent Agent { get => agent = agent != null ? agent : GetComponentInChildren<NavMeshAgent>(); }

    protected Transform target;
    virtual public Transform Target { get => target = target != null ? target : GameObject.FindGameObjectWithTag(END_POINT_TAG).transform; }

    protected Renderer _renderer;
    virtual public Renderer Renderer { get => _renderer = _renderer != null ? _renderer : GetComponentInChildren<Renderer>(); }

    private Collider _collider;
    virtual internal Collider Collider { get => _collider = _collider != null ? _collider : GetComponent<Collider>(); }

    private Rigidbody _rigidBody;
    public Rigidbody Rigidbody { get => _rigidBody = _rigidBody != null ? _rigidBody : GetComponent<Rigidbody>(); }

    protected EnemyBody enemyBody;
    virtual public EnemyBody EnemyBody { get => enemyBody = enemyBody != null ? enemyBody : GetComponentInChildren<EnemyBody>(); }

    public delegate void EnemyDelegate(Enemy enemy);
    public static event EnemyDelegate OnReachEnd;
    public static event EnemyDelegate OnDeath;
    public static event EnemyDelegate OnPathBlocked;

    virtual public void StartMoving()
    {
        ResetEnemy();     
        StartCoroutine(CheckNavMeshState());
    }

    virtual public void ResetEnemy()
    {
        IsReachedEnd = false;
        IsDead = false;
        CurrentHealth = MaxHealth;
        Agent.destination = Target.transform.position + Vector3.up * transform.position.y;
    }

    virtual public void TakeDamage(Damage damage)
    {
        CurrentHealth -= damage.value;
        CheckDeath(damage);
    }

    virtual protected void Start()
    {
        AddListeners();
        StartMoving();
    }

    virtual protected void AddListeners()
    {

    }

    virtual protected void OnTriggerEnter(Collider other)
    {
        OnBodyCollision(other);
    }

    virtual protected void OnBodyCollision(Collider other)
    {
        if (other.CompareTag(END_POINT_TAG))
        {
            IsReachedEnd = true;
            IsDead = true;
        }
    }

    virtual protected void CheckDeath(Damage damage)
    {
        if (CurrentHealth > 0f) return;
        IsDead = true;    
    }

    virtual protected IEnumerator CheckNavMeshState()
    {
        while (true)
        {
            if (Agent.pathStatus == NavMeshPathStatus.PathPartial)
            {
                OnPathBlocked?.Invoke(this);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    virtual protected IEnumerator DestroySmoothly()
    {
        float dissolveTime = 0f;
        yield return new WaitForSeconds(2f); //give physics body time to take impact before dissolve
        ClearStuckProjectiles();

        while (dissolveTime <= 1f)
        {
            dissolveTime += Time.deltaTime;
            Renderer.material.SetFloat("_AlphaClipScale", dissolveTime);
            yield return null;
        }
        Destroy(gameObject);       
    }

    virtual protected void OnDestroy()
    {
        ClearStuckProjectiles();
    }

    virtual protected void ClearStuckProjectiles()
    {
        var projectiles = GetComponentsInChildren<Projectile>();
        if (!projectiles.Any()) return;
        for (int i = 0; i < projectiles.Length; i++)
        {
            projectiles[i].ReadyForBool();        
        }
    }

    virtual protected void SetIsDead(bool value)
    {
        if (isDead == value) return;
        isDead = value;
        EnableEnemyPassThroughLayerWhenDead();
        StopEnemyWhenDead();
        ChangeAgentRadius(isDead ? shrinkedAgentRadius : originalAgentRadius);
        EnableEnemyBodyRotationWhenDead();

        if (isDead)
        {
            StartCoroutine(DestroySmoothly());
            OnDeath?.Invoke(this);
        }
    }

    virtual protected void ChangeAgentRadius(float newAgentRadius)
    {
        Agent.radius = newAgentRadius;
    }

    virtual protected void EnableEnemyBodyRotationWhenDead()
    {
        Rigidbody.constraints = isDead ? RigidbodyConstraints.None : RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
    }

    virtual protected void StopEnemyWhenDead()
    {
        Agent.isStopped = isDead;
        Agent.velocity = isDead ? Vector3.zero : Agent.velocity;
    }

    virtual protected void EnableEnemyPassThroughLayerWhenDead()
    {
        EnemyBody.gameObject.layer = isDead ? LayerMask.NameToLayer("PassThrough") : LayerMask.NameToLayer("Enemy");
        gameObject.layer = isDead ? LayerMask.NameToLayer("PassThrough") : LayerMask.NameToLayer("Enemy");
    }

    virtual protected void SetReachedEnd(bool value)
    {
        if (isReachedEnd == value) return;
        isReachedEnd = value;
        if (isReachedEnd) OnReachEnd?.Invoke(this);
    }
}
