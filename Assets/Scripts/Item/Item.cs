using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="New Item",menuName ="Inventory/Item")]
public class Item : ScriptableObject
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

}
