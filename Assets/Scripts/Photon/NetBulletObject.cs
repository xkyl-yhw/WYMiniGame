using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class NetBulletObject : MonoBehaviourPun,IPunObservable
{
    public float speed = 50;

    private float usefulDistance = 0;
    private NetWeaponController weaponController;
    private GameObject player;

    private Vector3 moveDirection = Vector3.zero;//角色移动
    //private CharacterController controller;

    public float damage;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        weaponController = player.GetComponent<NetWeaponController>();
        usefulDistance = weaponController.GetStrikingDistance();
        damage = weaponController.GetDamage();
        Debug.Log(damage);

        //controller = GetComponent<CharacterController>();

        moveDirection = weaponController.transform.forward;// forward 指向物体当前的前方
        moveDirection.y = 0f;// 只做平面的上下移动和水平移动，不做高度上的上下移动
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected)//如果观察不是当前角色以及网络连接上
        {
            return;
        }
        usefulDistance -= Time.deltaTime * speed;
        //controller.Move(moveDirection * Time.deltaTime * speed);
        this.transform.position += (moveDirection * Time.deltaTime * speed);

        if (usefulDistance <= 0)
        {
            //GameObject.Destroy(gameObject);
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(moveDirection);
        }
        else
        {
            // Network player, receive data
            moveDirection = (Vector3)stream.ReceiveNext();
        }
    }
}
