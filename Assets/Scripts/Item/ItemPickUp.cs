using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemPickUp : MonoBehaviourPun
{
    public Item item;
    public NetJingHua netJingHua;
    public NetPlayerAttribute playerAttribute;

    //public GameObject player;

    void Start()
    {
        netJingHua = GetComponent<NetJingHua>();
    }
    void Update()
    {
        netJingHua = GetComponent<NetJingHua>();
    }
    private void OnTriggerEnter(Collider other)
    {
        //匹配玩家tag,不能搞isMine
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log(netJingHua.name);
            playerAttribute = other.GetComponent<NetPlayerAttribute>();
            //数量增加
            playerAttribute.essencePickNum += netJingHua.itemCnt;
            //被捡物品消失
            PhotonNetwork.Destroy(gameObject);
            }
        }

   
}