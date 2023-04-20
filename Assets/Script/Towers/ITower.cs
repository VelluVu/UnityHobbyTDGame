using TheTD.Towers;
using UnityEngine;

public interface ITower
{
    int BuildCost { get; }
    TowerLoadData TowerData { get; set; }

    void BuildTower(Transform parent);
}