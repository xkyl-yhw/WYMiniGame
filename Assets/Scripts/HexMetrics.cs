using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexMetrics
{
    public const float outRadius = 1f;
    public const float innerRadius = outRadius * 0.866025404f;

    public static Vector3[] corners =
    {
        new Vector3(0,0,outRadius),
        new Vector3(innerRadius, 0, 0.5f * outRadius),
        new Vector3(innerRadius, 0, -0.5f * outRadius),
        new Vector3(0, 0, -outRadius),
        new Vector3(-innerRadius, 0, -0.5f * outRadius),
        new Vector3(-innerRadius, 0, 0.5f * outRadius),
        new Vector3(0,0,outRadius)
    };
}
