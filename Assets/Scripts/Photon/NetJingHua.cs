using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetJingHua : MonoBehaviourPun,IPunObservable
{
    // Start is called before the first frame update
    //名称
    public string itemName;
    //图标
    public Sprite icon = null;
    //多个同类物品持有的数量
    public int itemCnt;
    public bool isDefaultItem = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
