using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletObject : MonoBehaviour
{
    private float speed = 50;
    private float ySpeed = 50;
    public float time = 0.5f;

    private float usefulDistance = 0;
    private WeaponController weaponController;
    private GameObject player;

    private Vector3 moveDirection = Vector3.zero;//角色移动

    public float damage;

    private Vector3 destination;
    private float distance = 0;
    private Vector3 hitInfoPoint;

    // Start is called before the first frame update
    void Start()
    {
        GetRayPosition();

        player = GameObject.FindGameObjectWithTag("Player");
        weaponController = player.GetComponent<WeaponController>();
        usefulDistance = weaponController.getStrikingDistance();
        damage = weaponController.getDamage();

        destination.y = this.transform.position.y;
        distance = Vector3.Distance(destination, this.transform.position);
        if (distance >= usefulDistance)
        {
            distance = usefulDistance;
        }
        speed = distance / time;
        ySpeed = Vector3.Distance(new Vector3(this.transform.position.x, hitInfoPoint.y, 
            this.transform.position.z), this.transform.position) / time;

    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        distance -= Time.deltaTime * speed;
        Debug.Log(distance);
        this.transform.position += (moveDirection * Time.fixedDeltaTime * speed) - new Vector3(0, ySpeed, 0) * Time.fixedDeltaTime;

        if (distance <= 0)
        {
            GameObject.Destroy(gameObject);
        }
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
            moveDirection = this.transform.forward;
            moveDirection.y = 0;

            hitInfoPoint = hitInfo.point;
            destination = hitInfoPoint;
            destination.y = 0;
        }
    }

}
