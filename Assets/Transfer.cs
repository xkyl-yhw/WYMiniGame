using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Transfer : MonoBehaviour
{
    public GameObject player;
    public PlayerAttribute playerAttribute;
    public int essenceTransferNum; //每秒传输的精华量
    Toggle toggle;

    void Awake()
    {
        //添加监听
        GetComponent<Toggle>().onValueChanged.AddListener(OnValueChanged);
    }

    void Start()
    {
        //设置初始状态
        playerAttribute = player.GetComponent<PlayerAttribute>();
        OnValueChanged(GetComponent<Toggle>().isOn);
    }

    private void OnValueChanged(bool value)
    {
        while(value)
        {
            //选中了的逻辑
            Debug.Log("transfering");
            playerAttribute.essencePickNum -= 5;
        }
    }
}

