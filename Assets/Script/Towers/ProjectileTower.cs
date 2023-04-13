using System.Collections.Generic;
using System.Linq;
using TheTD.Enemies;
using TheTD.Projectiles;
using UnityEngine;

namespace TheTD.Towers
{
    public abstract class ProjectileTower : TowerBase
    {
        protected const string PathToProjectile = "Prefabs/Projectiles/";
        protected const string defaultProjectileName = "Arrow";

        [SerializeField] protected float maxShootForce = 10f;

        virtual protected string ProjectileName { get => GetProjectileName(); }

        [SerializeField] protected GameObject projectilePrefab;
        virtual protected GameObject ProjectilePrefab { get => projectilePrefab = projectilePrefab != null ? projectilePrefab : Resources.Load<GameObject>(PathToProjectile + ProjectileName); }

        virtual protected Projectile Projectile { get => GetProjectile(); }

        protected List<Projectile> projectiles = new List<Projectile>();

        protected ShootData currentShootData;

        protected override void Shoot()
        {
            Projectile.Launch(shootPoint.position, currentShootData.Velocity, transform);
        }

        protected override Enemy AimAtTarget()
        {
            currentShootData = CalculateShootData();
            if (!IsEnoughForceToShootTarget()) return null;
            Vector3 predictedPosition = currentShootData.Position;
            var aimPosition = predictedPosition;
            var aimDirection = aimPosition.normalized;
            TurnTurretTowardsAimDirection(aimDirection);
            isLockedToTarget = IsLockedOnEnemy(aimDirection);
            TryToShoot();
            return target;
        }

        virtual protected ShootData CalculateShootData()
        {
            currentShootData = CalculateDirectShootData(target.transform.position + target.EnemyBody.BodyCenterLocal, shootPoint.position);
            return CalculatePredictedPosition(currentShootData);
        }

        virtual protected ShootData CalculateDirectShootData(Vector3 targetPosition, Vector3 startPosition)
        {
            Vector3 displacement = new Vector3(targetPosition.x, startPosition.y, targetPosition.z) - startPosition;
            float deltaY = targetPosition.y - startPosition.y;
            float deltaXZ = displacement.magnitude;
            float gravity = Mathf.Abs(Physics.gravity.y);
            float reguiredShootForce = Mathf.Sqrt(gravity * (deltaY + Mathf.Sqrt(Mathf.Pow(deltaY, 2) + Mathf.Pow(deltaXZ, 2))));
            float shootForce = Mathf.Clamp(reguiredShootForce, 0.01f, maxShootForce);
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
            Vector3 targetMovement = target.Agent.velocity * time;
            Vector3 newTargetPosition = new Vector3(targetMovement.x + target.transform.position.x, shootPoint.position.y, targetMovement.z + target.transform.position.z);
            ShootData predictiveShootData = CalculateDirectShootData(newTargetPosition, shootPoint.position);
            predictiveShootData.Velocity = Vector3.ClampMagnitude(predictiveShootData.Velocity, maxShootForce);
            return predictiveShootData;
        }

        virtual protected Projectile GetProjectile()
        {
            var projectile = projectiles.Any() ? GetBulletFromBool() : SpawnProjectile();
            if (projectile == null)
            {
                projectile = SpawnProjectile();
            }
            return projectile;
        }

        virtual protected bool IsEnoughForceToShootTarget()
        {
            return currentShootData.ReguiredShootForce <= maxShootForce;
        }

        protected override Enemy IsTargetAvailable()
        {
            return IsInRange(transform.position, target.transform.position) && !target.IsDead ? target : null;
        }

        virtual protected Projectile SpawnProjectile()
        {
            var projectile = Instantiate(ProjectilePrefab, shootPoint.transform.position, shootPoint.transform.rotation).GetComponent<Projectile>();
            projectiles.Add(projectile);
            return projectile;
        }

        virtual protected Projectile GetBulletFromBool()
        {
            return projectiles.Find(o => o.gameObject.activeSelf == false);
        }

        virtual protected string GetProjectileName()
        {
            return defaultProjectileName;
        }
    }
}