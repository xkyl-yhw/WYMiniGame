﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetRecoveryMachine : MonoBehaviourPun, IPunObservable
{
    [SerializeField]
    public int essenceRequired; //复苏需要的量
    [SerializeField]
    public int currentEssence; //现在的量
    public float transferRadius; //传输需要范围
    public float recoveryRadius; //复苏以及处死怪物范围
    [SerializeField]
    public bool canTransfer;//能够传输
    [SerializeField]
    public bool canRecovery;//能够复苏
    [SerializeField]
    public float timer;
    public float recoveryTimer;//复苏需要时间
    public bool isInSphere; //是否在范围
    public Transform player;//检测玩家
    public GameObject machine;//复苏机器本身位置
    public Monster[] monsters;//被杀死的怪物
    void Start()
    {
        machine = gameObject;

    }

    void Update()
    {
        //player = GameObject.FindWithTag("Player").transform;
        if (currentEssence >= essenceRequired)
        {
            canTransfer = false;
            timer += Time.deltaTime;
            if (timer >= recoveryTimer)
            {
                canRecovery = true;
                timer = 0;
            }
            currentEssence = 0;


        }
        canTransfer = true;
       //else
       //{
       //    cantransfer = cantransfer(player, machine.transform, transferradius);
       //}

       monsters = (Monster[])GameObject.FindObjectsOfType(typeof(Monster));

        for (int i = 0; i < monsters.Length; i++)
        {
            isInSphere = InRecoverySphere(monsters[i].transform, machine.transform, recoveryRadius);
            monsters[i].inRecoverySphere = isInSphere;
            if (isInSphere)
            {
                monsters[i].objectMachine = machine;
            }
            else
            {
                monsters[i].objectMachine = null;
            }
        }

    }


    public bool InRecoverySphere(Transform monster, Transform machine, float radius)
    {
        float distance = Vector3.Distance(monster.position, machine.position);
        if (distance <= radius)
        {
            return true;
        }
        return false;
    }
    //范围可视化
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(machine.transform.position, transferRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(machine.transform.position, recoveryRadius);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(essenceRequired);
            stream.SendNext(currentEssence);
            stream.SendNext(transferRadius);
            stream.SendNext(canTransfer);
            stream.SendNext(canRecovery);
            stream.SendNext(timer);
        }
        else
        {
            essenceRequired = (int)stream.ReceiveNext();
            currentEssence = (int)stream.ReceiveNext();
            transferRadius = (float)stream.ReceiveNext();
            canTransfer = (bool)stream.ReceiveNext();
            canRecovery = (bool)stream.ReceiveNext();
            timer = (float)stream.ReceiveNext();

        }
    }
}