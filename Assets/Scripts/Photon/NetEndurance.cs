using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;

public class NetEndurance : MonoBehaviourPun
{
    public Image img;
    // Use this for initialization
    //public Text enText;
    public int enduranceMax;
    public GameObject player;
    public NetPlayerAttribute playerAttribute;
    public float currentEndurance;
    void Start()
    {
        //enduranceUp = 100;
        //currentEndurance = playerAttribute.enduranceMax;
        playerAttribute = player.GetComponent<NetPlayerAttribute>();
        img = GetComponentInChildren<Image>();    //获取Image组件
        //enText = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        enduranceMax = playerAttribute.enduranceMax;
        currentEndurance = Convert.ToInt64(playerAttribute.endurance);
        img.fillAmount = currentEndurance / enduranceMax;
        //enText.text = currentEndurance.ToString() + "/" + enduranceMax.ToString(); ;

    }
}
