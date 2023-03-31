namespace TheTD.Towers
{
    public class ArrowTower : ProjectileTower
    {
        protected const string projectileName = "Arrow";

        protected override string GetProjectileName()
        {
            return projectileName;
        }
    }
}