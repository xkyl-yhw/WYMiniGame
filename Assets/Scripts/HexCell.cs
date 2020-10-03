using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;
    public Color color;

    [SerializeField]
    HexCell[] neighbors;

    public HexCell GetNeighbor(HexDirection dirction)
    {
        return neighbors[(int)dirction];
    }

    public void SetNeightbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

}
