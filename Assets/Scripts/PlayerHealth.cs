using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public int ReStartTime = 10;
    public Vector3 spawnPos;
    public GameObject CountDown;

    void Start()
    {
        anim = GetComponent<Animator>();
        playerAttribute = GetComponent<PlayerAttribute>();
        isDie = false;
        isDeathAudioPlay = false;
        spawnPos = transform.position;
        CountDown = GameObject.Find("CountDown");
        CountDown.SetActive(false);
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
        //this.gameObject.SetActive(false);
        isDeathAudioPlay = false;
        StartCoroutine(player_ReStart());
    }

    private IEnumerator player_ReStart()
    {
        int temp = ReStartTime;
        CountDown.SetActive(true);
        while (temp > 0)
        {
            CountDown.GetComponentInChildren<Text>().text = temp.ToString();
            yield return new WaitForSeconds(1);
            temp--;
        }
        CountDown.SetActive(false);
        transform.position = spawnPos;
        playerAttribute.health = 100;
        this.GetComponent<PlayController>().enabled = true;
        isDie = false;
        anim.SetTrigger("isReborn");
    }
}
