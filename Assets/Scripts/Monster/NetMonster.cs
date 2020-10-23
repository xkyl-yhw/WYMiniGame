using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class NetMonster : MonoBehaviourPun, IPunObservable
{
    public int health;
    public int damage;
    public bool inRecoverySphere;
    private NetPlayerHealth playerHealth;
    public GameObject objectMachine;//所属复苏机器体
    public NetRecoveryMachine attachedMachine;//所属复苏机器

    public GameObject dropSingleEssence;  //掉落精华

    public GameObject dropFiftyEssence;

    public GameObject dropFiveEssence;

    public GameObject dropTwentyEssence;
    [SerializeField]
    public int dropNum; //掉落数量

    public Transform dropRange;//掉落范围
    [SerializeField]
    public float timer;
    //public Animator m_animator;

    public float radius;//测试离玩家半径
    [SerializeField]
    int fiftyNum;
    [SerializeField]
    int twentyNum ;
    [SerializeField]
    int fiveNum;
    [SerializeField]
    int singleNum;
    [SerializeField]
    int remain;

    public void Start()
    {
        if (dropRange == null)
        {
            dropRange = transform;
        }
        fiftyNum = dropNum / 50;
        remain = dropNum % 50;
        twentyNum = remain / 20;
        remain = remain % 20;
        fiveNum = remain / 5;
        remain = remain % 5;
        singleNum = remain / 1;
        //m_animator = GetComponent<Animator>();
        //playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<NetPlayerHealth>();

    }

    // Update is called once per frame
    public void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 2)
        {
            health = 0;
        }
            if (health <= 0)
        {
            Drop(dropNum);
            //m_animator.SetTrigger("die");
            PhotonNetwork.Destroy(gameObject);
        }
        objectMachine = GameObject.FindWithTag("RecoveryMachine");
        attachedMachine = objectMachine.GetComponent<NetRecoveryMachine>();
        if (attachedMachine.canRecovery)
        {
            health = 0;
        }

    }
    //精华掉落
    void Drop(int dropNum)
    {
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
            //Instantiate(essenceType, dropRange.TransformPoint(pos2), Quaternion.identity);
            PhotonNetwork.Instantiate(essenceType.name, dropRange.TransformPoint(pos2), Quaternion.identity, 0);
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
        ////碰撞并且类型是胶囊体碰撞，因为player可能存在多个Collider
        //if (other.gameObject.CompareTag("Player") && other.GetType().ToString() == "UnityEngine.CapsuleCollider")
        //{
        //    playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<NetPlayerHealth>();
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
                NetBulletObject bulletObject = other.gameObject.GetComponent<NetBulletObject>();
                //Debug.Log(playerHealth.health);
                this.TakeDamage((int)Mathf.Floor(bulletObject.damage));
                GameObject.Destroy(other.gameObject);
            }
        }
        if (other.gameObject.CompareTag("Machete"))
        {
            if (this.health > 0)
            {
                NetMachetesObject machetesObject = other.gameObject.GetComponent<NetMachetesObject>();
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
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health);
            stream.SendNext(timer);
            stream.SendNext(fiftyNum);
            stream.SendNext(twentyNum);
            stream.SendNext(fiveNum);
            stream.SendNext(singleNum);
            stream.SendNext(damage);
            stream.SendNext(dropNum);
            //stream.SendNext(dropRange);

        }
        else
        {
            health = (int)stream.ReceiveNext();
            timer= (float)stream.ReceiveNext();
            fiftyNum = (int)stream.ReceiveNext();
            twentyNum = (int)stream.ReceiveNext();
            fiveNum = (int)stream.ReceiveNext();
            singleNum = (int)stream.ReceiveNext();
            damage = (int)stream.ReceiveNext();
            dropNum = (int)stream.ReceiveNext();
            //dropRange =(transform)stream.ReceiveNext();


        }
    }

}
