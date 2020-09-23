using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGrass : MonoBehaviour
{
    public static GameObject Obj = null;
    public bool change = false;
    [Range(0, 1000)]
    public int terrainSize = 250;
    public Material terrainMat;
    [Range(0, 100)]
    public float terrainHeight = 10;

    [Range(1, 100)]
    public float scaleFatter = 10;
    [Range(1, 100)]
    public float offsetFatter = 10;
    List<Vector3> vertexs = new List<Vector3>();
    List<int> triangles = new List<int>();
    float[,] perlinNoise;

    private float xOffset;
    private float zOffset;

    [Range(0, 100)]
    public int grassRowCount = 50;
    [Range(1, 1000)]
    public int grassCountPerPatch = 100;
    public Material grassMat;
    public Mesh grassMesh;
    List<Vector3> grassVerts = new List<Vector3>();

    private void Start()
    {
        xOffset = transform.position.x;
        zOffset = transform.position.z; 
        GenerateTerrain();
        GenerateGrassArea(grassRowCount, grassCountPerPatch);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) change = true;
        if (change)
        {
            change = false;
            GenerateTerrain();
            GenerateGrassArea(grassRowCount, grassCountPerPatch);
        }
    }

    void CreateVertAndTris()
    {
        for (int i = 0; i < terrainSize; i++)
        {
            for (int j = 0; j < terrainSize; j++)
            {
                float noiseHeight = GeneratePerlinNoise(i, j);
                perlinNoise[i, j] = noiseHeight;
                vertexs.Add(new Vector3(i, noiseHeight * terrainHeight, j));
                if (i == 0 || j == 0)
                    continue;
                triangles.Add(terrainSize * i + j);
                triangles.Add(terrainSize * i + j - 1);
                triangles.Add(terrainSize * (i - 1) + j - 1);
                triangles.Add(terrainSize * (i - 1) + j - 1);
                triangles.Add(terrainSize * (i - 1) + j);
                triangles.Add(terrainSize * i + j);
            }
        }
    }

    float GeneratePerlinNoise(int i, int y)
    {
        float xCrood = (float)(i + xOffset) / terrainSize * scaleFatter + offsetFatter;
        float yCrood = (float)(y + zOffset) / terrainSize * scaleFatter + offsetFatter;
        return Mathf.PerlinNoise(xCrood, yCrood);
    }

    void GenerateTerrain()
    {
        if (Obj != null) Destroy(Obj);
        perlinNoise = new float[terrainSize, terrainSize];
        triangles.Clear();
        vertexs.Clear();
        CreateVertAndTris();
        Vector2[] uvs = new Vector2[vertexs.Count];
        for (int i = 0; i < vertexs.Count; i++)
        {
            uvs[i] = new Vector2(vertexs[i].x, vertexs[i].z);
        }
        GameObject Myterrain = new GameObject("Terrain");
        Myterrain.AddComponent<MeshFilter>();
        MeshRenderer renderer = Myterrain.AddComponent<MeshRenderer>();
        renderer.receiveShadows = true;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        MeshCollider collider = Myterrain.AddComponent<MeshCollider>();
        renderer.sharedMaterial = terrainMat;

        Mesh groundMesh = new Mesh();
        groundMesh.vertices = vertexs.ToArray();
        groundMesh.triangles = triangles.ToArray();
        groundMesh.RecalculateNormals();
        Myterrain.GetComponent<MeshFilter>().mesh = groundMesh;
        collider.sharedMesh = groundMesh;
        Obj = Myterrain;
        grassVerts.Clear();
    }

    void GenerateGrassArea(int rowCount, int perPatchSize)
    {
        List<int> indices = new List<int>();
        for (int i = 0; i < 65000; i++)
        {
            indices.Add(i);
        }

        Vector3 currentPos = transform.position;
        Vector3 patchSize = new Vector3(terrainSize / rowCount, 0, terrainSize / rowCount);
        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < rowCount; j++)
            {
                GenerateGrass(currentPos, patchSize, grassCountPerPatch);
                currentPos.x += patchSize.x;
            }
            currentPos.x = transform.position.x;
            currentPos.z = patchSize.z;
        }
        GameObject grassLayGroup1 = new GameObject("GrassLayerGroup1");
        GameObject grassLayer;
        MeshFilter grassMeshFilter;
        MeshRenderer grassMeshRenderer;
        int a = 0;
        while (grassVerts.Count>65000)
        {
            grassMesh = new Mesh();
            grassMesh.vertices = grassVerts.GetRange(0, 65000).ToArray();
            grassMesh.SetIndices(indices.ToArray(), MeshTopology.Points, 0);

            grassLayer = new GameObject("GrassLayer " + a++);
            grassLayer.transform.SetParent(grassLayGroup1.transform);
            grassMeshFilter = grassLayer.AddComponent<MeshFilter>();
            grassMeshRenderer = grassLayer.AddComponent<MeshRenderer>();
            grassMeshRenderer.receiveShadows = false;
            grassMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            grassMeshRenderer.sharedMaterial = grassMat;
            grassMeshFilter.mesh = grassMesh;
            grassVerts.RemoveRange(0, 65000);
        }
        grassLayer = new GameObject("GrassLayer " + a);
        grassLayer.transform.SetParent(grassLayGroup1.transform);
        grassMeshFilter = grassLayer.AddComponent<MeshFilter>();
        grassMeshRenderer = grassLayer.AddComponent<MeshRenderer>();
        grassMeshRenderer.receiveShadows = false;
        grassMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        grassMesh = new Mesh();
        grassMesh.vertices = grassVerts.ToArray();
        grassMesh.SetIndices(indices.GetRange(0, grassVerts.Count).ToArray(), MeshTopology.Points, 0);
        grassMeshRenderer.sharedMaterial = grassMat;
        grassMeshFilter.mesh = grassMesh;
    }

    void GenerateGrass(Vector3 vertPos,Vector3 patchSize,int grassCountPerPatch)
    {
        for (int i = 0; i < grassCountPerPatch; i++)
        {
            float randomX = Random.value * patchSize.x;
            float randomZ = Random.value * patchSize.z;
            int indexX = (int)((vertPos.x - transform.position.x) + randomX);
            int indexZ = (int)((vertPos.z - transform.position.z) + randomZ);

            if (indexX >= terrainSize)
            {
                indexX = (int)terrainSize - 1;
            }
            if (indexZ >= terrainSize)
            {
                indexZ = (int)terrainSize - 1;
            }
            grassVerts.Add(new Vector3(vertPos.x + randomX, perlinNoise[indexX, indexZ] * terrainHeight, vertPos.z + randomZ));
        }
    }
}
