using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tools 
{
    public enum ItemType
    {
        inital,
        wall,
        open,
        closed,
        path,
        start,
        end
    };

    public static float calculate_HValue(Vector3 start, Vector3 end)
    {
        
        Vector3 dist = end - start;

        return Mathf.Sqrt(Mathf.Pow(dist.x,2f) + Mathf.Pow(dist.y, 2f) + Mathf.Pow(dist.z, 2f));
    }
    public static float calculate_GValue()
    {
        float gValue = 0f;


        return gValue;
    }

    public static float calculate_FValue(float g, float h)
    {
        return g + h;
    }
}
