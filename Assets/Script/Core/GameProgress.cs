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
        [SerializeReference]public List<ITowerLoadData> towerLoadDatas = new List<ITowerLoadData>();

        public delegate void ProgressDelegate();
        public static event ProgressDelegate OnTowerProgressChange;

        private void Awake()
        {
            CheckSingleton();
            InitTowers();
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

        public List<ITowerLoadData> GetUnlockedTowers()
        {
            return towerLoadDatas.FindAll(o => o.IsUnlocked == true);
        }

        public void UnlockAllTowers()
        {
            towerLoadDatas.ForEach(o => o.IsUnlocked = true);
            OnTowerProgressChange?.Invoke();
        }

        public void UnlockTower(TowerType type)
        {
            var towerLoadData = FindTowerByType(type);
            towerLoadData.IsUnlocked = true;
            OnTowerProgressChange?.Invoke();
        }

        public ITowerLoadData FindTowerByType(TowerType type)
        {
            return towerLoadDatas.Find(o => o.TowerType == type);
        }

        private void Reset()
        {
            InitTowers();
        }

        public void ResetTowersList()
        {
            towerLoadDatas.Clear();
            InitTowers();
        }

        private void InitTowers()
        {
            if (towerLoadDatas.Any()) return;
            var gameObjects = Resources.LoadAll(TOWERS_PATH).ToList();
            gameObjects.ForEach(o =>
            {
                TowerLoadData tower = new TowerLoadData((TowerType)Enum.Parse(typeof(TowerType), o.name));
                tower.IsUnlocked = false;
                towerLoadDatas.Add(tower);
            });
            OnTowerProgressChange?.Invoke();
        }
    }
}