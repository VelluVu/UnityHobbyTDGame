using System;
using System.Collections.Generic;
using System.Linq;
using TheTD.Towers;
using UnityEngine;

namespace TheTD.Core
{
    public class GameProgress : MonoBehaviour
    {
        public static GameProgress Instance { get; private set; }

        public bool devMode = false;
        private const string TOWERS_PATH = "Prefabs/Towers/";
        public List<TowerLoadData> towers = new List<TowerLoadData>();

        public delegate void ProgressDelegate();
        public static event ProgressDelegate OnTowerProgressChange;

        private void Awake()
        {
            CheckSingleton();
        }

        private void CheckSingleton()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            if (devMode)
            {
                UnlockAllTowers();
            }
        }

        public List<TowerLoadData> GetUnlockedTowers()
        {
            return towers.FindAll(o => o.isUnlocked == true);
        }

        public void UnlockAllTowers()
        {
            towers.ForEach(o => o.isUnlocked = true);
            OnTowerProgressChange?.Invoke();
        }

        public void UnlockTower(TowerType type)
        {
            var tower = FindTowerByType(type);
            tower.isUnlocked = true;
            OnTowerProgressChange?.Invoke();
        }

        public TowerLoadData FindTowerByType(TowerType type)
        {
            return towers.Find(o => o.towerType == type);
        }

        private void Reset()
        {
            InitTowers();
        }

        public void ResetTowersList()
        {
            towers.Clear();
            InitTowers();
        }

        private void InitTowers()
        {
            if (towers.Any()) return;
            var gameObjects = Resources.LoadAll(TOWERS_PATH).ToList();
            gameObjects.ForEach(o =>
            {
                TowerLoadData tower = new TowerLoadData((TowerType)Enum.Parse(typeof(TowerType), o.name));
                tower.isUnlocked = false;
                towers.Add(tower);
            });
            OnTowerProgressChange?.Invoke();
        }
    }
}