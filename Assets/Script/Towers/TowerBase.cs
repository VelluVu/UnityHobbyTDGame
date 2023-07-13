using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheTD.DamageSystem;
using TheTD.Spawning;
using UnityEngine;

namespace TheTD.Towers
{
    public abstract class TowerBase : MonoBehaviour, ITower
    {
        protected const string SHOOT_POINT_IS_NULL_LOG_FORMAT = "Shoot point is null, please assign shoot point transform to turret prefabs script";
        protected const string PATH_TO_BASE_STATS = "ScriptableObjects/Towers/Stats/";
        protected const string ERROR_GET_TURRET_ROTATOR = "Unable to get turret rotator from the children, override getter, or make sure the shoot point game object is named: TurretRotator";
        protected const string ERROR_GET_SHOOT_POINT = "Unable to get shoot point from the children, override getter, or make sure the shoot point game object is named: ShootPoint";

        [SerializeField] protected bool _isLockedToTarget = false;
        [SerializeField] protected int _killCount = 0;
        [SerializeField, Range(0f, 1f)] protected float _minimumLockOnEnemyDotProductRatio = 0.95f;
        [SerializeField] protected float _debugDrawInterval = 1f;
        protected float _timeToNextDraw = 0f;
        protected float _nextShootTime = 0f;
        [SerializeField] protected SearchTargetMethod _searchTargetMethod = SearchTargetMethod.Closest;
        List<ITargetable> _oldTargets = new List<ITargetable>();

        public int BuildCost => Stats.BuildCost.RoundedValue;

        [SerializeField] protected Transform _turretRotator;
        public Transform TurretRotator { get => GetTurretRotator(); }

        [SerializeField] protected Transform _shootPoint;
        public Transform ShootPoint { get => GetShootPoint(); }

        [SerializeField] protected ITargetable _currentTarget;
        protected ITargetable CurrentTarget { get => _currentTarget; set => SetCurrentTarget(value); }

        protected TowerBaseStats _baseStats;
        public TowerBaseStats BaseStats { get => GetBaseStats(); }

        protected DynamicTowerStats _stats;
        public DynamicTowerStats Stats { get => GetDynamicStats(); }

        protected TowerLoadData _towerData;
        virtual public TowerLoadData TowerData { get => _towerData; set => _towerData = value; }

        protected delegate ITargetable TargetSearchDelegate(ITargetable ProcessedTarget, ITargetable target);

        virtual protected void Start()
        {
            StartCoroutine(SearchTarget());
            StartCoroutine(TurretAI());
        }

        private void OnTargetDestroy(ITargetable destroyedTarget, Damage damage)
        {
            if (_oldTargets.Contains(destroyedTarget))
            {
                destroyedTarget.OnEliminated -= OnTargetDestroy;
                _oldTargets.Remove(destroyedTarget);
            }

            if (damage.Source != transform) return;

            Debug.Log(this.name + " Killed " + destroyedTarget.Name + " enemy!, gain " + destroyedTarget.XPReward + " experience");
            _killCount++;
            _currentTarget.OnEliminated -= OnTargetDestroy;
            _currentTarget = null;
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
            if (_currentTarget == null) return;
            _currentTarget = IsTargetAvailable() != null ? AimAtTarget() : null;
            ResetTimers();
        }

        virtual protected ITargetable AimAtTarget()
        {
            var aimPosition = _currentTarget.Position - ShootPoint.transform.position + _currentTarget.BodyCenter;
            var aimDirection = aimPosition.normalized;
            TurnTurretTowardsAimDirection(aimDirection);
            _isLockedToTarget = IsLockedOnTarget(aimDirection);
            TryToShoot();
            return _currentTarget;
        }

        virtual protected void ResetTimers()
        {
            _timeToNextDraw = Time.time > _timeToNextDraw ? Time.time + _debugDrawInterval : _timeToNextDraw;
            _nextShootTime = Time.time > _nextShootTime ? Time.time + Stats.ShootInterval.Value : _nextShootTime;
        }

        virtual public void BuildTower(Transform parent)
        {
            Instantiate(TowerData.TowerPrefab, parent.position, transform.rotation, parent);
            //AstarPath.active.Scan();
        }

        virtual public bool IsLockedOnTarget(Vector3 aimDirection)
        {
            Vector3 turretForward = TurretRotator.transform.forward;
            Vector3 aimDirectionFlat = aimDirection;
            aimDirectionFlat.y = 0f;
            turretForward.y = 0f;

            var dot = Vector3.Dot(turretForward, aimDirectionFlat);
            DrawDebugLineInLoop(ShootPoint.position, aimDirection + ShootPoint.position, Color.red);
            float dotAbs = Mathf.Abs(dot);

            if (dotAbs >= _minimumLockOnEnemyDotProductRatio)
            {
                return true;
            }
            return false;
        }

        virtual protected void TurnTurretTowardsAimDirection(Vector3 aimDirection)
        {
            TurretRotator.transform.forward = Vector3.Slerp(TurretRotator.transform.forward, aimDirection, Time.deltaTime * Stats.TurnSpeed.Value);
        }

        virtual protected bool IsInRange(Vector3 start, Vector3 end)
        {
            return Vector3.Distance(start, end) <= Stats.MaxRange.Value ? true : false;
        }

