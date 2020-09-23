using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MeshGrass : MonoBehaviour
{
    public int terrainSize;
    public Material terrainMat;
    public Material grassMat;
    public Mesh quad;
    public Texture2D highMap;
    [Range(1, 100)]
    public float scaleFatter = 10;
    [Range(1, 100)]
    public float offsetFatter = 10;
    [Range(0, 100)]
    public float terrainHeight = 10;
    public int grassCount = 300;

    private float grassDensity;
    private List<Matrix4x4[]> matrixList = new List<Matrix4x4[]>();

    private void Start()
    {
        CreateTerrian();
        MeshComputing();
    }

    private void Update()
    {
        RenderGrass();    
    }

    void CreateTerrian()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        for (int i = 0; i < terrainSize; i++)
        {
            for (int j = 0; j < terrainSize; j++)
            {
                vertices.Add(new Vector3(i, highMap.GetPixel(i, j).grayscale * terrainHeight, j));
                if (i == 0 || j == 0) continue;
                triangles.Add(terrainSize * i + j);
                triangles.Add(terrainSize * i + j - 1);
                triangles.Add(terrainSize * (i - 1) + j - 1);
                triangles.Add(terrainSize * (i - 1) + j - 1);
                triangles.Add(terrainSize * (i - 1) + j);
                triangles.Add(terrainSize * i + j);
            }
        }
        Vector2[] uvs = new Vector2[vertices.Count];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }
        GameObject myTerrain = this.gameObject;
        myTerrain.AddComponent<MeshFilter>();
        MeshRenderer renderer = myTerrain.AddComponent<MeshRenderer>();
        renderer.receiveShadows = true;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        Mesh groundMesh = new Mesh();
        groundMesh.vertices = vertices.ToArray();
        groundMesh.uv = uvs;
        groundMesh.triangles = triangles.ToArray();
        groundMesh.RecalculateNormals();
        myTerrain.GetComponent<MeshFilter>().mesh = groundMesh;
        MeshCollider collider = myTerrain.AddComponent<MeshCollider>();
        renderer.sharedMaterial = terrainMat;
        vertices.Clear();
    }

    void MeshComputing()
    {
        grassDensity = (float)terrainSize / grassCount;
        Matrix4x4[] matrix = new Matrix4x4[1023];
        Quaternion rotation = Quaternion.Euler(0, 0, 0);
        Vector3 scale = Vector3.one;
        int mm = 0;
        for (int i = 0; i < grassCount; i++)
        {
            for (int j = 0; j < grassCount; j++)
            {
                float ran = Random.Range(-0.19f, 0.2f);
                float ii = i * grassDensity;
                float jj = j * grassDensity;
                float x = ii + ran;
                float y = jj + ran;
                float h = highMap.GetPixel(Mathf.FloorToInt(ii), Mathf.FloorToInt(jj)).grayscale * terrainHeight + 0.5f;
                Vector3 pos = new Vector3(x, h, y) + transform.position;
                matrix[mm] = Matrix4x4.TRS(pos, rotation, scale);
                mm++;
                if (mm % 1022 == 0)
                {
                    matrixList.Add(new Matrix4x4[1023]);
                    matrixList[matrixList.Count - 1] = matrix;
                    matrix = new Matrix4x4[1023];
                    mm = 0;
                }
            }
        }
        matrixList.Add(matrix);
    }

    void RenderGrass()
    {
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        foreach(Matrix4x4[] mat in matrixList)
        {
            Graphics.DrawMeshInstanced(quad, 0, grassMat, mat, 1023, block, ShadowCastingMode.Off, false);
        }
    }

    float GeneratePerlinNoise(int i, int y)
    {
        float xCrood = (float)i / terrainSize * scaleFatter + offsetFatter;
        float yCrood = (float)y / terrainSize * scaleFatter + offsetFatter;
        return Mathf.PerlinNoise(xCrood, yCrood);
    }
}
