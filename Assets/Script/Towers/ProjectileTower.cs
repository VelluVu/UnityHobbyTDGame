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
        protected ShootData _currentShootData;
        protected List<IProjectile> _projectiles = new List<IProjectile>();
        protected List<IOvertimeEffect> _overtimeEffects = new List<IOvertimeEffect>();
        protected Damage damage { get => new Damage(Stats.Damage, Stats.CriticalChange, Stats.CriticalDamageMultiplier, Stats.DamageType, _overtimeEffects, transform); }
        virtual protected string ProjectileName { get => GetProjectileName(); }
        virtual protected IProjectile Projectile { get => GetProjectile(); }
        private Vector3 _aimPosition = Vector3.zero;

        [SerializeField] protected GameObject projectilePrefab;
        virtual protected GameObject ProjectilePrefab { get => projectilePrefab = projectilePrefab != null ? projectilePrefab : Resources.Load<GameObject>(PATH_TO_PROJECTILE + ProjectileName); }

        protected override void Shoot()
        {
            Projectile.Launch(ShootPoint.position, _currentShootData.Velocity, transform, damage);
        }

        protected override ITargetable AimAtTarget()
        {
            _currentShootData = CalculateShootData();
            if (!IsEnoughForceToShootTarget()) return null;
            Vector3 predictedPosition = _currentShootData.Position;
            var aimDirection = predictedPosition.normalized;
            TurnTurretTowardsAimDirection(aimDirection);
            _isLockedToTarget = IsLockedOnTarget(aimDirection);
            TryToShoot();
            return _currentTarget;
        }

        virtual protected ShootData CalculateShootData()
        {
            _aimPosition = _currentTarget.Position + _currentTarget.BodyCenter;
            _currentShootData = CalculateDirectShootData(_aimPosition, ShootPoint.position);
            return CalculatePredictedPosition(_currentShootData);
        }

        virtual protected ShootData CalculateDirectShootData(Vector3 targetPosition, Vector3 startPosition)
        {
            Vector3 displacement = new Vector3(targetPosition.x, startPosition.y, targetPosition.z) - startPosition;
            float deltaY = targetPosition.y - startPosition.y;
            float deltaXZ = displacement.magnitude;
            float gravity = Mathf.Abs(Physics.gravity.y);
            float reguiredShootForce = Mathf.Sqrt(gravity * (deltaY + Mathf.Sqrt(Mathf.Pow(deltaY, 2) + Mathf.Pow(deltaXZ, 2))));
            float shootForce = Mathf.Clamp(reguiredShootForce, 0.01f, _maxShootForce);
            float PIDividedByTwo = Mathf.PI / 2f;
            float shootAngle = PIDividedByTwo - (0.5f * (PIDividedByTwo - (deltaY / deltaXZ)));
            Vector3 initialVelocity = Mathf.Cos(shootAngle) * shootForce * displacement.normalized + Mathf.Sin(shootAngle) * shootForce * Vector3.up;
            return new ShootData(displacement, initialVelocity, shootAngle, deltaXZ, deltaY, reguiredShootForce);
        }

        virtual protected ShootData CalculatePredictedPosition(ShootData directShootData)
        {
            Vector3 shootVelocity = directShootData.Velocity;
            shootVelocity.y = 0f;
            float time = directShootData.DeltaXZ / shootVelocity.magnitude;
            Vector3 targetMovement = _currentTarget.Velocity * time;
            Vector3 newTargetPosition = new Vector3(targetMovement.x + _currentTarget.Position.x, ShootPoint.position.y, targetMovement.z + _currentTarget.Position.z);
            ShootData predictiveShootData = CalculateDirectShootData(newTargetPosition, ShootPoint.position);
            predictiveShootData.Velocity = Vector3.ClampMagnitude(predictiveShootData.Velocity, _maxShootForce);
            return predictiveShootData;
        }

        virtual protected IProjectile GetProjectile()
        {
            var projectile = _projectiles.Any() ? GetBulletFromBool() : SpawnProjectile();
            if (projectile == null)
            {
                projectile = SpawnProjectile();
            }
            return projectile;
        }

        virtual protected bool IsEnoughForceToShootTarget()
        {
            return _currentShootData.ReguiredShootForce <= _maxShootForce;
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

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_aimPosition, 0.15f);
        }
    }
}