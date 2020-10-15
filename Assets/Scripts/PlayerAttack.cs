using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int damage;
    public float startTime;
    public float time;

    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Attack()
    {
        if (Input.GetButtonDown("Attack"))
        {
            //共计动画
            anim.SetTrigger("Attack");
            StartCoroutine(StartAttack());
        }
    }
    IEnumerator StartAttack()
    {
        yield return new WaitForSeconds(startTime);
        StartCoroutine(disableHitBox());
    }
    //攻击范围
    IEnumerator disableHitBox()
    {
        yield return new WaitForSeconds(time);
    }
    //对怪物伤害
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Monster"))
        {
            other.GetComponent<Monster>().TakeDamage(damage);
        }
    }
}
