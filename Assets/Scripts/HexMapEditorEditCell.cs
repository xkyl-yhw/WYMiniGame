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

    public Texture2D altitudeMap;
    public float maxHeight;
    public float minHeight = 0f;

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
        //hexGrid.SaveFile();
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
        float Radius = centerPos.x - 1;
        for (int i = 0; i < hexGrid.cells.Length; i++)
        {
            if(Vector3.Distance(new Vector3(hexGrid.cells[i].transform.localPosition.x, 0, hexGrid.cells[i].transform.localPosition.z), centerPos) > Radius)
            {
                hexGrid.cells[i].Elevation = 3;
            }
        }
    }

    public void LoadAltitudeMap()
    {
        int width = hexGrid.cellCountX;
        int height = hexGrid.cellCountZ;
        Vector3 pos = new Vector3((width + height * 0.5f - height / 2) * (HexMetrics.innerRadius * 2f), 0, height * (HexMetrics.outRadius * 1.5f));
        int texwidthPer = altitudeMap.width / (int)pos.x;
        int texheightPer = altitudeMap.height / (int)pos.z;
        for (int i = 0; i < pos.x; i++)
        {
            for (int j = 0; j < pos.z; j++)
            {
                Color color = altitudeMap.GetPixel(i * texwidthPer, j * texheightPer);
                
            }
        }
    }
}
