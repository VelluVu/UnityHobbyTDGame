using System;
using System.Collections.Generic;
using TheTD.DamageSystem;
using TheTD.Enemies;
using TheTD.StatSystem;
using UnityEngine;

namespace TheTD.ScriptableFiniteStateMachine
{
    public class EnemyFiniteStateMachine : FiniteStateMachine
    {
        private Enemy _enemy;
        public Enemy Enemy { get => _enemy = _enemy != null ? _enemy : GetComponent<Enemy>(); }

        protected override void AddListeners()
        {
            Enemy.OnReachEnd += OnEnemyReachEnd;
            Enemy.OnDeath += OnEnemyDeath;     
        }

        protected override void RemoveListeners()
        {
            Enemy.OnReachEnd -= OnEnemyReachEnd;
            Enemy.OnDeath -= OnEnemyDeath;
        }

        private void OnEnemyReachEnd(Enemy enemy)
        {
            if(enemy != Enemy) return;
            HasReachedEnd = enemy.HasReachedEnd;
        }

        private void OnEnemyDeath(Enemy enemy, Damage damage)
        {
            if(enemy == Enemy) IsDead = enemy.IsDead;
        }

        protected override MovementStats GetMovementStats()
        {
            return Enemy.Stats.movementStats;
        }

        protected override Vector3 GetHeightOffset()
        {
            return Enemy.BodyCenter;
        }

        protected override Rigidbody GetRigidBody()
        {
            return Enemy.Rigidbody;
        }
    }
}