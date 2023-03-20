using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private const string END_POINT_TAG = "EndPoint";
    protected EnemyType type = EnemyType.Goblin;

    protected bool isInTheEnd = false;
    public bool IsInTheEnd { get => isInTheEnd; }

    private bool isDead;
    virtual public bool IsDead { get => isDead; set => SetIsDead(value); }

    [SerializeField] protected float currentHealth = 20f;
    virtual public float CurrentHealth { get => currentHealth; private set => currentHealth = value; }

    [SerializeField] protected float maxHealth = 20f;
    virtual public float MaxHealth { get => maxHealth; private set => maxHealth = value; }

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
        EnemyBody.Collider.enabled = true;
        Agent.isStopped = false;
        Agent.destination = Target.transform.position + Vector3.up * transform.position.y;
        StartCoroutine(CheckNavMeshState());
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
            isInTheEnd = true;
            gameObject.layer = LayerMask.NameToLayer("Default");
            StartCoroutine(DestroySmoothly());
        }
    }

    virtual protected void CheckDeath()
    {
        if (CurrentHealth > 0f) return;
        StartCoroutine(DestroySmoothly());
    }

    virtual protected IEnumerator CheckNavMeshState()
    {
        while (true)
        {
            if(Agent.pathStatus == NavMeshPathStatus.PathPartial)
            {
                OnPathBlocked?.Invoke(this);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    virtual protected IEnumerator DestroySmoothly()
    {
        ClearProjectiles();
        IsDead = true;
        EnemyBody.Collider.enabled = false;
        float dissolveTime = 0f;
        Agent.isStopped = true;
        Agent.velocity = Vector3.zero;

        while (dissolveTime <= 1f)
        {
            dissolveTime += Time.deltaTime;
            Renderer.material.SetFloat("_AlphaClipScale", dissolveTime);
            yield return null;      
        }

        Destroy(gameObject);
        OnReachEnd?.Invoke(this);
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
            projectiles[i].transform.SetParent(projectiles[i].OriginalParent);
            projectiles[i].enabled = true;
            projectiles[i].gameObject.SetActive(false);
        }
    }

    virtual protected void SetIsDead(bool value)
    {
        if (isDead == value) return;
        isDead = value;
        OnDeath?.Invoke(this);
    }
}
