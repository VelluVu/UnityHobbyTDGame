namespace TheTD.ExperienceSystem
{
    public interface ILevelable
    {
        int Level { get; }
        float Experience { get; }

        void GainExperience(float experience);
        void GainLevel();

        public delegate void LevelDelegate(ILevelable levelable);
        public event LevelDelegate OnGainExperience;
        public event LevelDelegate OnLevel;
    }
}