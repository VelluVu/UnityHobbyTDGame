using System.Collections.Generic;
using System.Linq;
using TheTD.DamageSystem;
using UnityEngine;

namespace TheTD.Towers
{
    public abstract class ProjectileTower : TowerBase
    {
        protected const string PATH_TO_PROJECTILE = "Prefabs/Projectiles/";
        protected const string DEFAULT_PROJECTILE_NAME = "ArrowProjectile";

        [SerializeField] protected float _maxShootForce = 10f;
        [SerializeField, Range(0f, 1f)] protected float _forceRatio = 0.0f;
        public float launchStrength;

        [SerializeField] private bool _useMovementPrediction;
        [Range(0.01f, 5f)] public float historicalTime = 1f;
        [Range(1, 100)] public int historicalResolution = 10;
        public PredictionMode movementPredictionMode;
        private Queue<Vector3> _historicalPositions;
        private float _historicalPositionInterval;
        private float _lastHistoryRecordedTime;
        protected TrajectoryData _trajectoryData;
        protected List<IProjectile> _projectiles = new List<IProjectile>();
        protected List<IOvertimeEffect> _overtimeEffects = new List<IOvertimeEffect>();
        protected Damage damage { get => new Damage(Stats.Damage, Stats.CriticalChange, Stats.CriticalDamageMultiplier, Stats.DamageType, _overtimeEffects, transform); }
        virtual protected string ProjectileName { get => GetProjectileName(); }
        virtual protected IProjectile Projectile { get => GetProjectile(); }
        public Vector3 TargetCenterPosition => _currentTarget.Position + _currentTarget.BodyCenter;

        [SerializeField] protected GameObject projectilePrefab;
        virtual protected GameObject ProjectilePrefab { get => projectilePrefab = projectilePrefab != null ? projectilePrefab : Resources.Load<GameObject>(PATH_TO_PROJECTILE + ProjectileName); }

        protected override void Start()
        {
            base.Start();

            int capacity = Mathf.CeilToInt(historicalResolution * historicalTime);
            _historicalPositions = new Queue<Vector3>(capacity);
            for (int i = 0; i < capacity; i++)
            {
                if (_currentTarget != null)
                {
                    _historicalPositions.Enqueue(CurrentTarget.Position);
                }
            }
            _historicalPositionInterval = historicalTime / historicalResolution;
        }

        protected override void Shoot()
        {
            Projectile.Launch(_trajectoryData, transform, damage, _currentTarget);
        }

        override protected void Update()
        {
            base.Update();

            if (_lastHistoryRecordedTime + _historicalPositionInterval < Time.time)
            {
                _lastHistoryRecordedTime = Time.time;
                if (_historicalPositions.Any() && _currentTarget != null)
                {
                    _historicalPositions.Dequeue();
                    _historicalPositions.Enqueue(CurrentTarget.Position);
                }
            }
        }

        protected override ITargetable AimAtTarget()
        {
            _trajectoryData = CalculateTrajectoryData(ShootPoint.position, TargetCenterPosition);
            //Equations.CalculateVelocity(ShootPoint.position, TargetCenterPosition);
            if (_useMovementPrediction)
            {
                _trajectoryData = GetPredictedPositionTrajectoryData(_trajectoryData);
            }

            DrawDebugLineInLoop(ShootPoint.position, _trajectoryData.TargetPosition, Color.red);
            TurnTurretTowardsAimDirection(_trajectoryData.AimDirection);
            _isLockedToTarget = IsLockedOnTarget(_trajectoryData.AimDirection);
            TryToShoot();
            return _currentTarget;
        }

        private TrajectoryData GetPredictedPositionTrajectoryData(TrajectoryData directTrajectoryData)
        {
            Vector3 projectileVelocity = directTrajectoryData.Velocity;
            projectileVelocity.y = 0;
            float time = directTrajectoryData.DeltaXZ / projectileVelocity.magnitude;
            Vector3 enemyMovement;

            if (movementPredictionMode == PredictionMode.CurrentVelocity)
            {              
                enemyMovement = CurrentTarget.Velocity * time;
            }
            else
            {
                Vector3[] positions = _historicalPositions.ToArray();
                Vector3 averageVelocity = Vector3.zero;
                for (int i = 1; i < positions.Length; i++)
                {
                    averageVelocity += (positions[i] - positions[i - 1]) / _historicalPositionInterval;
                }
                averageVelocity /= historicalTime * historicalResolution;
                enemyMovement = averageVelocity;

            }

            Vector3 newTargetPosition = new Vector3(
                directTrajectoryData.TargetPosition.x + enemyMovement.x,
                directTrajectoryData.TargetPosition.y + enemyMovement.y,
                directTrajectoryData.TargetPosition.z + enemyMovement.z
            );

            // Option Calculate again the trajectory based on target position
            TrajectoryData predictiveThrowData = CalculateTrajectoryData(
                directTrajectoryData.StartPosition,
                newTargetPosition
            );

            return predictiveThrowData;
        }

