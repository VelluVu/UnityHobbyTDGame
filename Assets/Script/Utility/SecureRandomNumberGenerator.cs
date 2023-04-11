using System;
using System.Security.Cryptography;

public class SecureRandomNumberGenerator
{
    private readonly RNGCryptoServiceProvider rng;

    public SecureRandomNumberGenerator()
    {
        rng = new RNGCryptoServiceProvider();
    }

    public int GenerateRandomNumber(int min, int max)
    {
        byte[] randomNumber = new byte[1];
        rng.GetBytes(randomNumber);
        double asciiValueOfRandomCharacter = Convert.ToDouble(randomNumber[0]);
        double multiplier = Math.Max(0, (asciiValueOfRandomCharacter / 255d) - 0.00000000001d);
        int range = max - min + 1;
        double randomValueInRange = Math.Floor(multiplier * range);
        return (int)(min + randomValueInRange);
    }

    public bool RollPercent(double percentChance)
    {
        if (percentChance <= 0) return false;
        if (percentChance >= 100) return true;
        int roll = GenerateRandomNumber(1, 100);
        return roll <= percentChance;
    }
}