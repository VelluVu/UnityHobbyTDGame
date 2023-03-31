namespace TheTD.Spawning
{
    [System.Serializable]
    public class Spawn
    {
        public int amount = 1;
        public float interval = 1f;
        public float startDelay = 0f;
        public EnemyType enemyType = EnemyType.Goblin;
    }
}