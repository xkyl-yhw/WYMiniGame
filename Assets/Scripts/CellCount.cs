using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellCount : MonoBehaviour
{
    List<HexCell> CountCells=new List<HexCell>();
    HexGrid hexGrid;
    //public Image[] barList;
    Dictionary<TeamTag, int> TeamAreaCount = new Dictionary<TeamTag, int>();
    public bool add = true;
    public float winRate = 0.4f;

    public Sprite[] playerArr;
    public Image[] playerImg;
    public GameObject win;

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
        if (redRate > winRate)
        {
            win.SetActive(true);
            playerImg[0].sprite = playerArr[0];
            playerImg[1].sprite = (greenRate > blueRate) ? playerArr[2] : playerArr[1];
            playerImg[2].sprite = (greenRate > blueRate) ? playerArr[1] : playerArr[2];

        }else if (greenRate > winRate)
        {
            win.SetActive(true);
            playerImg[0].sprite = playerArr[2];
            playerImg[1].sprite = (redRate > blueRate) ? playerArr[0] : playerArr[1];
            playerImg[2].sprite = (redRate > blueRate) ? playerArr[1] : playerArr[0];
        }
        else if (blueRate > winRate)
        {
            win.SetActive(true);
            playerImg[0].sprite = playerArr[1];
            playerImg[1].sprite = (greenRate > redRate) ? playerArr[2] : playerArr[0];
            playerImg[2].sprite = (greenRate > redRate) ? playerArr[0] : playerArr[2];
        }
        //Debug.Log(TeamAreaCount[TeamTag.red] + " " + TeamAreaCount[TeamTag.green] + " " + TeamAreaCount[TeamTag.blue]);
        //barList[0].fillAmount = redRate + blueRate + greenRate;
        //barList[1].fillAmount = greenRate + blueRate;
        //barList[2].fillAmount = greenRate;
    }
}
