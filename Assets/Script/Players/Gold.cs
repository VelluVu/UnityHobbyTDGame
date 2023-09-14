﻿using TheTD.Building;
using TheTD.Enemies;
using TheTD.Spawning;
using UnityEngine;

namespace TheTD.Players
{

    [System.Serializable]
    public class Gold : IEventListener
    {
        public int lastValueChange = 0;

        [SerializeField] private int _current = 0;
        public int Current { get => _current; }

        public delegate void GoldDelegate(Gold gold);
        internal event GoldDelegate OnSpend;
        internal event GoldDelegate OnGain;

        public void AddListeners()
        {
            SpawnersControl.Instance.OnEnemyKilled += OnEnemyKill;
            BuildArea.OnBuild += OnBuildTower;
            BuildArea.OnSell += OnSellTower;
        }

        public void RemoveListeners()
        {
            SpawnersControl.Instance.OnEnemyKilled -= OnEnemyKill;
            BuildArea.OnBuild -= OnBuildTower;
            BuildArea.OnSell -= OnSellTower;
        }

        private void OnSellTower(Construction building)
        {
            Add(building.IsNew ? building.Tower.BuildCost : Mathf.FloorToInt(building.Tower.BuildCost * 0.5f));
        }

        private void OnEnemyKill(WaveState waveState, Enemy enemy)
        {
            Add(Mathf.RoundToInt(enemy.BaseStats.GoldReward.Value));
        }

        private void OnBuildTower(Construction building)
        {
            Use(building.Tower.BuildCost);
        }

        private void Use(int value)
        {
            lastValueChange = value;
            _current -= value;
            OnSpend?.Invoke(this);
            if (_current <= 0)
            {
                _current = 0;
            }
        }

        private void Add(int value)
        {
            lastValueChange = value;
            _current += value;
            OnGain?.Invoke(this);
        }

        public bool HasGoldForTower(int goldCost)
        {
            return goldCost <= Current;
        }
    }
}