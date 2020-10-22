using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class NetTransfer : MonoBehaviourPun, IPunObservable
{
    public Transform player;
    public NetPlayerAttribute playerAttribute;
    public GameObject machine;//己方复苏机器
    public NetRecoveryMachine netRecoveryMachine;
    public float transferRadius; //传输需要范围
    [SerializeField]
    public float timer;
    [SerializeField]
    public bool canTransfer;
    [SerializeField]
    public int essenceTransferNum; //每秒传输的精华量
    public Toggle transferToggle;


    void Start()
    {
        //需要加一个出生点机器的初始化
        machine = GameObject.FindGameObjectWithTag("RecoveryMachine");
        netRecoveryMachine = machine.GetComponent<NetRecoveryMachine>();
        transferToggle.enabled = false;
        canTransfer = netRecoveryMachine.canTransfer;
        transferToggle.onValueChanged.AddListener(TouchButton);
    }

    void Update()
    {
        Debug.Log(transferToggle.enabled);
        transferRadius = netRecoveryMachine.transferRadius;

        if (playerAttribute.essencePickNum <= 0)
        {
            transferToggle.isOn = false;
            canTransfer = false;
            playerAttribute.essencePickNum = 0;
        }
        else
        {
            canTransfer = CanTransfer( player.transform,  machine.transform, transferRadius) && netRecoveryMachine.canTransfer;
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

        TouchTransferButton(transferToggle.isOn);

    }


    //判断玩家能够进行传输的范围
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
        if (isOn)
        {
            timer += Time.deltaTime;
            if (timer >= 1)
            {

                if (netRecoveryMachine.essenceRequired - netRecoveryMachine.currentEssence >= essenceTransferNum)
                {
                    playerAttribute.essencePickNum -= essenceTransferNum;
                    netRecoveryMachine.currentEssence += essenceTransferNum;
                }
                else
                {
                    playerAttribute.essencePickNum -= netRecoveryMachine.essenceRequired - netRecoveryMachine.currentEssence;
                    netRecoveryMachine.currentEssence += netRecoveryMachine.essenceRequired - netRecoveryMachine.currentEssence;
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
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(canTransfer);
            stream.SendNext(essenceTransferNum);
            stream.SendNext(timer);
        }
        else
        {
            canTransfer = (bool)stream.ReceiveNext();
            essenceTransferNum = (int)stream.ReceiveNext();
            timer = (float)stream.ReceiveNext();

        }
    }
}


