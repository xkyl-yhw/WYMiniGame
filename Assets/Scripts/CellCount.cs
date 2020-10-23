using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellCount : MonoBehaviour
{
    List<HexCell> CountCells=new List<HexCell>();
    HexGrid hexGrid;
    public Image[] barList;
    Dictionary<TeamTag, int> TeamAreaCount = new Dictionary<TeamTag, int>();
    public bool add = true;

    private void Start()
    {
        hexGrid = GetComponent<HexGrid>();
    }

    private void Update()
    {
        if (add)
        {
            add = false;
            addCells();
        }
        AreaCountMethod();
    }

    public void addCells()
    {
        foreach (var item in hexGrid.cells)
        {
            CountCells.Add(item);
        }
    }

    public void AreaCountMethod()
    {
        if (CountCells.Count == 0) return;
        TeamTag temp = TeamTag.red;
        for (int i = 0; i < 3; i++)
        {
            TeamAreaCount[temp++] = 0;
        }
        for (int i = 0; i < CountCells.Count; i++)
        {
            if (CountCells[i].Color != hexGrid.defaultColor)
                TeamAreaCount[TeamSetup.returnTeam(CountCells[i].Color)] += 1;
        }
        float redRate = TeamAreaCount[TeamTag.red] / (float)CountCells.Count;
        float greenRate = TeamAreaCount[TeamTag.green] / (float)CountCells.Count;
        float blueRate = TeamAreaCount[TeamTag.blue] / (float)CountCells.Count;
        //Debug.Log(TeamAreaCount[TeamTag.red] + " " + TeamAreaCount[TeamTag.green] + " " + TeamAreaCount[TeamTag.blue]);
        barList[0].fillAmount = redRate + blueRate + greenRate;
        barList[1].fillAmount = greenRate + blueRate;
        barList[2].fillAmount = greenRate;
    }
}
