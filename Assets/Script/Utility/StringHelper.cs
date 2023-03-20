public static class StringHelper
{
    public static string ConvertNumberToTwoDigitString(int number)
    {
        string numberString = number.ToString();
        if (number < 10) numberString = "0" + numberString;
        return numberString;
    }
}
