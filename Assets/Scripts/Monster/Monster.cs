using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Monster : MonoBehaviour
{
    public int health;
    public int damage;
    public bool inRecoverySphere;
    private PlayerHealth playerHealth;
    //public GameObject objectMachine;//所属复苏机器体
    //public RecoveryMachine attachedMachine;//所属复苏机器

    public GameObject dropSingleEssence;  //掉落精华

    public GameObject dropFiftyEssence;

    public GameObject dropFiveEssence;

    public GameObject dropTwentyEssence;

    public int dropNum; //掉落数量

    public Transform dropRange;//掉落范围

    public Animator m_animator;

    public float radius;//测试离玩家半径

    private bool isDead = false;
    public float deathTime = 3f;

    public GameObject player;

    public void Start()
    {
        if (dropRange == null)
        {
            dropRange = transform;
        }
        m_animator = GetComponent<Animator>();

        isDead = false;
    }

    // Update is called once per frame
    public void Update()
    {
        if (health <= 0 && !isDead)
        {
            Drop(dropNum);
            m_animator.SetTrigger("die");
            isDead = true;
            
            StartCoroutine(Countdown());//deathTime时间后删除物体
        }
        //机器复苏功能
        //attachedMachine = objectMachine.GetComponent<RecoveryMachine>();
        //if (attachedMachine.canRecovery)
        //{
        //    health = 0;
        //}
        getPlayer();// 获取地图上离自己最近的player
        playerHealth = player.GetComponent<PlayerHealth>();

    }
    //精华掉落
    void Drop(int dropNum)
    {
        int fiftyNum = 0;
        int twentyNum = 0;
        int fiveNum = 0;
        int singleNum = 0;
        int remain = 0;
        fiftyNum = dropNum / 50;
        remain = dropNum % 50;
        twentyNum = remain / 20;
        remain = remain % 20;
        fiveNum = remain / 5;
        remain = remain % 5;
        singleNum = remain / 1;
        Debug.Log("DROP" + fiftyNum.ToString() + " " + twentyNum.ToString() + " " + fiveNum.ToString() + " " + singleNum.ToString() + "ESSENCE");
        GenerateEssence(fiftyNum, dropFiftyEssence);
        GenerateEssence(twentyNum, dropTwentyEssence);
        GenerateEssence(fiveNum, dropFiveEssence);
        GenerateEssence(singleNum, dropSingleEssence);
    }
    //精华生成
    void GenerateEssence(int generateNum, GameObject essenceType)
    {
        for (int i = 0; i < generateNum; i++)
        {
            Vector2 p = Random.insideUnitCircle * 0.5f;
            Vector2 pos = p.normalized * (0.5f + p.magnitude);
            Vector3 pos2 = new Vector3(pos.x, 0, pos.y);
            GameObject a = Instantiate(essenceType, dropRange.TransformPoint(pos2), Quaternion.identity);
            a.transform.position = a.transform.position + new Vector3(0, 0.5f, 0);
        }
    }

    //怪物受伤
    public void TakeDamage(int damage)
    {
        health -= damage;

    }


    //触发造成伤害
    public void OnTriggerEnter(Collider other)
    {
        //碰撞并且类型是胶囊体碰撞，因为player可能存在多个Collider
        //if (other.gameObject.CompareTag("Player") && other.GetType().ToString() == "UnityEngine.CapsuleCollider")
        //{
        //    if (playerHealth != null)
        //    {
        //        //Debug.Log(playerHealth.health);
        //        playerHealth.DamagePlayer(damage);
        //    }

        //}
        if (other.gameObject.CompareTag("Bullet") || other.gameObject.CompareTag("Machete"))
        {
            if (this.health > 0)
            {
                BulletObject bulletObject = other.gameObject.GetComponent<BulletObject>();
                Debug.Log(this.health);
                this.TakeDamage((int)Mathf.Floor(bulletObject.damage));
                GameObject.Destroy(other.gameObject);
            }
        }
        if (other.gameObject.CompareTag("Machete"))
        {
            if (this.health > 0)
            {
                MachetesObject machetesObject = other.gameObject.GetComponent<MachetesObject>();
                if (machetesObject.isDamage)
                {
                    this.TakeDamage((int)Mathf.Floor(machetesObject.damage));
                    machetesObject.isDamage = false;
                }
            }
        }
    }
    //范围可视化
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);

    }
    
    IEnumerator Countdown()
    {
        for (float timer = deathTime; timer >= 0; timer -= Time.deltaTime)
            yield return 0;
        Debug.Log("This message appears after " + deathTime + " seconds!");
        Destroy(gameObject);
    }

    void getPlayer()
    {
        GameObject[] playerArray;
        Vector3 playerPosition = new Vector3(0, 0, 0);
        float nearest = 0;

        playerArray = GameObject.FindGameObjectsWithTag("Player");

        for (int i=0;i<playerArray.Length;i++)
        {
            float dis = (transform.position - playerArray[i].transform.position).sqrMagnitude;
            if(i==0)
            {
                nearest = dis;
                player = playerArray[i];
            }
            else
            {
                if(nearest>dis)
                {
                    nearest = dis;
                    player = playerArray[i];
                }
            }
        }
    }

    // 对玩家造成伤害
    public void hitPlayer(GameObject mplayer)
    {
        playerHealth = mplayer.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            Debug.Log("正在造成伤害" + damage);
            playerHealth.DamagePlayer(damage);
            if(playerHealth.playerAttribute.health<=0)
            {
                this.GetComponent<MonsterWander>().hasPlayerDie = true;
            }
        }
    }

}
