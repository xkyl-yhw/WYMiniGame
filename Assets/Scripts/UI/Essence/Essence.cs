using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Essence : MonoBehaviour
{
    public Image img;
    // Use this for initialization
    public Text essenceText;
    public GameObject player;
    public PlayerAttribute playerAttribute;
    public float essenceMax;
    public float currentEssence;
    void Start()
    {
        playerAttribute = player.GetComponent<PlayerAttribute>();
        img = GetComponent<Image>();    //获取Image组件
        essenceText = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        essenceMax = playerAttribute.essenceMax;
        currentEssence =playerAttribute.essencePickNum;
        img.fillAmount = currentEssence / essenceMax;
        essenceText.text = currentEssence.ToString() + "/" + essenceMax.ToString();

    }

}
