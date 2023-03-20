using System;

public static class ArrayHelper
{
    public static bool IsNullOrEmpty(this Array array)
    {
        return array == null || array.Length == 0;
    }
}
