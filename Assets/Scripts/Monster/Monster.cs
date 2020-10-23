using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Monster : MonoBehaviour
{
    public int health;
    public int damage;
    public bool inRecoverySphere;
    private PlayerHealth playerHealth;
    public GameObject objectMachine;//所属复苏机器体
    public NetRecoveryMachine attachedMachine;//所属复苏机器

    public GameObject dropSingleEssence;  //掉落精华

    public GameObject dropFiftyEssence;

    public GameObject dropFiveEssence;

    public GameObject dropTwentyEssence;

    public int dropNum; //掉落数量

    public Transform dropRange;//掉落范围

    public Animator m_animator;

    public float radius;//测试离玩家半径

    public void Start()
    {
        if (dropRange == null)
        {
            dropRange = transform;
        }
        m_animator = GetComponent<Animator>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();

    }

    // Update is called once per frame
    public void Update()
    {
        if (health <= 0)
        {
            Drop(dropNum);
            m_animator.SetTrigger("die");
            //Destroy(gameObject);
        }
        attachedMachine = objectMachine.GetComponent<NetRecoveryMachine>();
        if (attachedMachine.canRecovery)
        {
            health = 0;
        }

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
            Instantiate(essenceType, dropRange.TransformPoint(pos2), Quaternion.identity);
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
        if (other.gameObject.CompareTag("Player") && other.GetType().ToString() == "UnityEngine.CapsuleCollider")
        {
            if (playerHealth != null)
            {
                //Debug.Log(playerHealth.health);
                playerHealth.DamagePlayer(damage);
            }

        }
        if (other.gameObject.CompareTag("Bullet") || other.gameObject.CompareTag("Machete"))
        {
            if (this.health > 0)
            {
                BulletObject bulletObject = other.gameObject.GetComponent<BulletObject>();
                //Debug.Log(playerHealth.health);
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

}
