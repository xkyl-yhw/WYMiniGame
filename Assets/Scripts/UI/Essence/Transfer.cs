using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Transfer : MonoBehaviour
{
    public Transform player;
    public PlayerAttribute playerAttribute;
    public RecoveryMachine machine;
    public float radius; //复苏需要范围
    public float timer;
    public bool canRecovery;
    public int essenceTransferNum; //每秒传输的精华量
    public Toggle transferToggle;

    // Use this for initialization
    void Start()
    {
        transferToggle.enabled = false;
        canRecovery = machine.canRecovery;
        transferToggle.onValueChanged.AddListener(touchMusicSwitchButton);
    }
    // Update is called once per frame
    void Update()
    {
        canRecovery = machine.canRecovery;
        touchStartButton(transferToggle.isOn);
        if (canRecovery)
        {
            transferToggle.enabled = true;
        }
        else
        {
            transferToggle.enabled = false;
        }
    }

    public void touchStartButton(bool isOn)
    {
        if(isOn)
        {
            timer += Time.deltaTime;
            if (timer >= 1)
            {
                playerAttribute.essencePickNum -= essenceTransferNum;
                //精华为0时，传输按钮失效
                if (playerAttribute.essencePickNum == 0)
                {
                    transferToggle.isOn = false;
                    canRecovery = false;
                    playerAttribute.essencePickNum = 0;
                }
                timer = 0;
                Debug.Log("touch start Button YES");
            }
        }
    }

    public void touchMusicSwitchButton(bool isP)
    {

        Debug.Log("now " + transferToggle.isOn);
    }
}


