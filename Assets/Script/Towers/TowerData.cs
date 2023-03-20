using System;
using UnityEngine;

[System.Serializable]
public class TowerData
{
    private const string PATH_TO_TOWER_PREFABS = "Prefabs/Towers/";

    public bool isUnlocked = false;
    public TowerType towerType = TowerType.BlockTower;

    public string PathToPrefab { get => PATH_TO_TOWER_PREFABS + towerType.ToString(); }
    public string Name { get => GetFormattedName(); }

    private GameObject towerPrefab;
    public GameObject TowerPrefab { get => towerPrefab = towerPrefab != null ? towerPrefab : Resources.Load<GameObject>(PathToPrefab); }

    private Tower tower;
    public Tower Tower { get => GetTower(); }

    private Tower GetTower()
    {
        if (tower != null) return tower;
        tower = TowerPrefab.GetComponent<Tower>();
        tower.TowerData = this;
        return tower;
    }

    public TowerData(TowerType towerType) 
    { 
        this.towerType = towerType;
    }

    private string GetFormattedName()
    {
        var typeAsString = towerType.ToString();
        if (typeAsString.Length <= 1) return typeAsString;

        var formattedName = typeAsString;
        for (int i = 1; i < typeAsString.Length; i++)
        {
            if(Char.IsUpper(typeAsString[i]))
            {
                formattedName = typeAsString.Insert(i, " ");
            }
        }
        return formattedName;
    }
}
