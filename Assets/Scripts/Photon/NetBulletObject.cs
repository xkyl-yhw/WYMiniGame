using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class NetBulletObject : MonoBehaviourPun,IPunObservable
{
    [SerializeField]
    private float speed = 50;
    [SerializeField]
    private float ySpeed = 50;
    public float time = 0.5f;
    [SerializeField]
    private float usefulDistance = 0;
    private NetWeaponController weaponController;
    private GameObject player;
    [SerializeField]
    private Vector3 moveDirection = Vector3.zero;//角色移动
    [SerializeField]
    public float damage;
    [SerializeField]
    private Vector3 destination;
    [SerializeField]
    private float distance = 0;
    [SerializeField]
    private Vector3 hitInfoPoint;

    // Start is called before the first frame update
    void Start()
    {
        GetRayPosition();

        player = GameObject.FindGameObjectWithTag("Player");
        weaponController = player.GetComponent<NetWeaponController>();
        usefulDistance = weaponController.weapon.GetComponent<NetWeaponObject>().strikingDistance;
        damage = weaponController.weapon.GetComponent<NetWeaponObject>().damage;

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
        this.transform.position += (moveDirection * Time.fixedDeltaTime * speed) - new Vector3(0, ySpeed, 0) * Time.fixedDeltaTime;

        if (distance <= 0)
        {
            Destroy(gameObject);
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
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(speed);
            stream.SendNext(ySpeed);
            stream.SendNext(usefulDistance);
            stream.SendNext(damage);
            stream.SendNext(moveDirection);
            stream.SendNext(destination);
            stream.SendNext(distance);
            stream.SendNext(hitInfoPoint);

        }
        else
        {
            // Network player, receive data
            speed = (float)stream.ReceiveNext();
            ySpeed = (float)stream.ReceiveNext();
            usefulDistance = (float)stream.ReceiveNext();
            damage = (float)stream.ReceiveNext();
            moveDirection = (Vector3)stream.ReceiveNext();
            destination = (Vector3)stream.ReceiveNext();
            distance= (float)stream.ReceiveNext();
            hitInfoPoint = (Vector3)stream.ReceiveNext();

        }
    }
}
