using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    //public float health;
    //死亡多久消失
    public float dieTime;

    // Start is called before the first frame update
    private Animator anim;

    public PlayerAttribute playerAttribute;

    //是否无敌
    public static bool isDefended = false;
    void Start()
    {
        anim = GetComponent<Animator>();
        playerAttribute = GetComponent<PlayerAttribute>();
    }

    // Update is called once per frame
    void Update()
    {
        //Health.currentHealth = playerAttribute.health;
    }
    
    public void DamagePlayer(int damage)
    {
        if(!isDefended)
        {
            playerAttribute.health -= damage;
        }
        if (playerAttribute.health < 0)
        {
            playerAttribute.health = 0;
        }
        if (playerAttribute.health <= 0)
        {
            anim.SetTrigger("Die");
            Invoke("KillPlayer", dieTime);
        }
    }
    public void Defended()
    {
        //OnTriggerExit(Collider other);
    }

    void KillPlayer()
    {
        Destroy(gameObject);
    }
}
