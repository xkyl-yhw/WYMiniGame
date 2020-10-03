using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexMetrics
{
    public const float outRadius = 0.5f;
    public const float innerRadius = outRadius * 0.866025404f;

    public const float solidFactor = 0.75f;
    public const float blendFactor = 1f - solidFactor;

    static Vector3[] corners =
    {
        new Vector3(0,0,outRadius),
        new Vector3(innerRadius, 0, 0.5f * outRadius),
        new Vector3(innerRadius, 0, -0.5f * outRadius),
        new Vector3(0, 0, -outRadius),
        new Vector3(-innerRadius, 0, -0.5f * outRadius),
        new Vector3(-innerRadius, 0, 0.5f * outRadius),
        new Vector3(0,0,outRadius)
    };

    public static Vector3 GetFirstCorner(HexDirection direction)
    {
        return corners[(int)direction];
    }

    public static Vector3 GetSecondCorner(HexDirection direction)
    {
        return corners[(int)direction + 1];
    }

    public static Vector3 GetFirstSolidCorner(HexDirection direction)
    {
        return corners[(int)direction] * solidFactor;
    }

    public static Vector3 GetSecondSolidCorner(HexDirection direction)
    {
        return corners[(int)direction + 1] * solidFactor;
    }

    public static Vector3 GetBridge(HexDirection direction)
    {
        return (corners[(int)direction] + corners[(int)direction + 1]) * blendFactor;
    }
}
