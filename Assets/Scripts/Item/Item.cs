﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
[CreateAssetMenu(fileName ="New Item",menuName ="Inventory/Item")]
public class Item : ScriptableObject,IPunObservable
{
    //名称
     public string itemName;
    //图标
    public Sprite icon = null;
    //多个同类物品持有的数量
    public int itemCnt;
    public bool isDefaultItem = false;

    //物品描述
    [TextArea]
    public string itemInfo;

    void Start()
    {
        //itemCnt = 0;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(itemCnt);
            stream.SendNext(isDefaultItem);
        }
        else
        {
            // Network player, receive data
            itemCnt = (int)stream.ReceiveNext();
            isDefaultItem = (bool)stream.ReceiveNext();
        }
    }
}
