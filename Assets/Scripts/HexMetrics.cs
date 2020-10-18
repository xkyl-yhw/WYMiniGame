using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HexEdgeType
{
    Flat, Slope, Cliff
}

public static class HexMetrics
{
    public const int chunkSizeX = 5, chunkSizeZ = 5;

    public const float outRadius = 0.5f;
    public const float innerRadius = outRadius * 0.866025404f;

    public const float solidFactor = 0.75f;
    public const float blendFactor = 1f - solidFactor;

    public const float elevationStep = 0.1f;

    public const int terracesPerSlope = 2;
    public const int terraceSteps = terracesPerSlope * 2 + 1;
    public const float horizontalTerraceStepSize = 1f / terraceSteps;
    public const float verticalTerraceStepSize = 1f / (terracesPerSlope + 1);

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

    public static Vector3 TerraceLerp(Vector3 a, Vector3 b, int step)
    {
        float h = step * HexMetrics.horizontalTerraceStepSize;
        a.x += (b.x - a.x) * h;
        a.z += (b.z - a.z) * h;
        float v = ((step + 1) / 2) * HexMetrics.verticalTerraceStepSize;
        a.y += (b.y - a.y) * v;
        return a;
    }

    public static Color TerraceLerp(Color a, Color b, int step)
    {
        float h = step * HexMetrics.horizontalTerraceStepSize;
        return Color.Lerp(a, b, h);
    }

    public static HexEdgeType GetEdgeType(int elevation1,int elevation2)
    {
        if (elevation1 == elevation2)
        {
            return HexEdgeType.Flat;
        }
        int delta = elevation2 - elevation1;
        if (delta == 1 || delta == -1)
        {
            return HexEdgeType.Slope;
        }
        return HexEdgeType.Cliff;
    }
}
