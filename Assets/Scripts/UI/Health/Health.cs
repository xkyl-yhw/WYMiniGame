using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public Image img;
    public Text hpText;
    public float healthMax;
    public  static float currentHealth;
    public GameObject player;
    public PlayerAttribute playerAttribute;

    void Start()
    {
        //healthMax = 100;
        currentHealth = healthMax;
        playerAttribute = player.GetComponent<PlayerAttribute>();
        img = GetComponent<Image>();    //获取Image组件
        hpText = GetComponentInChildren<Text>();
    }


    void Update()
    {
        healthMax = playerAttribute.healthMax;
        currentHealth = playerAttribute.health;
        img.fillAmount = currentHealth / healthMax;
        hpText.text =currentHealth.ToString()+ "/"+ healthMax.ToString();
    }

}
