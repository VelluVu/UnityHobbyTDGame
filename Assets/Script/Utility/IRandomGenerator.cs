public interface IRandomGenerator
{
    int GenerateRandomNumber(int min, int max);
    bool RollPercent(float percentChance);
    bool RollPercent(int percentChance);
}