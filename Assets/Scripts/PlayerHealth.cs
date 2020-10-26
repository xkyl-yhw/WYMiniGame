using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    //public float health;
    //死亡多久消失
    public float dieTime = 1f;
    public bool isDie = false;

    // Start is called before the first frame update
    private Animator anim;

    public PlayerAttribute playerAttribute;

    //是否无敌
    public static bool isDefended = false;

    public AudioClip deathAudio; //倒地音效
    private AudioSource audioSource;
    private bool isDeathAudioPlay = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        playerAttribute = GetComponent<PlayerAttribute>();
        isDie = false;
        isDeathAudioPlay = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Health.currentHealth = playerAttribute.health;
        if (playerAttribute.health <= 0 && !isDie)
        {
            this.GetComponent<PlayController>().enabled = false;
            isDie = true;
            anim.SetTrigger("isDie");
            Invoke("KillPlayer", dieTime);

            if (!isDeathAudioPlay)
            {
                //爆炸声
                audioSource = GetComponent<AudioSource>();
                audioSource.clip = deathAudio;
                audioSource.Play();
                isDeathAudioPlay = true;
            }
        }
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
    }
    public void Defended()
    {
        //OnTriggerExit(Collider other);
    }

    void KillPlayer()
    {
        Debug.Log("人物死亡");
        //Destroy(gameObject);
        this.gameObject.SetActive(false);
        isDeathAudioPlay = false;
    }
}
