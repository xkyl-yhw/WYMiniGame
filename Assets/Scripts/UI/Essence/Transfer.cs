using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Transfer : MonoBehaviour
{
    public Transform player;
    public PlayerAttribute playerAttribute;
    public GameObject occupyMachine;//己方复苏机器
    public RecoveryMachine recoveryMachine;
    public float timer;
    public bool canTransfer;
    public float transferRadius; //传输需要范围
    public int essenceTransferNum; //每秒传输的精华量
    public Toggle transferToggle;


    void Start()
    {
        transferToggle.enabled = false;
        canTransfer = false;
        recoveryMachine = occupyMachine.GetComponent<RecoveryMachine>();

        transferRadius = recoveryMachine.transferRadius;
        transferToggle.onValueChanged.AddListener(TouchButton);
    }

    void Update()
    {
        Debug.Log(transferToggle.enabled);
        //recoveryMachine = occupyMachine.GetComponent<RecoveryMachine>();
        transferRadius = recoveryMachine.transferRadius;
        if (playerAttribute.essencePickNum <= 0)
        {
            transferToggle.isOn = false;
            canTransfer = false;
            playerAttribute.essencePickNum = 0;
        }
        else
        {
            canTransfer = CanTransfer(player.transform, occupyMachine.transform, transferRadius) && recoveryMachine.canTransfer;
        }
        if (canTransfer)
        {
            transferToggle.enabled = true;
        }
        else
        {
            transferToggle.isOn = false;
            transferToggle.enabled = false;
        }
        transferToggle.targetGraphic.color = new Color(255, 255, 255, 255);
        TouchTransferButton(transferToggle.isOn);

    }
    public bool CanTransfer(Transform player, Transform machine, float radius)
    {
        float distance = Vector3.Distance(player.position, machine.position);
        if (distance <= radius)
        {
            return true;
        }
        return false;
    }

    public void TouchTransferButton(bool isOn)
    {
        if(isOn)
        {
            transferToggle.targetGraphic.color= new Color(255, 255, 255, 0);
            timer += Time.deltaTime;
            if (timer >= 1)
            {
                
                if(recoveryMachine.essenceRequired- recoveryMachine.currentEssence>=essenceTransferNum)
                {
                    playerAttribute.essencePickNum -= essenceTransferNum;
                    recoveryMachine.currentEssence += essenceTransferNum;
                }
                else
                {
                    playerAttribute.essencePickNum -= recoveryMachine.essenceRequired - recoveryMachine.currentEssence;
                    recoveryMachine.currentEssence += recoveryMachine.essenceRequired - recoveryMachine.currentEssence;
                }
                //精华为0时或者复苏机器精华量已满，传输按钮失效
                timer = 0;
                Debug.Log("touch Transfer Button YES");
            }
        }
    }

    public void TouchButton(bool isP)
    {

        Debug.Log("now " + transferToggle.isOn);
    }
}


