using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private const string END_POINT_TAG = "EndPoint";
    protected EnemyType type = EnemyType.Goblin;

    protected bool reachedEnd = false;
    public bool ReachedEnd { get => reachedEnd; private set => SetReachedEnd(value); }

    private bool isDead;
    virtual public bool IsDead { get => isDead; set => SetIsDead(value); }

    [SerializeField] protected float currentHealth = 20f;
    virtual public float CurrentHealth { get => currentHealth; private set => currentHealth = value; }

    [SerializeField] protected float maxHealth = 20f;
    virtual public float MaxHealth { get => maxHealth; private set => maxHealth = value; }

    [SerializeField]protected int damage = 1;
    virtual public int Damage { get => damage; private set => damage = value; }

    protected NavMeshAgent agent;
    virtual public NavMeshAgent Agent { get => agent = agent != null ? agent : GetComponentInChildren<NavMeshAgent>(); }

    protected Transform target;
    virtual public Transform Target { get => target = target != null ? target : GameObject.FindGameObjectWithTag(END_POINT_TAG).transform; }

    protected Renderer _renderer;
    virtual public Renderer Renderer { get => _renderer = _renderer != null ? _renderer : GetComponentInChildren<Renderer>(); }

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
        ReachedEnd = false;
        IsDead = false;
        CurrentHealth = MaxHealth;
        Agent.destination = Target.transform.position + Vector3.up * transform.position.y;
    }

    virtual public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        CheckDeath();
    }

    virtual protected void Start()
    {
        AddListeners();
        StartMoving();
    }

    virtual protected void AddListeners()
    {
        EnemyBody.OnCollision += OnBodyCollision;
    }

    virtual protected void OnBodyCollision(Collider other)
    {
        if (other.CompareTag(END_POINT_TAG))
        {
            ReachedEnd = true;
            IsDead = true;
        }
    }

    virtual protected void CheckDeath()
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
        yield return new WaitForSeconds(2f);
        ClearProjectiles();

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
        ClearProjectiles();
    }

    virtual protected void ClearProjectiles()
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
        EnemyBody.gameObject.layer = isDead ? LayerMask.NameToLayer("PassThrough") : LayerMask.NameToLayer("Enemy");
        Agent.isStopped = isDead;
        Agent.velocity = isDead ? Vector3.zero : Agent.velocity;
        EnemyBody.Rigidbody.constraints = isDead ? RigidbodyConstraints.None : RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;

        if (isDead)
        {
            StartCoroutine(DestroySmoothly());
            OnDeath?.Invoke(this);
        }
    }

    virtual protected void SetReachedEnd(bool value)
    {
        if (reachedEnd == value) return;
        reachedEnd = value;
        if (reachedEnd) OnReachEnd?.Invoke(this);
    }
}
