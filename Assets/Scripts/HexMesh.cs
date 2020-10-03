using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
    Mesh hexMesh;
    List<Vector3> vertices;
    List<int> triangles;
    MeshCollider meshCollider;

    List<Color> colors;

    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        meshCollider = gameObject.AddComponent<MeshCollider>();
        hexMesh.name = "HexMesh";
        vertices = new List<Vector3>();
        colors = new List<Color>();
        triangles = new List<int>();
    }

    public void Triangulate(HexCell[] cells)
    {
        hexMesh.Clear();
        vertices.Clear();
        triangles.Clear();
        colors.Clear();
        for (int i = 0; i < cells.Length; i++)
        {
            Triangulate(cells[i]);
        }
        hexMesh.vertices = vertices.ToArray();
        hexMesh.colors = colors.ToArray();
        hexMesh.triangles = triangles.ToArray();
        hexMesh.RecalculateNormals();
        meshCollider.sharedMesh = hexMesh;
    }

    void addTriangleColor(Color c1, Color c2, Color c3)
    {
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
    }

    void addTriangleColor(Color color)
    {
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }

    void Triangulate(HexCell cell)
    {
        /*
        Vector3 center = cell.transform.localPosition;
        for (int i = 0; i < 6; i++)
        {
            AddTriangle(center, center + HexMetrics.corners[i], center + HexMetrics.corners[i + 1]);
            addTriangleColor(cell.color);
        }*/
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            Triangulate(d, cell);
        }
    }

    void Triangulate(HexDirection direction, HexCell cell)
    {
        Vector3 center = cell.transform.localPosition;
        Vector3 v1 = center + HexMetrics.GetFirstSolidCorner(direction);
        Vector3 v2 = center + HexMetrics.GetSecondSolidCorner(direction);
        AddTriangle(center, v1, v2);
        addTriangleColor(cell.color);
        if (direction <= HexDirection.SE)
            TriangularConnection(direction, cell, v1, v2);
        //Vector3 bridge = HexMetrics.GetBridge(direction);
        //Vector3 v3 = v1 + bridge;
        //Vector3 v4 = v2 + bridge;
        //AddQuad(v1, v2, v3, v4);
        //HexCell prevNeighbor = cell.GetNeighbor(direction.Previous()) ?? cell;
        //HexCell neighbor = cell.GetNeighbor(direction) ?? cell;
        //HexCell nextNeighbor = cell.GetNeighbor(direction.Next()) ?? cell;
        //Color bridgeColor = (cell.color + neighbor.color) * 0.5f;
        //AddQuadColor(cell.color, bridgeColor);

        //AddTriangle(v1, center + HexMetrics.GetFirstCorner(direction), v3);
        //addTriangleColor(cell.color, (cell.color + prevNeighbor.color + neighbor.color) / 3f, bridgeColor);
        //AddTriangle(v2, v4, center + HexMetrics.GetSecondCorner(direction));
        //addTriangleColor(cell.color, bridgeColor, (cell.color + neighbor.color + nextNeighbor.color) / 3f);
    }

    void TriangularConnection(HexDirection direction, HexCell cell, Vector3 v1, Vector3 v2)
    {
        HexCell neighbor = cell.GetNeighbor(direction);
        if (neighbor == null) return;
        Vector3 bridge = HexMetrics.GetBridge(direction);
        Vector3 v3 = v1 + bridge;
        Vector3 v4 = v2 + bridge;
        AddQuad(v1, v2, v3, v4);
        AddQuadColor(cell.color, neighbor.color);
        HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
        if (nextNeighbor != null && direction <= HexDirection.E)
        {
            AddTriangle(v2, v4, v2 + HexMetrics.GetBridge(direction.Next()));
            addTriangleColor(cell.color, neighbor.color, nextNeighbor.color);
        }
    }

    void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

    void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        vertices.Add(v4);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);
    }

    void AddQuadColor(Color c1, Color c2)
    {
        colors.Add(c1);
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c2);
    }
}
