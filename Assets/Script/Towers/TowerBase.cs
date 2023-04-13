using System.Collections;
using System.Linq;
using TheTD.DamageSystem;
using TheTD.Enemies;
using TheTD.Spawning;
using UnityEngine;

namespace TheTD.Towers
{
    public abstract class TowerBase : MonoBehaviour
    {
        protected const string SHOOT_POINT_IS_NULL_LOG_FORMAT = "Shoot point is null, please assign shoot point transform to turret prefabs script";

        [SerializeField] protected int killCount = 0;

        [SerializeField] protected DamageProperties damageProperties;
        [SerializeField] protected bool isLockedToTarget = false;
        [SerializeField, Range(0f,1f)] protected float minimumLockOnEnemyDotProductRatio = 0.95f;
        //Add Scriptable Object Hierarchy for stat loading?
        [SerializeField] protected float targetFindInterval = 1f;
        [SerializeField] protected float turnSpeed = 2f;
        [SerializeField] protected float shootInterval = 1f;
        [SerializeField] protected float maxRange = 6f;
        [SerializeField] protected float debugDrawInterval = 1f;
        [SerializeField] protected Transform turretRotator;
        [SerializeField] protected Transform shootPoint;
        [SerializeField] protected Enemy target;
        [SerializeField] protected SearchTargetMethod searchTargetMethod = SearchTargetMethod.Closest;

        protected float timeToNextDraw = 0f;
        protected float nextShootTime = 0f;

        protected int _buildCost = 5;
        public int BuildCost { get => _buildCost; }

        protected TowerLoadData _towerData;
        virtual public TowerLoadData TowerData { get => _towerData; set => _towerData = value; }

        protected delegate Enemy TargetSearchDelegate(Enemy currentEnemy, Enemy previousEnemy);

        virtual protected void Start()
        {
            AddListeners();
            SetupDamageProperties();
            StartCoroutine(SearchTarget());
            StartCoroutine(TurretAI());
        }

        virtual protected void OnDestroy()
        {
            RemoveListeners();
        }

        virtual protected void AddListeners()
        {
            Enemy.OnDeath += OnEnemyDeath;
        }

        virtual protected void RemoveListeners()
        {
            Enemy.OnDeath -= OnEnemyDeath;
        }

        private void OnEnemyDeath(Enemy enemy, Damage damage)
        {
            if (damage.Attacker != transform) return;
            Debug.Log(this.name + " Killed " + enemy.name + " enemy!, gain " + enemy.ExperienceReward + " experience");
            killCount++;
        }

        protected virtual void SetupDamageProperties()
        {
            
        }

        virtual protected IEnumerator TurretAI()
        {
            while (true)
            {
                CheckTarget();
                yield return null;
            }
        }

        virtual protected void CheckTarget()
        {
            if (target == null) return;
            target = IsTargetAvailable() ? AimAtTarget() : null;
            ResetTimers();
        }

        virtual protected Enemy AimAtTarget()
        {
            var aimPosition = target.transform.position - shootPoint.transform.position + target.EnemyBody.BodyCenterLocal;
            var aimDirection = aimPosition.normalized;
            TurnTurretTowardsAimDirection(aimDirection);
            isLockedToTarget = IsLockedOnEnemy(aimDirection);
            TryToShoot();
            return target;
        }

        virtual protected void ResetTimers()
        {
            timeToNextDraw = Time.time > timeToNextDraw ? Time.time + debugDrawInterval : timeToNextDraw;
            nextShootTime = Time.time > nextShootTime ? Time.time + shootInterval : nextShootTime;
        }

        virtual public void BuildTower(Transform parent)
        {
            Instantiate(TowerData.TowerPrefab, parent.position, transform.rotation, parent);
        }

        virtual public bool IsLockedOnEnemy(Vector3 aimDirection)
        {
            Vector3 turretForward = turretRotator.transform.forward;
            Vector3 aimDirectionFlat = aimDirection;
            aimDirectionFlat.y = 0f;
            turretForward.y = 0f;

            var dot = Vector3.Dot(turretForward, aimDirectionFlat);
            DrawDebugLineInLoop(shootPoint.position, aimDirection + shootPoint.position, Color.red);
            float dotAbs = Mathf.Abs(dot);

            if (dotAbs >= minimumLockOnEnemyDotProductRatio)
            {
                return true;
            }
            return false;
        }

        virtual protected void TurnTurretTowardsAimDirection(Vector3 aimDirection)
        {
            turretRotator.transform.forward = Vector3.Slerp(turretRotator.transform.forward, aimDirection, Time.deltaTime * turnSpeed);
        }

