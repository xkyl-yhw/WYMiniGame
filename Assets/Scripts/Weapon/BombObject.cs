using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombObject : MonoBehaviour
{
    public string weaponName; //名字
    public int damage; //伤害值
    //public bool isRanged = false; //是否是远程，是=远程，否=近战
    public int strikingDistance; //攻击距离
    private bool canShoot = true; //是否可以开火
    public bool isShoot = true; //是否正在开火
    public bool isBasy = false; //鼠标左键是否用来点击道具了；是=用了；否=没用，可以开抢
    public int bombRadius = 5; //爆炸范围
    public float deathTime = 0.8f;//爆炸延迟


    private Vector3 destination;
    private float distance = 0;
    private Vector3 forceDirection = Vector3.zero;//施加力的方向
    
    private float force;
    public float time = 0.5f;
    public float exitTime = 5.0f;
    public float angle = 60;
    public float angle2 = 500;
    public float switchWeaponTime = 0.2f;
    public bool isSwitchWeapon = false;

    // Start is called before the first frame update
    void Start()
    {
        isShoot = false;
        canShoot = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (canShoot && !isShoot && Input.GetKeyDown(KeyCode.Mouse0))
        {
            isShoot = true;

            transform.transform.parent = null;

            this.GetComponent<Rigidbody>().useGravity = true;

            GetRayPosition();

            distance = Vector3.Distance(destination, this.transform.position);
            if (distance >= strikingDistance)
            {
                distance = strikingDistance;
                destination = strikingDistance * this.transform.forward;
            }
            force = distance / time;
            //this.transform.Rotate(this.transform.right, angle);
            forceDirection = /*Quaternion.Euler(new Vector3(-angle, 0, 0)) **/ this.transform.forward * force * 0.1f;
            GetComponent<Rigidbody>().velocity = forceDirection;
            GetComponent<Rigidbody>().AddForce(new Vector3(0, 1, 0) * angle2);
        }

        if (isShoot)
        {
            //distance -= Time.deltaTime * speed;
            //this.transform.position += (moveDirection * Time.deltaTime * speed);
            exitTime -= Time.deltaTime;
            switchWeaponTime -= Time.deltaTime;
            if (switchWeaponTime < 0)
            {
                isSwitchWeapon = true;
            }

            if (/*Vector3.Distance(destination, this.transform.position)*/ exitTime <= 0)
            {
                isShoot = false;
                canShoot = false;
                this.GetComponent<Rigidbody>().constraints = ~RigidbodyConstraints.FreezePositionY;
                //GameObject.Destroy(gameObject);
                CheckDamage();
                Rough();
                StartCoroutine(Countdown());//deathTime时间后删除物体
            }
        }
    }

    void FixedUpdate()
    {

    }

    private void GetRayPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;//存储射线信息
        int groundLayerIndex = LayerMask.GetMask("Ground"); //初始化地面layer的序列
        if (Physics.Raycast(ray, out hitInfo, 200, groundLayerIndex))//生成射线
        {
            Vector3 playerToMouse = hitInfo.point - transform.position;
            playerToMouse.y = 0;
            Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
            this.transform.rotation = newRotation;

            destination = hitInfo.point;
            destination.y = 0;
        }
    }

    public void CheckDamage()
    {
        Vector3 a = this.transform.position;
        foreach (Collider b in Physics.OverlapSphere(a, bombRadius))
        {
            if (b.gameObject.tag == "Player" && b.gameObject != this.gameObject) // 对玩家造成伤害
            {
                PlayerHealth playerHealth = b.gameObject.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    Debug.Log("爆炸正在对玩家造成伤害");
                    playerHealth.DamagePlayer(this.damage);
                }
            }
            else if (b.gameObject.tag == "Monster") // 对怪物造成伤害
            {
                if (b.gameObject.GetComponent<Monster>().health > 0)
                {
                    Debug.Log("爆炸正在对怪物造成伤害");
                    b.gameObject.GetComponent<Monster>().TakeDamage(this.damage);
                }
            }
        }

    }

    IEnumerator Countdown()
    {
        for (float timer = deathTime; timer >= 0; timer -= Time.deltaTime)
            yield return 0;
        Debug.Log("This message appears after " + deathTime + " seconds!");
        Destroy(gameObject);
    }

    void Rough()
    {
        //请在这里加上长草需要的代码
    }
}
