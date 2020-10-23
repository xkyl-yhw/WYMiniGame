﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using Photon.Pun;

public class NetWeaponController : MonoBehaviourPun, IPunObservable
{

    public GameObject weapon;
    [SerializeField]
    private int weaponIndex;
    [SerializeField]
    private string weaponType; //Gun,Machete,Bomb
    //public int gunNum = 10;
    //public int macheteNum = 10;
    //public int bombNum = 10;
    [SerializeField]
    public float SwitchWeaponCD = 1f;
    [SerializeField]
    public float SwitchWeaponCD2 = 0.5f;
    [SerializeField]
    private float mSwitchWeapon;
    [SerializeField]
    private float mSwitchWeapon2;
    [SerializeField]
    private float AxisCounts;
    [SerializeField]
    private bool isScroll;
    [SerializeField]
    private bool isScrollCD;

    public Vector3 weaponPosition = new Vector3(1.8f, 0f, 1.0f);

    public List<GameObject> mList = new List<GameObject>(); //武器列表


    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected)//如果观察不是当前角色以及网络连接上
        {
            return;
        }
        mSwitchWeapon = 0;
        mSwitchWeapon2 = 0;
        AxisCounts = 0;
        isScroll = false;
        isScrollCD = false;

        weaponType = "Gun";
        weaponIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected)//如果观察不是当前角色以及网络连接上
        {
            return;
        }

        float MouseScrollWheel = Input.GetAxis("Mouse ScrollWheel"); // 滚轮角度
        if (Input.GetAxis("Mouse ScrollWheel") != 0 && !isScroll && !isScrollCD)
        {
            isScroll = true;
        }
        if (isScroll)
        {
            MouseScrollWheelAxisCounts(MouseScrollWheel);
            if (AxisCounts > 0.3 || AxisCounts < -0.3)
            {
                isScroll = false;
                isScrollCD = true;
                Destroy(weapon);
                SwitchWeapon(AxisCounts);
            }
        }
        if (isScrollCD)
        {
            mSwitchWeapon2 += Time.deltaTime;
            if (mSwitchWeapon2 > SwitchWeaponCD2)
            {
                isScrollCD = false;
                mSwitchWeapon2 = 0;
            }
        }

        //当炸弹投出去的时候切换武器
        if (weaponType == "Bomb")
        {
            BombObject bombObject = weapon.GetComponent<BombObject>();
            if (bombObject.isSwitchWeapon)
            {
                SwitchWeapon(1);
            }
        }
    }


    // 滚轮控制武器切换
    private void SwitchWeapon(float MouseScrollWheel)
    {
        AxisCounts = 0;

        if (MouseScrollWheel > 0)
            NextWeapon();
        if (MouseScrollWheel < 0)
            PreviousWeapon();

        //weapon = Instantiate(mList[weaponIndex]);
       
        weaponType = GetWeaponType(weaponIndex);

        weapon = PhotonNetwork.Instantiate(mList[weaponIndex].name, this.transform.position+ weaponPosition, Quaternion.Euler(0f, 0f, 0f), 0);

        weapon.transform.parent = this.transform;
        //weapon.transform.localPosition = weaponPosition;
        //weapon.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    public void NextWeapon()//下一个武器
    {
        weaponIndex++;
        if (weaponIndex > mList.Count - 1)
        {
            weaponIndex = 0;
        }
    }

    public void PreviousWeapon()//上一个武器
    {
        weaponIndex--;
        if (weaponIndex < 0)
        {
            weaponIndex = mList.Count - 1;
        }
    }

    // 计算每次开始滚滚轮，一秒内鼠标滚轮的角度
    private void MouseScrollWheelAxisCounts(float MouseScrollWheel)
    {
        if (mSwitchWeapon > SwitchWeaponCD)
        {
            mSwitchWeapon = 0;
            AxisCounts = 0;
            isScroll = false;
        }
        else
        {
            mSwitchWeapon += Time.deltaTime;
            AxisCounts += MouseScrollWheel;
        }
    }

    private string GetWeaponType(int i)
    {
        // 获取GameObject名称
        string name = mList[i].name;
        Regex re = new Regex(@"[a-zA-Z]+");
        Match m = re.Match(name);
        string fixtype = m.Value;

        Debug.Log(fixtype);
        return fixtype;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            //stream.SendNext(weapon);
            stream.SendNext(weaponIndex);
            stream.SendNext(weaponType);
            stream.SendNext(SwitchWeaponCD);
            stream.SendNext(SwitchWeaponCD2);
            stream.SendNext(mSwitchWeapon);
            stream.SendNext(mSwitchWeapon2);
            stream.SendNext(AxisCounts);
            stream.SendNext(isScroll);
            stream.SendNext(isScrollCD);
        }
        else
        {
            // Network player, receive data
            //weapon=(GameObject)stream.ReceiveNext();
            weaponIndex = (int)stream.ReceiveNext();
            weaponType = (string)stream.ReceiveNext();
            SwitchWeaponCD = (float)stream.ReceiveNext();
            SwitchWeaponCD2 = (float)stream.ReceiveNext();
            mSwitchWeapon= (float)stream.ReceiveNext();
            mSwitchWeapon2 = (float)stream.ReceiveNext();
            AxisCounts = (float)stream.ReceiveNext();
            isScroll = (bool)stream.ReceiveNext();
            isScrollCD = (bool)stream.ReceiveNext();

        }
    }
}