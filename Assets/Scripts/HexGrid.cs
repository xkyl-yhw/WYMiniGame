﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class HexGrid : MonoBehaviour
{
    public HexGridChunk chunkPrefebs;
    public int chunkCountX = 4, chunkCountZ = 3;
    [HideInInspector]
    public int cellCountX, cellCountZ;
    public HexCell cellPrefeb;

    public HexCell[] cells;

    public Color defaultColor = Color.white;
    public Color touchedColor = Color.magenta;

    int activeElevation;

    HexGridChunk[] chunks;

    private string ScenesName;

    public GameObject grassCellPrefebs;
    public Transform grassCellTran;

    public Dictionary<int, GameObject> grassCastDict = new Dictionary<int, GameObject>();

    private void Awake()
    {
        ScenesName = SceneManager.GetActiveScene().name;
        cellCountX = chunkCountX * HexMetrics.chunkSizeX;
        cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;
        CreateChunks();
        LoadFile();
    }

    void LoadFile()
    {
        if (File.Exists(Application.dataPath + "/" + ScenesName + ".txt"))
        {
            StreamReader sr = new StreamReader(Application.dataPath + "/" + ScenesName + ".txt");
            string temp = sr.ReadToEnd();
            HexCellMsgArray tempMeg = JsonUtility.FromJson<HexCellMsgArray>(temp);
            cells = new HexCell[cellCountX * cellCountZ];
            for (int i = 0; i < cellCountX * cellCountZ; i++)
            {
                CreateCell(tempMeg.cellArray[i].x, tempMeg.cellArray[i].y, i, tempMeg.cellArray[i].color, tempMeg.cellArray[i].Elevation);
            }
        }
        else CreateCells();
    }

    public void SaveFile()
    {
        HexCellMsgArray tempCell = CreateCellMsg();
        string jsonStr = JsonUtility.ToJson(tempCell);
        StreamWriter sw = new StreamWriter(Application.dataPath + "/" + ScenesName + ".txt");
        sw.Write(jsonStr);
        sw.Close();
        //Debug.Log(1);
    }

    HexCellMsgArray CreateCellMsg()
    {
        HexCellMsgArray tempCell = new HexCellMsgArray()
        {
            cellArray = new List<HexCellMsg>()
        };
        for (int i = 0; i < cellCountX * cellCountZ; i++)
        {
            HexCellMsg temp = new HexCellMsg();
            temp.x = cells[i].x;
            temp.y = cells[i].z;
            temp.color = cells[i].Color;
            temp.Elevation = cells[i].Elevation;
            tempCell.cellArray.Add(temp);
        }
        return tempCell;
    }


    void CreateChunks()
    {
        chunks = new HexGridChunk[chunkCountX * chunkCountZ];
        for (int z = 0, i = 0; z < chunkCountZ; z++)
        {
            for (int x = 0; x < chunkCountX; x++)
            {
                HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefebs);
                chunk.transform.SetParent(transform);
            }
        }
    }

    void CreateCells()
    {
        cells = new HexCell[cellCountX * cellCountZ];
        for (int z = 0, i = 0; z < cellCountZ; z++)
        {
            for (int x = 0; x < cellCountX; x++)
            {
                CreateCell(x, z, i++, defaultColor, 0);
            }
        }
    }

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
        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
        HexCell cell = cells[index];
        //Debug.Log(1);
        if (cell.Color != defaultColor && cell.Color == color) return;
        cell.Color = color;
        //hexMesh.Triangulate(cells);
        if (color != defaultColor)
        {
            //if (cell.Color != color) grassCastDict.Remove(index);
            ////Debug.Log(1);
            //CmdCreateGrass(index, pos,color);
            if (cell.Color != color)
            {
                grassCastDict[index].GetComponent<GrassCell>().tempTeam = TeamSetup.returnTeam(color);
                return;
            }
            Debug.Log(1);
            CmdCreateGrass(index, pos, color);
        }
    }
    private void CmdCreateGrass(int index, Vector3 pos, Color temp)
    {
        GameObject go = Instantiate(grassCellPrefebs, pos, Quaternion.identity, grassCellTran);
        go.GetComponent<GrassCell>().tempTeam = TeamSetup.returnTeam(temp);
        if (grassCastDict.ContainsKey(index)) return;
        grassCastDict.Add(index, go);
    }

    void CreateCell(int x, int z, int i, Color color, int elevation)
    {
        Vector3 pos = new Vector3((x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f), 0, z * (HexMetrics.outRadius * 1.5f));
        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefeb);
        cell.transform.localPosition = pos;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.Color = color;
        cell.x = x;
        cell.z = z;
        if (x > 0)
        {
            cell.SetNeightbor(HexDirection.W, cells[i - 1]);
        }
        if (z > 0)
        {
            if ((z & 1) == 0)
            {
                cell.SetNeightbor(HexDirection.SE, cells[i - cellCountX]);
                if (x > 0)
                {
                    cell.SetNeightbor(HexDirection.SW, cells[i - cellCountX - 1]);
                }
            }
            else
            {
                cell.SetNeightbor(HexDirection.SW, cells[i - cellCountX]);
                if (x < cellCountX - 1)
                {
                    cell.SetNeightbor(HexDirection.SE, cells[i - cellCountX + 1]);
                }
            }
        }
        //Text label = Instantiate<Text>(cellLabelPrefebs, gridCanvas.transform, false);
        //label.rectTransform.anchoredPosition = new Vector2(pos.x, pos.z);
        //label.text = cell.coordinates.ToStringOnSeparateLines();
        cell.Elevation = elevation;
        AddCellToChunk(x, z, cell);
        HexCellMsg tempMsg = new HexCellMsg();
        tempMsg.x = x;
        tempMsg.y = z;
        tempMsg.color = color;
        tempMsg.Elevation = elevation;
    }

    void AddCellToChunk(int x, int z, HexCell cell)
    {
        int chunkX = x / HexMetrics.chunkSizeX;
        int chunkZ = z / HexMetrics.chunkSizeZ;
        HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];
        int localX = x - chunkX * HexMetrics.chunkSizeX;
        int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
        chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
    }

    public HexCell GetCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPos(position);
        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
        return cells[index];
    }
}
