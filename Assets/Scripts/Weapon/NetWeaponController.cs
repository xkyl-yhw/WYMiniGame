using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetWeaponController : MonoBehaviourPun
{
    public GameObject weapon;
    private NetWeaponObject weaponObject;

    private int weaponIndex;
    private string weaponType;
    //public int weaponNum = 10;
    public int gunNum = 10;
    public int macheteNum = 10;

    public float SwitchWeaponCD = 1f;
    public float SwitchWeaponCD2 = 0.5f;
    private float mSwitchWeapon;
    private float mSwitchWeapon2;
    private float AxisCounts;
    private bool isScroll;
    private bool isScrollCD;

    public Vector3 weaponPosition = new Vector3(0.7f, 0f, 0.5f);

    //public GameObject bulletPrefab;

    // Start is called before the first frame update
    void Start()
    {
        weaponObject = weapon.GetComponent<NetWeaponObject>();
        mSwitchWeapon = 0;
        mSwitchWeapon2 = 0;
        AxisCounts = 0;
        isScroll = false;
        isScrollCD = false;
        weaponType = "Gun";
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine&& PhotonNetwork.IsConnected)//如果观察不是当前角色以及网络连接上
        {
            return;
        }
        if (weaponObject.isShoot)
        {
            Debug.Log("射击");
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
    }

    // 提供攻击距离
    public float GetStrikingDistance()
    {
        float strikingDistance;
        strikingDistance = weaponObject.strikingDistance;
        return strikingDistance;
    }

    // 提供武器伤害
    public float GetDamage()
    {
        float weaponDamage;
        weaponDamage = weaponObject.damage;
        return weaponDamage;
    }

    // 滚轮控制武器切换
    private void SwitchWeapon(float MouseScrollWheel)
    {
        Destroy(weapon);

        AxisCounts = 0;

        if (MouseScrollWheel > 0)
            NextWeapon();
        if (MouseScrollWheel < 0)
            PreviousWeapon();

        GameObject weaponResource = Resources.Load<GameObject>("Weapons/" + weaponType + weaponIndex);
        if (weaponResource != null)
        {
            weapon = Instantiate(weaponResource);
            weapon.transform.parent = this.transform;
            weapon.transform.localPosition = weaponPosition;
            weapon.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            weapon.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
    }

    public void NextWeapon()//下一个武器
    {
        Debug.Log("1111");
        if (weaponType == "Gun")
        {
            weaponIndex++;
            if (weaponIndex > gunNum - 1)
            {
                weaponIndex = 0;
                weaponType = "Machete";
            }
        }
        else if (weaponType == "Machete")
        {
            weaponIndex++;
            if (weaponIndex > macheteNum - 1)
            {
                weaponIndex = 0;
                weaponType = "Gun";
            }
        }
    }

    public void PreviousWeapon()//上一个武器
    {
        Debug.Log("222");
        if (weaponType == "Gun")
        {
            weaponIndex--;
            if (weaponIndex < 0)
            {
                weaponIndex = macheteNum - 1;
                weaponType = "Machete";
            }
        }
        else if (weaponType == "Machete")
        {
            weaponIndex--;
            if (weaponIndex < 0)
            {
                weaponIndex = gunNum - 1;
                weaponType = "Gun";
            }
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
}
