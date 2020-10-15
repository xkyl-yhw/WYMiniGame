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
    public float upEssence;
    public float currentEssence;
    void Start()
    {
        playerAttribute = player.GetComponent<PlayerAttribute>();
        img = GetComponentInChildren<Image>();    //获取Image组件
        essenceText = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        currentEssence =playerAttribute.essencePickNum;
        img.fillAmount = currentEssence / upEssence;
        essenceText.text = currentEssence.ToString() + "/" + upEssence.ToString();

    }
}
