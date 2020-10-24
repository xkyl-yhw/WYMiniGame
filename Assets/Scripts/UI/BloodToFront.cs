using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BloodToFront : MonoBehaviour
{
    public PlayerAttribute playerAttribute;
    public GameObject player;
    public Image img;
    public float currentHealth;
    public float healthMax;
    void Start()
    {
        currentHealth = healthMax;
        playerAttribute = player.GetComponent<PlayerAttribute>();
        //img = GetComponent<Image>();    //获取Image组件
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
        currentHealth = playerAttribute.health;
        img.fillAmount = currentHealth / healthMax;
    }
}
