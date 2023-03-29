using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Progress : MonoBehaviour
{
    public bool devMode = false;
    private const string TOWERS_PATH = "Prefabs/Towers/";
    public List<TowerData> towers = new List<TowerData>();

    public delegate void ProgressDelegate(Progress progress);
    public static event ProgressDelegate OnTowerProgressChange;

    private void Start()
    {
        if(devMode)
        {
            UnlockAllTowers();
        }
    }

    public List<TowerData> GetUnlockedTowers()
    {
        return towers.FindAll(o => o.isUnlocked == true);
    }

    public void UnlockAllTowers()
    {
        towers.ForEach(o => o.isUnlocked = true);
        OnTowerProgressChange?.Invoke(this);
    }

    public void UnlockTower(TowerType type)
    {
        var tower = FindTowerByType(type);
        tower.isUnlocked = true;
        OnTowerProgressChange?.Invoke(this);
    }

    public TowerData FindTowerByType(TowerType type)
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
            TowerData tower = new TowerData((TowerType)Enum.Parse(typeof(TowerType),o.name));
            tower.isUnlocked = false;
            towers.Add(tower);
        });
        OnTowerProgressChange?.Invoke(this);
    }
}
