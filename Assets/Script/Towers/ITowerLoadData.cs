using TheTD.Towers;

public interface ITowerLoadData
{
    bool IsUnlocked { get; set; }
    string Name { get; }
    ITower Tower { get; }
    TowerType TowerType { get; }
} 