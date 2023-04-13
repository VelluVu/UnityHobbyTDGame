using System.Collections.Generic;
using UnityEngine;

public static class ListHelperFunctions
{
    public static Vector3 MinVectorValueFromList(List<Vector3> list)
    {
        var min = list[0];
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].magnitude < min.magnitude)
            {
                min = list[i];
            }
        }
        return min;
    }

    public static Vector3 MaxVectorValueFromList(List<Vector3> list)
    {
        var max = list[0];
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].magnitude < max.magnitude)
            {
                max = list[i];
            }
        }
        return max;
    }
}
