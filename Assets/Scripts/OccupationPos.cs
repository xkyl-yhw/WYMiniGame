using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public enum OccupyMode { Time, area };
public class OccupationPos : MonoBehaviour
{
    public OccupyMode occupyMode;
    public float[] teamNumAddRate;
    public float OccupyRate;
    public float[] teamOccupyPerson;

    public float radius;
    public int PointCount;

    private LineRenderer lineRenderer;
    private float angle;
    Dictionary<TeamTag, List<GameObject>> TeamCount = new Dictionary<TeamTag, List<GameObject>>();
    public Text[] teamTextList;
    public TeamTag precentOccTeam;
    private TeamTag lastOccTeam;
    public Slider OccupySlider;
    public GameObject SliderBack;
    public float prevTime;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        TeamCount.Add(TeamTag.blue, new List<GameObject>());
        TeamCount.Add(TeamTag.red, new List<GameObject>());
        TeamCount.Add(TeamTag.green, new List<GameObject>());
    }

    private void Update()
    {
        OccupyRate = Mathf.Clamp(OccupyRate, 0, 100);
        if (OccupyRate == 0) precentOccTeam = lastOccTeam;

        OccupySlider.value = OccupyRate / 100f;
        SliderBack.GetComponent<Image>().color = TeamSetup.returnColor(precentOccTeam);
        drawCircle();
        showNums();

        if ((Time.time - prevTime) > 1)
        {
            prevTime = Time.time;
            int InAreaTeam = 0;
            int TeamNums = 0;
            TeamTag InAreaTeamName = TeamTag.red;
            TeamTag tempTag = TeamTag.red;
            for (int i = 0; i < TeamCount.Count; i++)
            {
                if (TeamCount[tempTag].Count != 0)
                {
                    InAreaTeam++;
                    if (TeamCount[tempTag].Count > TeamNums) TeamNums = TeamCount[tempTag].Count;
                    InAreaTeamName = tempTag;
                }
                tempTag++;
            }
            if (InAreaTeam == 1)
            {
                lastOccTeam = InAreaTeamName;
                if (precentOccTeam != InAreaTeamName)
                {
                    OccupyRate -= teamNumAddRate[TeamNums];
                }
                else
                {
                    OccupyRate += teamNumAddRate[TeamNums];
                }
            }
        }
    }

    public void showNums()
    {
        GameObject[] PlayerArray = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < PlayerArray.Length; i++)
        {
            TeamTag tag = PlayerArray[i].GetComponent<TeamSetup>().teamTag;
            bool temp = !TeamCount[tag].Contains(PlayerArray[i]);
            if (Vector3.Distance(transform.position, PlayerArray[i].transform.position) < radius)
            {
                if (temp)
                {
                    TeamCount[tag].Add(PlayerArray[i]);
                }
            }
            else
            {
                if (!temp)
                {
                    TeamCount[tag].Remove(PlayerArray[i]);
                }
            }
        }
        TeamTag tempTag = TeamTag.red;
        for (int i = 0; i < teamTextList.Length; i++)
        {
            teamTextList[i].text = TeamCount[tempTag].Count.ToString();
            teamTextList[i].color = TeamSetup.returnColor(tempTag);
            tempTag++;
        }
    }

    private void drawCircle()
    {
        Vector3 dir = transform.forward * radius;
        lineRenderer.positionCount = PointCount + 1;
        angle = 360f / (PointCount - 1);
        for (int i = 0; i < PointCount; i++)
        {
            lineRenderer.SetPosition(i, dir + transform.position);
            dir = Quaternion.Euler(new Vector3(0, angle, 0)) * dir;
        }
        lineRenderer.SetPosition(PointCount, dir + transform.position);
    }

    public void delePlayer(GameObject temp)
    {
        TeamCount[temp.GetComponent<TeamSetup>().teamTag].Remove(temp);
        
    }
}
