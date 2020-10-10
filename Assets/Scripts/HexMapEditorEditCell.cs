using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMapEditorEditCell : MonoBehaviour
{
    public HexGrid hexGrid;
    public Color touchedColor = Color.magenta;

    int activeElevation;

    public Transform circleCenter;
    public bool drawBtn;

    private void Update()
    {
        if (drawBtn)
        {
            drawBtn = false;
            drawCircle();
        }
        if (Input.GetMouseButton(0))
            HandleInput();
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            EditCell(hexGrid.GetCell(hit.point));
        }
    }
    void EditCell(HexCell cell)
    {
        cell.Elevation = activeElevation;
        hexGrid.SaveFile();
    }

    public void SetElevation(float elevation)
    {
        activeElevation = (int)elevation;
    }

    public void drawCircle()
    {
        int x = hexGrid.chunkCountX * HexMetrics.chunkSizeX / 2;
        int z = hexGrid.chunkCountZ * HexMetrics.chunkSizeZ / 2;
        Vector3 centerPos= new Vector3((x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f), 0, z * (HexMetrics.outRadius * 1.5f));
        float Radius = centerPos.z - 1;
        for (int i = 0; i < hexGrid.cells.Length; i++)
        {
            if(Vector3.Distance(new Vector3(hexGrid.cells[i].transform.localPosition.x, 0, hexGrid.cells[i].transform.localPosition.z), centerPos) > Radius)
            {
                hexGrid.cells[i].Elevation = 3;
            }
        }
    }
}
