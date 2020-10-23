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
    public NetRecoveryMachine machine;//己方复苏机器
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
        transferToggle.enabled = false;
        canTransfer = machine.canTransfer;
        transferToggle.onValueChanged.AddListener(TouchButton);
    }

    void Update()
    {

        if (playerAttribute.essencePickNum <= 0)
        {
            transferToggle.isOn = false;
            canTransfer = false;
            playerAttribute.essencePickNum = 0;
        }
        else
        {
            canTransfer = machine.canTransfer;
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

    public void TouchTransferButton(bool isOn)
    {
        if (isOn)
        {
            timer += Time.deltaTime;
            if (timer >= 1)
            {

                if (machine.essenceRequired - machine.currentEssence >= essenceTransferNum)
                {
                    playerAttribute.essencePickNum -= essenceTransferNum;
                    machine.currentEssence += essenceTransferNum;
                }
                else
                {
                    playerAttribute.essencePickNum -= machine.essenceRequired - machine.currentEssence;
                    machine.currentEssence += machine.essenceRequired - machine.currentEssence;
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


