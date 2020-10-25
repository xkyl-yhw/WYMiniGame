using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassCell : MonoBehaviour
{
    public GameObject grassPrefeb;
    public int minNum;
    public int maxNum;
    public float radius;
    public List<GameObject> grassList = new List<GameObject>();

    public Material[] grassMat;
    public TeamTag tempTeam;
    public TeamTag prevTeam;

    private void Start()
    {
        radius = HexMetrics.outRadius;
        int InsNum = Random.Range(minNum, maxNum);
        float k = HexMetrics.innerRadius * 1.732f / (HexMetrics.innerRadius - HexMetrics.outRadius * 2);
        for (int i = 0; i < InsNum; i++)
        {
            float x = Random.Range(-radius, radius);
            float y = 0;

            if (Mathf.Abs(x) < HexMetrics.innerRadius / 2)
            {
                y = HexMetrics.innerRadius;
            }
            else if (x > HexMetrics.innerRadius / 2)
            {
                y = k * x - k * HexMetrics.outRadius;
            }
            else if (x < HexMetrics.innerRadius / 2)
            {
                y = -k * x - k * HexMetrics.outRadius;
            }
            y = Random.Range(-y, y);

            GameObject go = GameObject.Instantiate(grassPrefeb, transform);
            go.transform.GetChild(0).GetComponent<MeshRenderer>().material = grassMat[(int)tempTeam];
            go.transform.localPosition = new Vector3(x, 0, y);
            grassList.Add(go);
            prevTeam = tempTeam;
        }
    }

    private void Update()
    {
        if (prevTeam != tempTeam)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetChild(0).GetComponent<MeshRenderer>().material = grassMat[(int)tempTeam];
            }
            prevTeam = tempTeam;
        }

    }
}
