using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NetworkGameManager : NetworkBehaviour
{
    //Dictionary<TeamTag, List<GameObject>> teamDict = new Dictionary<TeamTag, List<GameObject>>();
    [SyncVar]
    public TeamTag needTeam;

    public Toggle[] array;

    public int colorIndex;

    public override void OnStartServer()
    {
        base.OnStartServer();
        needTeam = TeamTag.red;
    }

    public void addTeam()
    {
        Debug.Log(TeamSetup.returnColor(needTeam));
        if (needTeam == TeamTag.blue) needTeam = TeamTag.red;
        else needTeam++;
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
