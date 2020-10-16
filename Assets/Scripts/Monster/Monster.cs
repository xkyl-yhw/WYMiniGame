using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  abstract class Monster : MonoBehaviour
{
    public int health;
    public int damage;
    private PlayerHealth playerHealth;
    //掉落精华
    public GameObject dropEssence;

    public GameObject dropfiftyEssence;
    //掉落数量
    public int dropNum;
    //掉落范围
    public Transform dropRange;

    public void Start()
    {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    public void Update()
    {
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
        for (int i = 0; i < fiftyNum; i++)
        {
            Vector2 p = Random.insideUnitCircle * 2;
            Vector2 pos = p.normalized * (1 + p.magnitude);
            Vector3 pos2 = new Vector3(pos.x, 0, pos.y);
            Instantiate(dropfiftyEssence, dropRange.TransformPoint(pos2), Quaternion.identity);
        }
        for (int i = 0; i < singleNum; i++)
        {
            Vector2 p = Random.insideUnitCircle * 2;
            Vector2 pos = p.normalized * (1 + p.magnitude);
            Vector3 pos2 = new Vector3(pos.x, 0, pos.y);
            Instantiate(dropEssence, dropRange.TransformPoint(pos2), Quaternion.identity);
        }
    }
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
        if (other.gameObject.CompareTag("Bullet"))
        {
            if (this.health > 0)
            {
                BulletObject bulletObject = other.gameObject.GetComponent<BulletObject>();
                //Debug.Log(playerHealth.health);
                this.TakeDamage((int)Mathf.Floor(bulletObject.damage));
                GameObject.Destroy(other.gameObject);
            }
        }
    }
}
