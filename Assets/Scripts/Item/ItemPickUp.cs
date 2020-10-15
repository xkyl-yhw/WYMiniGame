using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public Item item;

    public PlayerAttribute playerAttribute;

    //public GameObject player;

    void Start()
    {
  
    }
    private void OnTriggerEnter(Collider other)
    {
        //匹配玩家tag
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log(item.name);
            playerAttribute = other.GetComponent<PlayerAttribute>();
            //数量增加
            playerAttribute.essencePickNum += item.itemCnt;
                //被捡物品消失
            Destroy(gameObject);
            }
        }

   
}