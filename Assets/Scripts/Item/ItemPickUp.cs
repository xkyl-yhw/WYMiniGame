using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemPickUp : MonoBehaviour
{
    public Item item;

    //public GameObject player;
    public PlayerAttribute playerAttribute;

    void Start()
    {
    }
    void Update()
    {
    }
    private void OnTriggerEnter(Collider other)
    {
        //匹配玩家tag,不能搞isMine
        if (other.gameObject.CompareTag("Player"))
        {
            playerAttribute = other.GetComponent<PlayerAttribute>();
            //数量增加
            playerAttribute.essencePickNum +=item.itemCnt;
            //被捡物品消失
            Destroy(gameObject);
            }
        }

   
}