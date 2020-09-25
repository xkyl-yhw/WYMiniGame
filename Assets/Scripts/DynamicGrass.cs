using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicGrass : MonoBehaviour
{
    public float effectRadius = 2f;
    public Material GrassMat;
    public GameObject grassModel;

    public float grassAreaWidth = 8f;
    public float grassAreaLength = 6f;
    public float grassDensity = 5;
    public float grassSize = 1;
    public float grassHeight = 1;

    public float windAngle = 0;
    public float windStrength = 1;
    private Vector2 windDir = new Vector2(0, 0);

    public Transform[] obstacles;
    private Vector4[] obstaclePositions = new Vector4[100];

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private float prevGrassAreaWidth;
    private float prevGrassAreaLength;
    private float prevGrassSize;
    private float prevGrassHeight;
    private GameObject prevGrassModel;

    private void Start()
    {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();

        meshRenderer.material = GrassMat;

        InitializeGrass();

        prevGrassAreaLength = grassAreaLength;
        prevGrassAreaWidth = grassAreaWidth;
        prevGrassSize = grassSize;
        prevGrassHeight = grassHeight;
        prevGrassModel = grassModel;
    }

    private void Update()
    {
        // send data to grass shader
        for (int n = 0; n < obstacles.Length; n++)
        {
            obstaclePositions[n] = obstacles[n].position;
        }

        Shader.SetGlobalFloat("_PositionArray", obstacles.Length);
        Shader.SetGlobalVectorArray("_ObstaclePositions", obstaclePositions);

        Shader.SetGlobalVector("_ObstaclePosition", obstaclePositions[0]); // for H5

        Shader.SetGlobalFloat("_EffectRadius", effectRadius);

        windDir = GetWindDir(windAngle);

        Shader.SetGlobalFloat("_WindDirectionX", windDir.x);
        Shader.SetGlobalFloat("_WindDirectionZ", windDir.y);
        Shader.SetGlobalFloat("_WindStrength", windStrength);

        float shakeBending = Mathf.Lerp(0.5f, 2f, windStrength);
        Shader.SetGlobalFloat("_ShakeBending", shakeBending);

        // check grass size change
        if (prevGrassAreaWidth != grassAreaWidth || prevGrassAreaLength != grassAreaLength
            || prevGrassSize != grassSize || prevGrassHeight != grassHeight || prevGrassModel != grassModel)
        {
            InitializeGrass();

            prevGrassAreaWidth = grassAreaWidth;
            prevGrassAreaLength = grassAreaLength;
            prevGrassSize = grassSize;
            prevGrassHeight = grassHeight;
            prevGrassModel = grassModel;
        }
    }

    void InitializeGrass()
    {
        int tempGrassNumW = Mathf.FloorToInt(grassAreaWidth * grassDensity);
        int tempGrassNumL = Mathf.FloorToInt(grassAreaLength * grassDensity);
        float tempPosInterval = 1f / grassDensity;
        float tempPosStartX = grassAreaWidth * -0.5f;
        float tempPosStartZ = grassAreaLength * -0.5f;
        Vector3[] tempGrassPosList = new Vector3[tempGrassNumW * tempGrassNumL];
        GameObject[] tempGrassObjList = new GameObject[tempGrassNumW * tempGrassNumL];
        CombineInstance[] combine = new CombineInstance[tempGrassNumW * tempGrassNumL];
        float tempPosOffset = tempPosInterval * 0.3f;

        for (int i = 0; i < tempGrassNumW; i++)
        {
            for (int j = 0; j < tempGrassNumL; j++)
            {
                tempGrassPosList[i + j * tempGrassNumW] = new Vector3(tempPosStartX + i * tempPosInterval, transform.position.y, tempPosStartZ + j * tempPosInterval) + new Vector3(Random.Range(-tempPosOffset, tempPosOffset), 0, Random.Range(-tempPosOffset, tempPosOffset));
                tempGrassObjList[i + j * tempGrassNumW] = Instantiate(grassModel, tempGrassPosList[i + j * tempGrassNumW], Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)));
                tempGrassObjList[i + j * tempGrassNumW].transform.localScale = new Vector3(grassSize, Random.Range(0.7f, 1.3f) * grassHeight, grassSize);
                MeshFilter mf = tempGrassObjList[i + j * tempGrassNumW].GetComponent<MeshFilter>();
                combine[i + j * tempGrassNumW].mesh = mf.mesh;
                combine[i + j * tempGrassNumW].transform = tempGrassObjList[i + j * tempGrassNumW].transform.localToWorldMatrix;
                tempGrassObjList[i + j * tempGrassNumW].SetActive(false);
            }
        }
        meshFilter.mesh = new Mesh();
        meshFilter.mesh.CombineMeshes(combine);
        transform.gameObject.active = true;
        for (int i = 0; i < tempGrassPosList.Length; i++)
        {
            Destroy(tempGrassObjList[i]);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.2f);
        Gizmos.DrawCube(transform.position, new Vector3(grassAreaWidth, 0.2f, grassAreaLength));
    }

    Vector2 GetWindDir(float degree)
    {
        Vector2 dir = new Vector2(0, 0);
        float radian = degree * Mathf.Deg2Rad;
        dir = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        return dir.normalized;
    }
}
