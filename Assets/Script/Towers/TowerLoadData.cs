using System;
using UnityEngine;

namespace TheTD.Towers
{
    [System.Serializable]
    public class TowerLoadData : ITowerLoadData
    {
        private const string PATH_TO_TOWER_PREFABS = "Prefabs/Towers/";

        public bool _isUnlocked = false;
        public bool IsUnlocked { get => _isUnlocked; set => _isUnlocked = value; }

        public string PathToPrefab { get => PATH_TO_TOWER_PREFABS + TowerType.ToString(); }
        public string Name { get => GetFormattedName(); }

        private GameObject towerPrefab;
        public GameObject TowerPrefab { get => towerPrefab = towerPrefab != null ? towerPrefab : Resources.Load<GameObject>(PathToPrefab); }

        private ITower tower;
        public ITower Tower { get => GetTower(); }

        protected TowerType _towerType = TowerType.BlockTower;
        public TowerType TowerType { get => _towerType; private set => _towerType = value; }

        private ITower GetTower()
        {
            if (tower != null) return tower;
            tower = TowerPrefab.GetComponent<TowerBase>();
            tower.TowerData = this;
            return tower;
        }

        public TowerLoadData(TowerType towerType)
        {
            TowerType = towerType;
        }

        private string GetFormattedName()
        {
            var typeAsString = TowerType.ToString();
            if (typeAsString.Length <= 1) return typeAsString;

            var formattedName = typeAsString;
            for (int i = 1; i < typeAsString.Length; i++)
            {
                if (Char.IsUpper(typeAsString[i]))
                {
                    formattedName = typeAsString.Insert(i, " ");
                }
            }
            return formattedName;
        }
    }
}
