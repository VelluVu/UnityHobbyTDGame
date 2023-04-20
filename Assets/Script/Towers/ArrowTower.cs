namespace TheTD.Towers
{
    public class ArrowTower : ProjectileTower
    {
        protected const string projectilePrefabName = "ArrowProjectile";

        protected override string GetProjectileName()
        {
            return projectilePrefabName;
        }
    }
}