        virtual protected ITargetable IsTargetAvailable()
        {
            return IsInRange(transform.position, _currentTarget.Position) && !_currentTarget.IsDestroyed ? _currentTarget : null;
        }

        virtual protected void DrawDebugLineInLoop(Vector3 startPos, Vector3 endPos, Color lineColor)
        {
            if (Time.time < _timeToNextDraw) return;
            Debug.DrawLine(startPos, endPos, lineColor, _debugDrawInterval);
        }

        virtual protected void TryToShoot()
        {
            if (!_isLockedToTarget) return;
            if (Time.time < _nextShootTime) return;
            if (ShootPoint == null)
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
                _currentTarget = SelectTheCorrectMethod();
                yield return null;
            }
        }

        virtual protected ITargetable SelectTheCorrectMethod()
        {
            switch (_searchTargetMethod)
            {
                case SearchTargetMethod.Closest: return SearchEnemyWithSearchMethod(SearchClosest);
                case SearchTargetMethod.First: return SearchEnemyWithSearchMethod(SearchFirst);
                case SearchTargetMethod.Last: return SearchEnemyWithSearchMethod(SearchLast);
                case SearchTargetMethod.Healthiest: return SearchEnemyWithSearchMethod(SearchHealthiest);
                case SearchTargetMethod.Weakest: return SearchEnemyWithSearchMethod(SearchWeakest);
                default: return SearchEnemyWithSearchMethod(SearchClosest);
            }
        }

        virtual protected ITargetable SearchEnemyWithSearchMethod(TargetSearchDelegate searchDelegate)
        {
            ITargetable firstTarget = null;
            if (!SpawnersControl.Instance.enemiesInLevel.Any()) return null;
            for (int i = 0; i < SpawnersControl.Instance.enemiesInLevel.Count; i++)
            {
                var currentEnemy = SpawnersControl.Instance.enemiesInLevel[i];
                if (currentEnemy == null) continue;
                if (firstTarget == null)
                {
                    firstTarget = currentEnemy;
                    continue;
                }

                firstTarget = searchDelegate(currentEnemy, firstTarget);
            }
            return firstTarget;
        }


        virtual protected ITargetable SearchFirst(ITargetable proceccedTarget, ITargetable firstTarget)
        {
            return IsInRange(ShootPoint.position, proceccedTarget.Position) && (Vector3.Distance(proceccedTarget.Position, proceccedTarget.Position) < Vector3.Distance(firstTarget.Position, firstTarget.Position)) ? proceccedTarget : firstTarget;
        }

        virtual protected ITargetable SearchLast(ITargetable oricessedTarget, ITargetable lastTarget)
        {
            return IsInRange(ShootPoint.position, oricessedTarget.Position) && (Vector3.Distance(oricessedTarget.Position, oricessedTarget.Position) > Vector3.Distance(lastTarget.Position, lastTarget.Position)) ? oricessedTarget : lastTarget;
        }

        virtual protected ITargetable SearchClosest(ITargetable processedTarget, ITargetable closestTarget)
        {
            return Vector3.Distance(transform.position, processedTarget.Position) < Vector3.Distance(transform.position, closestTarget.Position) ? processedTarget : closestTarget;
        }

        virtual protected ITargetable SearchHealthiest(ITargetable processedTarget, ITargetable healthiestTarget)
        {
            return IsInRange(ShootPoint.position, processedTarget.Position) && (processedTarget.Health > healthiestTarget.Health) ? processedTarget : healthiestTarget;
        }

        virtual protected ITargetable SearchWeakest(ITargetable processedTarget, ITargetable weakestTarget)
        {
            return IsInRange(ShootPoint.position, processedTarget.Position) && (processedTarget.Health < weakestTarget.Health) ? processedTarget : weakestTarget;
        }

        virtual protected void SetCurrentTarget(ITargetable value)
        {
            if (value == _currentTarget) return;
            _oldTargets.Add(_currentTarget);
            _currentTarget = value;
            _currentTarget.OnEliminated += OnTargetDestroy;
        }

        virtual protected Transform GetTurretRotator()
        {
            if (_turretRotator != null) return _turretRotator;
            var children = GetComponentsInChildren<Transform>();
            var turretRotator = children.First(o => o.gameObject.name == "TurretRotator");
            if (turretRotator == null) Debug.LogWarning(ERROR_GET_TURRET_ROTATOR);
            return turretRotator;
        }

        protected Transform GetShootPoint()
        {
            if (_shootPoint != null) return _shootPoint;
            var children = GetComponentsInChildren<Transform>();
            var shootPoint = children.First(o => o.gameObject.name == "ShootPoint");
            if (shootPoint == null) Debug.LogWarning(ERROR_GET_SHOOT_POINT);
            return shootPoint;
        }

        virtual protected TowerBaseStats GetBaseStats()
        {
            if (_baseStats != null) return _baseStats;
            string fileName = name.Replace("(Clone)", "") + "Stats";
            _baseStats = Resources.Load<TowerBaseStats>(PATH_TO_BASE_STATS + fileName);
            return _baseStats;
        }

        virtual protected DynamicTowerStats GetDynamicStats()
        {
            if (_stats != null) return _stats;
            _stats = new DynamicTowerStats(BaseStats);
            return _stats;
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
