using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
    public int width = 6;
    public int height = 6;
    public HexCell cellPrefeb;

    //public Text cellLabelPrefebs;
    //Canvas gridCanvas;

    HexCell[] cells;

    HexMesh hexMesh;

    public Color defaultColor = Color.white;
    public Color touchedColor = Color.magenta;

    private void Awake()
    {
        //gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh = GetComponentInChildren<HexMesh>();
        cells = new HexCell[width * height];
        for (int i = 0, x = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                CreateCell(j, i, x++);
            }
        }
    }

    private void Start()
    {
        hexMesh.Triangulate(cells);
    }

    /*
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleInput();
        }
    }
    */

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            InfectCell(hit.point, touchedColor);
        }
    }

    public void InfectCell(Vector3 pos, Color color)
    {
        pos = transform.InverseTransformPoint(pos);
        HexCoordinates coordinates = HexCoordinates.FromPos(pos);
        int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
        HexCell cell = cells[index];
        if (cell.color != defaultColor && cell.color == color) return;
        cell.color = color;
        hexMesh.Triangulate(cells);
    }

    void CreateCell(int x, int z, int i)
    {
        Vector3 pos = new Vector3((x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f), 0, z * (HexMetrics.outRadius * 1.5f));
        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefeb, transform, false);
        cell.transform.localPosition = pos;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.color = defaultColor;
        if (x > 0)
        {
            cell.SetNeightbor(HexDirection.W, cells[i - 1]);
        }
        if (z > 0)
        {
            if ((z & 1) == 0)
            {
                cell.SetNeightbor(HexDirection.SE, cells[i - width]);
                if (x > 0)
                {
                    cell.SetNeightbor(HexDirection.SW, cells[i - width - 1]);
                }
            }
            else
            {
                cell.SetNeightbor(HexDirection.SW, cells[i - width]);
                if (x < width - 1)
                {
                    cell.SetNeightbor(HexDirection.SE, cells[i - width + 1]);
                }
            }
        }
        //Text label = Instantiate<Text>(cellLabelPrefebs, gridCanvas.transform, false);
        //label.rectTransform.anchoredPosition = new Vector2(pos.x, pos.z);
        //label.text = cell.coordinates.ToStringOnSeparateLines();
    }
}