        virtual protected bool IsInRange(Vector3 start, Vector3 end)
        {
            return Vector3.Distance(start, end) <= maxRange ? true : false;
        }

        virtual protected Enemy IsTargetAvailable()
        {
            return IsInRange(transform.position, target.transform.position) && !target.IsDead ? target : null;
        }

        virtual protected void DrawDebugLineInLoop(Vector3 startPos, Vector3 endPos, Color lineColor)
        {
            if (Time.time < timeToNextDraw) return;
            Debug.DrawLine(startPos, endPos, lineColor, debugDrawInterval);
        }

        virtual protected void TryToShoot()
        {
            if (!isLockedToTarget) return;
            if (Time.time < nextShootTime) return;
            if (shootPoint == null)
            {
                Debug.LogFormat(SHOOT_POINT_IS_NULL_LOG_FORMAT);
                return;
            }
            Shoot();
        }

        virtual protected IEnumerator SearchTarget()
        {
            while (true)
            {
                target = SelectTheCorrectMethod();
                yield return null;
            }
        }

        virtual protected Enemy SelectTheCorrectMethod()
        {
            switch (searchTargetMethod)
            {
                case SearchTargetMethod.Closest: return SearchEnemyWithSearchMethod(SearchClosest);
                case SearchTargetMethod.First: return SearchEnemyWithSearchMethod(SearchFirst);
                case SearchTargetMethod.Last: return SearchEnemyWithSearchMethod(SearchLast);
                case SearchTargetMethod.Healthiest: return SearchEnemyWithSearchMethod(SearchHealthiest);
                case SearchTargetMethod.Weakest: return SearchEnemyWithSearchMethod(SearchWeakest);
                default: return SearchEnemyWithSearchMethod(SearchClosest);
            }
        }

        virtual protected Enemy SearchEnemyWithSearchMethod(TargetSearchDelegate searchDelegate)
        {
            Enemy firstEnemy = null;
            if (!SpawnersControl.Instance.enemiesInLevel.Any()) return null;
            for (int i = 0; i < SpawnersControl.Instance.enemiesInLevel.Count; i++)
            {
                var currentEnemy = SpawnersControl.Instance.enemiesInLevel[i];
                if (currentEnemy == null) continue;
                if (firstEnemy == null)
                {
                    firstEnemy = currentEnemy;
                    continue;
                }

                firstEnemy = searchDelegate(currentEnemy, firstEnemy);
            }
            return firstEnemy;
        }

        virtual protected Enemy SearchFirst(Enemy currentEnemy, Enemy firstEnemy)
        {
            return IsInRange(shootPoint.position, currentEnemy.transform.position) && (Vector3.Distance(currentEnemy.transform.position, currentEnemy.Target.position) < Vector3.Distance(firstEnemy.transform.position, firstEnemy.Target.position)) ? currentEnemy : firstEnemy;
        }

        virtual protected Enemy SearchLast(Enemy currentEnemy, Enemy lastEnemy)
        {
            return IsInRange(shootPoint.position, currentEnemy.transform.position) && (Vector3.Distance(currentEnemy.transform.position, currentEnemy.Target.position) > Vector3.Distance(lastEnemy.transform.position, lastEnemy.Target.position)) ? currentEnemy : lastEnemy;
        }

        virtual protected Enemy SearchClosest(Enemy currentEnemy, Enemy closestEnemy)
        {
            return Vector3.Distance(transform.position, currentEnemy.transform.position) < Vector3.Distance(transform.position, closestEnemy.transform.position) ? currentEnemy : closestEnemy;
        }

        virtual protected Enemy SearchHealthiest(Enemy currentEnemy, Enemy healthiestEnemy)
        {
            return IsInRange(shootPoint.position, currentEnemy.transform.position) && (currentEnemy.CurrentHealth > healthiestEnemy.CurrentHealth) ? currentEnemy : healthiestEnemy;
        }

        virtual protected Enemy SearchWeakest(Enemy currentEnemy, Enemy weakestEnemy)
        {
            return IsInRange(shootPoint.position, currentEnemy.transform.position) && (currentEnemy.CurrentHealth < weakestEnemy.CurrentHealth) ? currentEnemy : weakestEnemy;
        }

        virtual protected void Shoot() { }
    }

    public enum SearchTargetMethod
    {
        Closest,
        First,
        Last,
        Healthiest,
        Weakest,
    }
}