        private TrajectoryData CalculateTrajectoryData(Vector3 startPosition, Vector3 targetPosition)
        {
            // v = initial velocity, assume max speed for now
            // x = distance to travel on X/Z plane only
            // y = difference in altitudes from thrown point to target hit point
            // g = gravity

            Vector3 displacement = new Vector3(
                targetPosition.x,
                startPosition.y,
                targetPosition.z
            ) - startPosition;
            float deltaY = targetPosition.y - startPosition.y;
            float deltaXZ = displacement.magnitude;

            // find lowest initial launch velocity with other magic formula from https://en.wikipedia.org/wiki/Projectile_motion
            // v^2 / g = y + sqrt(y^2 + x^2)
            // meaning.... v = sqrt(g * (y+ sqrt(y^2 + x^2)))
            float gravity = Mathf.Abs(Physics.gravity.y);
            launchStrength = Mathf.Clamp(Mathf.Sqrt(gravity * (deltaY + Mathf.Sqrt(Mathf.Pow(deltaY, 2) + Mathf.Pow(deltaXZ, 2)))), 0.01f, _maxShootForce);
            launchStrength = Mathf.Lerp(launchStrength, _maxShootForce, _forceRatio);
            float angle;

            if (_forceRatio == 0)
            {
                // optimal angle is chosen with a relatively simple formula
                angle = Mathf.PI / 2f - (0.5f * (Mathf.PI / 2 - (deltaY / deltaXZ)));
            }
            else
            {
                // when we know the initial velocity, we have to calculate it with this formula
                // Angle to throw = arctan((v^2 +- sqrt(v^4 - g * (g * x^2 + 2 * y * v^2)) / g*x)
                angle = Mathf.Atan(
                    (Mathf.Pow(launchStrength, 2) - Mathf.Sqrt(
                        Mathf.Pow(launchStrength, 4) - gravity
                        * (gravity * Mathf.Pow(deltaXZ, 2)
                        + 2 * deltaY * Mathf.Pow(launchStrength, 2)))
                    ) / (gravity * deltaXZ)
                );
            }

            Vector3 initialVelocity =
                Mathf.Cos(angle) * launchStrength * displacement.normalized
                + Mathf.Sin(angle) * launchStrength * Vector3.up;

            float time = deltaXZ / initialVelocity.magnitude;

            return new TrajectoryData(startPosition, targetPosition, initialVelocity, angle, deltaXZ, deltaY, time);
        }

        virtual protected IProjectile GetProjectile()
        {
            var projectile = _projectiles.Any() ? GetBulletFromBool() : SpawnProjectile();
            return projectile != null ? projectile : SpawnProjectile();
        }

        protected override ITargetable IsTargetAvailable()
        {
            return IsInRange(transform.position, _currentTarget.Position) && !_currentTarget.IsDestroyed ? _currentTarget : null;
        }

        virtual protected IProjectile SpawnProjectile()
        {
            var projectile = Instantiate(ProjectilePrefab, ShootPoint.transform.position, ShootPoint.transform.rotation).GetComponent<IProjectile>();
            _projectiles.Add(projectile);
            return projectile;
        }

        virtual protected IProjectile GetBulletFromBool()
        {
            return _projectiles.Find(o => o.IsActive == false);
        }

        virtual protected string GetProjectileName()
        {
            return DEFAULT_PROJECTILE_NAME;
        }

        private void OnDrawGizmos()
        {
            if (_currentTarget != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(TargetCenterPosition, 0.15f);
            }
        }
    }
    public enum PredictionMode
    {
        CurrentVelocity,
        AverageVelocity
    }
}