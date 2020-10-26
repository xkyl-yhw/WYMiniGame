using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NetworkGameManager : NetworkBehaviour
{
    Dictionary<TeamTag, List<GameObject>> teamDict = new Dictionary<TeamTag, List<GameObject>>();
    public TeamTag needTeam;

    public Toggle[] array;

    public int colorIndex;

    public override void OnStartServer()
    {
        base.OnStartServer();
        for (TeamTag i = 0; i <= TeamTag.blue; i++)
        {
            teamDict.Add(i, new List<GameObject>());
        }
    }

    private void Update()
    {
        needTeam = TeamTag.red + colorIndex;
    }

    public TeamTag addDict(GameObject player)
    {
        teamDict[needTeam].Add(player);
        return needTeam;
    }

    public void chengeToggleValue()
    {
        int index = 0;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>())
            {
                index = i;
                break;
            }
        }
        if (array[index].isOn)
            for (int i = 0; i < array.Length; i++)
            {
                if (i == index)
                {
                    colorIndex = index;
                    continue;
                }
                array[i].isOn = false;
            }
    }
}
