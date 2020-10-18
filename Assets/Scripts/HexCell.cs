using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HexCellMsgArray
{
    public List<HexCellMsg> cellArray;
}


[System.Serializable]
public class HexCellMsg
{
    public Color color;
    public int Elevation;
    public int x;
    public int y;
}


public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;
    Color color;

    public int x, z;

    [SerializeField]
    HexCell[] neighbors;

    public int Elevation
    {
        get
        {
            return elevation;
        }
        set
        {
            if (elevation == value)
            {
                return;
            }
            elevation = value;
            Vector3 position = transform.localPosition;
            position.y = value * HexMetrics.elevationStep;
            transform.localPosition = position;
            Refresh();
        }
    }

    public Color Color
    {
        get
        {
            return color;
        }
        set
        {
            if (color == value)
            {
                return;
            }
            color = value;
            Refresh();
        }
    }

    //public float Height
    //{
    //    get
    //    {
    //        return height;
    //    }
    //    set
    //    {
    //        if (height == value)
    //            return;
    //        height = value;
    //        Vector3 position = transform.localPosition;
    //        position.y = value;
    //        transform.localPosition = position;
    //        Refresh();
    //    }
    //}

    int elevation = int.MinValue;
    //float height = float.MinValue;
    public HexGridChunk chunk;

    public HexCell GetNeighbor(HexDirection dirction)
    {
        return neighbors[(int)dirction];
    }

    public void SetNeightbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    public HexEdgeType GetEdgeType(HexDirection direction)
    {
        return HexMetrics.GetEdgeType(
            elevation, neighbors[(int)direction].elevation
        );
    }

    public HexEdgeType GetEdgeType(HexCell otherCell)
    {
        return HexMetrics.GetEdgeType(elevation, otherCell.elevation);
    }

    void Refresh()
    {
        if (chunk)
        {
            chunk.Refresh();
            for (int i = 0; i < neighbors.Length; i++)
            {
                HexCell neighbor = neighbors[i];
                if (neighbor != null && neighbor.chunk != chunk)
                {
                    neighbor.chunk.Refresh();
                }
            }
        }
        //GameObject[] poss = GameObject.FindGameObjectsWithTag("OccupyPos");

        //for (int i = 0; i < poss.Length; i++)
        //{
        //    if (poss[i].GetComponent<OccupationPos>().occupyMode == OccupyMode.area)
        //    {
        //        poss[i].GetComponent<OccupationPos>().AreaCountMethod();
        //    }
        //}
    }
}
