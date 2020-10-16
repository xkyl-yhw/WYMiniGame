using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  abstract class Monster : MonoBehaviour
{
    public int health;
    public int damage;
    public bool inRecoverySphere;
    private PlayerHealth playerHealth;
    public GameObject objectMachine;//所属复苏机器体
    public RecoveryMachine attachedMachine;//所属复苏机器
  
    public GameObject dropSingleEssence;  //掉落精华

    public GameObject dropFiftyEssence;

    public GameObject dropFiveEssence;

    public GameObject dropTwentyEssence;
   
    public int dropNum; //掉落数量
 
    public Transform dropRange;//掉落范围

    public void Start()
    {
        if (dropRange==null)
        {
            dropRange = transform;
        }
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        
    }

    // Update is called once per frame
    public void Update()
    {
        attachedMachine = objectMachine.GetComponent<RecoveryMachine>();
        if (attachedMachine.canRecovery)
        {
            health = 0;
        }
        if (health <= 0)
        {
            Drop(dropNum);
            Destroy(gameObject);
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
        if (other.gameObject.CompareTag("Player")&&other.GetType().ToString()== "UnityEngine.CapsuleCollider")
        {
            if (playerHealth!=null)
            {
                //Debug.Log(playerHealth.health);
                playerHealth.DamagePlayer(damage);
            }

        }
    }
}
