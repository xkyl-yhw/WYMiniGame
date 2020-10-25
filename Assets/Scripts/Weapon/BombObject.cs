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

    public TeamSetup playerTeam;

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
                GameObject.Destroy(gameObject);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<HexGrid>() != null)
        {
            HexCell temp = other.GetComponentInParent<HexGrid>().GetCell(transform.position);
            for (HexDirection i = HexDirection.NE; i < HexDirection.NW; i++)
            {
                other.GetComponentInParent<HexGrid>().InfectCell(temp.GetNeighbor(i).transform.position, playerTeam.teamColor);
            }
            other.GetComponentInParent<HexGrid>().InfectCell(temp.transform.position, playerTeam.teamColor);
        }
    }
}
