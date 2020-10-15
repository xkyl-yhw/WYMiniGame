using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Endurance : MonoBehaviour
{
    public Image img;
    // Use this for initialization
    public Text enText;
    public int enduranceMax;
    public GameObject player;
    public PlayerAttribute playerAttribute;
    public  float currentEndurance;
    void Start()
    {
        //enduranceUp = 100;
        currentEndurance = enduranceMax;
        playerAttribute = player.GetComponent<PlayerAttribute>();
        img = GetComponentInChildren<Image>();    //获取Image组件
        enText = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        enduranceMax = playerAttribute.enduranceMax;
       currentEndurance = Convert.ToInt64(playerAttribute.endurance);
        img.fillAmount = currentEndurance / enduranceMax;
        enText.text = currentEndurance.ToString() + "/" + enduranceMax.ToString(); ;

    }

}
